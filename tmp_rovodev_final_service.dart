// Final service using both Expenses and Budgets tables correctly

import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../service/api/base/generic_handler.dart';
import 'tmp_rovodev_final_models.dart';
import 'tmp_rovodev_final_storage.dart';

part 'tmp_rovodev_final_service.g.dart';

@riverpod
CompleteFinancialService completeFinancialService(CompleteFinancialServiceRef ref) {
  return CompleteFinancialService(
    localStorage: ref.watch(completeLocalStorageProvider),
  );
}

class CompleteFinancialService {
  final CompleteLocalStorage _localStorage;
  
  // Using your generic handlers for all entities
  late final ApiService<FinancialAccount, String> _accountService;
  late final ApiService<Expense, String> _expenseService;
  late final ApiService<Budget, String> _budgetService;
  late final ApiService<ExpenseCategory, String> _categoryService;

  CompleteFinancialService({required CompleteLocalStorage localStorage}) 
      : _localStorage = localStorage {
    
    _accountService = ApiService<FinancialAccount, String>(
      endpoint: '/api/financial-accounts',
      fromJson: (json) => FinancialAccount.fromJson(json),
    );
    
    _expenseService = ApiService<Expense, String>(
      endpoint: '/api/expenses',
      fromJson: (json) => Expense.fromJson(json),
    );
    
    _budgetService = ApiService<Budget, String>(
      endpoint: '/api/budgets',
      fromJson: (json) => Budget.fromJson(json),
    );
    
    _categoryService = ApiService<ExpenseCategory, String>(
      endpoint: '/api/expense-categories',
      fromJson: (json) => ExpenseCategory.fromJson(json),
    );
  }

  // Get complete balance data (combines all data sources)
  Future<CompleteBalanceData> getCompleteBalanceData(String userId) async {
    try {
      // Get all data in parallel for better performance
      final futures = await Future.wait([
        _getUserAccounts(userId),
        _getUserExpensesForCurrentMonth(userId),
        _getUserBudgets(userId),
        _getExpenseCategories(),
      ]);
      
      final accounts = futures[0] as List<FinancialAccount>;
      final expenses = futures[1] as List<Expense>;
      final budgets = futures[2] as List<Budget>;
      final categories = futures[3] as List<ExpenseCategory>;
      
      // Get default account balance
      final defaultAccount = accounts.where((account) => account.isDefault).firstOrNull;
      final availableBalance = defaultAccount?.balance ?? 0.0;
      
      // Calculate monthly totals
      double monthlyIncome = 0;
      double monthlyExpenses = 0;
      
      for (final expense in expenses) {
        if (expense.type == ExpenseType.income) {
          monthlyIncome += expense.amount;
        } else {
          monthlyExpenses += expense.amount;
        }
      }
      
      // Enhance budgets with spent amounts and recent transactions
      final enhancedBudgets = await _enhanceBudgetsWithSpentData(budgets, expenses, categories);
      
      // Get recent transactions (last 10 across all categories)
      final recentTransactions = _getRecentTransactionsWithCategoryNames(expenses, categories);
      
      final completeData = CompleteBalanceData(
        userId: userId,
        availableBalance: availableBalance,
        monthlyIncome: monthlyIncome,
        monthlyExpenses: monthlyExpenses,
        lastUpdated: DateTime.now(),
        budgets: enhancedBudgets,
        recentTransactions: recentTransactions,
        accounts: accounts,
      );
      
      // Cache the complete data
      await _localStorage.saveCompleteBalanceData(completeData);
      
      return completeData;
    } catch (e) {
      // Try cached data if API fails
      final cachedData = await _localStorage.getCompleteBalanceData(userId);
      if (cachedData != null) {
        return cachedData.copyWith(error: 'Using cached data - $e');
      }
      throw Exception('Failed to get complete balance data: $e');
    }
  }

  // Update account balance
  Future<void> updateAccountBalance(String accountId, double newBalance) async {
    try {
      final updateData = UpdateAccountBalanceRequest(
        accountId: accountId,
        newBalance: newBalance,
      );
      
      await _accountService.update(accountId, updateData.toJson());
    } catch (e) {
      throw Exception('Failed to update account balance: $e');
    }
  }

  // Add expense/income transaction
  Future<Expense> addExpense(CreateExpenseRequest request, String userId) async {
    try {
      final expenseData = request.toJson();
      expenseData['userId'] = userId;
      expenseData['expensesId'] = _generateGuid();
      expenseData['createdDate'] = DateTime.now().toIso8601String();
      
      return await _expenseService.create(expenseData);
    } catch (e) {
      throw Exception('Failed to add expense: $e');
    }
  }

  // Set spending limit (create/update budget)
  Future<Budget> setBudget(CreateBudgetRequest request, String userId) async {
    try {
      final budgetData = request.toJson();
      budgetData['userId'] = userId;
      budgetData['budgetId'] = _generateGuid();
      
      return await _budgetService.create(budgetData);
    } catch (e) {
      throw Exception('Failed to set budget: $e');
    }
  }

  // Get recent transactions for a specific category
  Future<List<Expense>> getRecentTransactionsForCategory(String userId, String categoryId, {int limit = 5}) async {
    try {
      final allExpenses = await _expenseService.getAll();
      final categoryExpenses = allExpenses
          .where((expense) => 
            expense.userId == userId && 
            expense.exCId == categoryId &&
            expense.type == ExpenseType.expense)
          .toList();
      
      // Sort by date (newest first) and take limit
      categoryExpenses.sort((a, b) => b.createdDate.compareTo(a.createdDate));
      return categoryExpenses.take(limit).toList();
    } catch (e) {
      throw Exception('Failed to get recent transactions for category: $e');
    }
  }

  // Get expense categories
  Future<List<ExpenseCategory>> getExpenseCategories() async {
    try {
      return await _categoryService.getAll();
    } catch (e) {
      throw Exception('Failed to get expense categories: $e');
    }
  }

  // Get cached complete balance data
  Future<CompleteBalanceData?> getCachedCompleteBalanceData(String userId) async {
    return await _localStorage.getCompleteBalanceData(userId);
  }

  // Private helper methods
  Future<List<FinancialAccount>> _getUserAccounts(String userId) async {
    final allAccounts = await _accountService.getAll();
    return allAccounts.where((account) => account.userId == userId).toList();
  }

  Future<List<Expense>> _getUserExpensesForCurrentMonth(String userId) async {
    final allExpenses = await _expenseService.getAll();
    final currentMonth = DateTime.now();
    
    return allExpenses.where((expense) => 
      expense.userId == userId &&
      expense.createdDate.year == currentMonth.year &&
      expense.createdDate.month == currentMonth.month
    ).toList();
  }

  Future<List<Budget>> _getUserBudgets(String userId) async {
    final allBudgets = await _budgetService.getAll();
    return allBudgets.where((budget) => budget.userId == userId).toList();
  }

  Future<List<ExpenseCategory>> _getExpenseCategories() async {
    return await _categoryService.getAll();
  }

  // Enhance budgets with spent amounts and recent transactions
  Future<List<Budget>> _enhanceBudgetsWithSpentData(
    List<Budget> budgets, 
    List<Expense> expenses, 
    List<ExpenseCategory> categories
  ) async {
    final enhancedBudgets = <Budget>[];
    
    for (final budget in budgets) {
      // Calculate spent amount for this budget
      final relevantExpenses = expenses.where((expense) =>
        expense.exCId == budget.categoryId &&
        expense.type == ExpenseType.expense &&
        expense.createdDate.isAfter(budget.startDate) &&
        expense.createdDate.isBefore(budget.endDate.add(const Duration(days: 1)))
      ).toList();
      
      final spentAmount = relevantExpenses.fold(0.0, (sum, expense) => sum + expense.amount);
      
      // Get recent transactions for this budget (last 3)
      final recentExpenses = relevantExpenses
          .toList()
        ..sort((a, b) => b.createdDate.compareTo(a.createdDate));
      final recentTransactions = recentExpenses.take(3).toList();
      
      // Get category name
      final category = categories.where((cat) => cat.exCId == budget.categoryId).firstOrNull;
      
      enhancedBudgets.add(budget.copyWith(
        spentAmount: spentAmount,
        recentExpenses: recentTransactions,
        categoryName: category?.categoryName,
      ));
    }
    
    return enhancedBudgets;
  }

  // Get recent transactions with category names
  List<Expense> _getRecentTransactionsWithCategoryNames(
    List<Expense> expenses, 
    List<ExpenseCategory> categories
  ) {
    // Sort by date (newest first) and take last 10
    final sortedExpenses = expenses.toList()
      ..sort((a, b) => b.createdDate.compareTo(a.createdDate));
    
    final recentExpenses = sortedExpenses.take(10).toList();
    
    // Add category names
    return recentExpenses.map((expense) {
      final category = categories.where((cat) => cat.exCId == expense.exCId).firstOrNull;
      return expense.copyWith(categoryName: category?.categoryName);
    }).toList();
  }

  String _generateGuid() {
    return DateTime.now().millisecondsSinceEpoch.toString();
  }
}
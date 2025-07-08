// Final Riverpod providers using both Expenses and Budgets tables

import 'package:riverpod_annotation/riverpod_annotation.dart';
import 'tmp_rovodev_final_models.dart';
import 'tmp_rovodev_final_service.dart';

part 'tmp_rovodev_final_provider.g.dart';

// Main complete balance data provider
@riverpod
class CompleteBalanceNotifier extends _$CompleteBalanceNotifier {
  @override
  Future<CompleteBalanceData> build(String userId) async {
    // Try to get cached data first for immediate UI update
    final service = ref.watch(completeFinancialServiceProvider);
    final cachedData = await service.getCachedCompleteBalanceData(userId);
    
    if (cachedData != null) {
      // Return cached data immediately, then fetch fresh data
      _fetchFreshData(userId);
      return cachedData;
    }
    
    // No cached data, fetch from API
    return await service.getCompleteBalanceData(userId);
  }

  // Fetch fresh data in background
  Future<void> _fetchFreshData(String userId) async {
    try {
      final service = ref.read(completeFinancialServiceProvider);
      final freshData = await service.getCompleteBalanceData(userId);
      state = AsyncData(freshData);
    } catch (e) {
      // Don't update state if background fetch fails
      // User still sees cached data
    }
  }

  // Update account balance with optimistic update
  Future<void> updateAccountBalance(String accountId, double newBalance) async {
    final currentData = state.value;
    if (currentData == null) return;

    // Find the account to update
    final accountIndex = currentData.accounts.indexWhere(
      (account) => account.accountId == accountId,
    );
    
    if (accountIndex == -1) return;

    // Optimistic update - update UI immediately
    final updatedAccounts = [...currentData.accounts];
    updatedAccounts[accountIndex] = updatedAccounts[accountIndex].copyWith(
      balance: newBalance,
    );
    
    final updatedAvailableBalance = updatedAccounts[accountIndex].isDefault
        ? newBalance 
        : currentData.availableBalance;
    
    final optimisticData = currentData.copyWith(
      accounts: updatedAccounts,
      availableBalance: updatedAvailableBalance,
      lastUpdated: DateTime.now(),
    );
    state = AsyncData(optimisticData);

    try {
      final service = ref.read(completeFinancialServiceProvider);
      await service.updateAccountBalance(accountId, newBalance);
      
      // Refresh data from server to get accurate state
      await _fetchFreshData(currentData.userId);
    } catch (e) {
      // Revert optimistic update on error
      state = AsyncData(currentData.copyWith(
        error: 'Failed to update balance: $e',
      ));
    }
  }

  // Set spending limit (budget) with optimistic update
  Future<void> setBudget({
    required String categoryId,
    required String accountId,
    required double budgetAmount,
    required DateTime startDate,
    required DateTime endDate,
  }) async {
    final currentData = state.value;
    if (currentData == null) return;

    try {
      final service = ref.read(completeFinancialServiceProvider);
      final request = CreateBudgetRequest(
        categoryId: categoryId,
        accountId: accountId,
        budgetAmount: budgetAmount,
        startDate: startDate,
        endDate: endDate,
      );
      
      final newBudget = await service.setBudget(request, currentData.userId);

      // Update local state
      final updatedBudgets = [...currentData.budgets];
      final existingIndex = updatedBudgets.indexWhere(
        (budget) => budget.categoryId == categoryId && budget.accountId == accountId,
      );

      if (existingIndex >= 0) {
        updatedBudgets[existingIndex] = newBudget;
      } else {
        updatedBudgets.add(newBudget);
      }

      final updatedData = currentData.copyWith(
        budgets: updatedBudgets,
        lastUpdated: DateTime.now(),
      );

      state = AsyncData(updatedData);
      
      // Refresh to get accurate spent amounts
      await _fetchFreshData(currentData.userId);
    } catch (e) {
      state = AsyncData(currentData.copyWith(
        error: 'Failed to set budget: $e',
      ));
    }
  }

  // Add expense/income with optimistic update
  Future<void> addExpense({
    required double amount,
    required String categoryId,
    required String accountId,
    required String description,
    required ExpenseType type,
    ExpenseFrequency frequency = ExpenseFrequency.once,
  }) async {
    final currentData = state.value;
    if (currentData == null) return;

    // Find the account to update
    final accountIndex = currentData.accounts.indexWhere(
      (account) => account.accountId == accountId,
    );
    
    if (accountIndex == -1) return;

    // Calculate new balance based on expense type
    final balanceChange = type == ExpenseType.income ? amount : -amount;
    final currentAccount = currentData.accounts[accountIndex];
    final newBalance = currentAccount.balance + balanceChange;

    // Update account balance
    final updatedAccounts = [...currentData.accounts];
    updatedAccounts[accountIndex] = currentAccount.copyWith(balance: newBalance);

    // Update available balance if it's the default account
    final updatedAvailableBalance = currentAccount.isDefault 
        ? newBalance 
        : currentData.availableBalance;

    // Update monthly totals
    final newMonthlyIncome = type == ExpenseType.income 
        ? currentData.monthlyIncome + amount 
        : currentData.monthlyIncome;
    final newMonthlyExpenses = type == ExpenseType.expense 
        ? currentData.monthlyExpenses + amount 
        : currentData.monthlyExpenses;

    // Update budgets if it's an expense
    final updatedBudgets = currentData.budgets.map((budget) {
      if (budget.categoryId == categoryId && type == ExpenseType.expense) {
        return budget.copyWith(spentAmount: budget.spentAmount + amount);
      }
      return budget;
    }).toList();

    // Create new expense for recent transactions
    final newExpense = Expense(
      expensesId: DateTime.now().millisecondsSinceEpoch.toString(),
      amount: amount,
      description: description,
      createdDate: DateTime.now(),
      type: type,
      exCId: categoryId,
      accountId: accountId,
      userId: currentData.userId,
    );

    // Add to recent transactions (keep only last 10)
    final updatedRecentTransactions = [newExpense, ...currentData.recentTransactions];
    if (updatedRecentTransactions.length > 10) {
      updatedRecentTransactions.removeLast();
    }

    // Optimistic update
    final optimisticData = currentData.copyWith(
      monthlyIncome: newMonthlyIncome,
      monthlyExpenses: newMonthlyExpenses,
      availableBalance: updatedAvailableBalance,
      accounts: updatedAccounts,
      budgets: updatedBudgets,
      recentTransactions: updatedRecentTransactions,
      lastUpdated: DateTime.now(),
    );
    state = AsyncData(optimisticData);

    try {
      final service = ref.read(completeFinancialServiceProvider);
      final request = CreateExpenseRequest(
        amount: amount,
        description: description,
        type: type,
        frequency: frequency,
        exCId: categoryId,
        accountId: accountId,
      );
      
      await service.addExpense(request, currentData.userId);

      // Refresh data from server to get accurate state
      await _fetchFreshData(currentData.userId);
    } catch (e) {
      // Revert optimistic update on error
      state = AsyncData(currentData.copyWith(
        error: 'Failed to add expense: $e',
      ));
    }
  }

  // Refresh data manually
  Future<void> refresh() async {
    final currentData = state.value;
    if (currentData == null) return;
    
    state = const AsyncLoading();
    try {
      final service = ref.read(completeFinancialServiceProvider);
      final freshData = await service.getCompleteBalanceData(currentData.userId);
      state = AsyncData(freshData);
    } catch (e) {
      state = AsyncError(e, StackTrace.current);
    }
  }
}

// Computed providers for specific data
@riverpod
double availableBalance(AvailableBalanceRef ref, String userId) {
  final balanceAsync = ref.watch(completeBalanceNotifierProvider(userId));
  return balanceAsync.when(
    data: (data) => data.availableBalance,
    loading: () => 0.0,
    error: (_, __) => 0.0,
  );
}

@riverpod
List<Budget> activeBudgets(ActiveBudgetsRef ref, String userId) {
  final balanceAsync = ref.watch(completeBalanceNotifierProvider(userId));
  return balanceAsync.when(
    data: (data) {
      final now = DateTime.now();
      return data.budgets.where((budget) => 
        budget.startDate.isBefore(now) && budget.endDate.isAfter(now)
      ).toList();
    },
    loading: () => [],
    error: (_, __) => [],
  );
}

@riverpod
Budget? budgetForCategory(BudgetForCategoryRef ref, String userId, String categoryId) {
  final budgets = ref.watch(activeBudgetsProvider(userId));
  try {
    return budgets.firstWhere((budget) => budget.categoryId == categoryId);
  } catch (e) {
    return null;
  }
}

@riverpod
List<Expense> recentTransactions(RecentTransactionsRef ref, String userId) {
  final balanceAsync = ref.watch(completeBalanceNotifierProvider(userId));
  return balanceAsync.when(
    data: (data) => data.recentTransactions,
    loading: () => [],
    error: (_, __) => [],
  );
}

@riverpod
List<Expense> recentTransactionsForCategory(
  RecentTransactionsForCategoryRef ref, 
  String userId, 
  String categoryId
) {
  final balanceAsync = ref.watch(completeBalanceNotifierProvider(userId));
  return balanceAsync.when(
    data: (data) => data.getRecentTransactionsForCategory(categoryId),
    loading: () => [],
    error: (_, __) => [],
  );
}

@riverpod
FinancialAccount? defaultAccount(DefaultAccountRef ref, String userId) {
  final balanceAsync = ref.watch(completeBalanceNotifierProvider(userId));
  return balanceAsync.when(
    data: (data) => data.accounts.where((account) => account.isDefault).firstOrNull,
    loading: () => null,
    error: (_, __) => null,
  );
}

// Provider for expense categories
@riverpod
class ExpenseCategoriesNotifier extends _$ExpenseCategoriesNotifier {
  @override
  Future<List<ExpenseCategory>> build() async {
    final service = ref.watch(completeFinancialServiceProvider);
    return await service.getExpenseCategories();
  }

  Future<void> refresh() async {
    state = const AsyncLoading();
    try {
      final service = ref.read(completeFinancialServiceProvider);
      final categories = await service.getExpenseCategories();
      state = AsyncData(categories);
    } catch (e) {
      state = AsyncError(e, StackTrace.current);
    }
  }
}

// Provider for budget health (how many budgets are over limit)
@riverpod
Map<String, int> budgetHealth(BudgetHealthRef ref, String userId) {
  final budgets = ref.watch(activeBudgetsProvider(userId));
  
  int overBudget = 0;
  int nearLimit = 0; // >80% of budget
  int healthy = 0;   // <80% of budget
  
  for (final budget in budgets) {
    if (budget.isOverBudget) {
      overBudget++;
    } else if (budget.spentPercentage > 80) {
      nearLimit++;
    } else {
      healthy++;
    }
  }
  
  return {
    'overBudget': overBudget,
    'nearLimit': nearLimit,
    'healthy': healthy,
    'total': budgets.length,
  };
}
// Final models using both Expenses and Budgets tables correctly

import 'package:freezed_annotation/freezed_annotation.dart';

part 'tmp_rovodev_final_models.freezed.dart';
part 'tmp_rovodev_final_models.g.dart';

// Financial Account model (matches FinancialAccounts table)
@freezed
class FinancialAccount with _$FinancialAccount {
  const factory FinancialAccount({
    required String accountId,
    required String accountName,
    required double balance,
    @Default('VND') String currencyCode,
    required String userId,
    @Default(false) bool isDefault,
  }) = _FinancialAccount;

  factory FinancialAccount.fromJson(Map<String, dynamic> json) =>
      _$FinancialAccountFromJson(json);
}

// Expense Category model (matches ExpensesCategories table)
@freezed
class ExpenseCategory with _$ExpenseCategory {
  const factory ExpenseCategory({
    required String exCId,
    required String categoryName,
    String? description,
  }) = _ExpenseCategory;

  factory ExpenseCategory.fromJson(Map<String, dynamic> json) =>
      _$ExpenseCategoryFromJson(json);
}

// Expense model (matches Expenses table) - for individual transactions
@freezed
class Expense with _$Expense {
  const factory Expense({
    required String expensesId,
    required double amount,
    String? description,
    required DateTime createdDate,
    required ExpenseType type,
    @Default(ExpenseFrequency.once) ExpenseFrequency frequency,
    DateTime? nextDueDate,
    required String exCId,      // links to ExpenseCategory
    required String accountId,  // links to FinancialAccount
    required String userId,
    
    // Additional fields for UI
    String? categoryName,       // populated from join with ExpenseCategory
  }) = _Expense;

  factory Expense.fromJson(Map<String, dynamic> json) =>
      _$ExpenseFromJson(json);
}

// Budget model (matches Budgets table) - for spending limits
@freezed
class Budget with _$Budget {
  const factory Budget({
    required String budgetId,
    required String categoryId,    // links to ExpenseCategory (exCId)
    required String accountId,     // links to FinancialAccount
    required double budgetAmount,  // spending limit
    required DateTime startDate,
    required DateTime endDate,
    required String userId,
    
    // Calculated fields (not in database)
    @Default(0.0) double spentAmount,     // calculated from Expenses
    @Default([]) List<Expense> recentExpenses, // recent expenses in this category
    String? categoryName,                 // populated from join
  }) = _Budget;

  // Computed properties
  double get remainingAmount => budgetAmount - spentAmount;
  double get spentPercentage => budgetAmount > 0 ? (spentAmount / budgetAmount) * 100 : 0;
  bool get isOverBudget => spentAmount > budgetAmount;

  factory Budget.fromJson(Map<String, dynamic> json) =>
      _$BudgetFromJson(json);
}

// Complete balance data combining everything
@freezed
class CompleteBalanceData with _$CompleteBalanceData {
  const factory CompleteBalanceData({
    required String userId,
    required double availableBalance,    // from default FinancialAccount
    required double monthlyIncome,       // calculated from Expenses
    required double monthlyExpenses,     // calculated from Expenses
    required DateTime lastUpdated,
    @Default([]) List<Budget> budgets,           // spending limits with spent amounts
    @Default([]) List<Expense> recentTransactions, // recent expenses across all categories
    @Default([]) List<FinancialAccount> accounts,
    @Default(false) bool isLoading,
    String? error,
  }) = _CompleteBalanceData;

  // Computed properties
  double get netSavings => monthlyIncome - monthlyExpenses;
  
  // Get recent transactions for a specific category
  List<Expense> getRecentTransactionsForCategory(String categoryId) {
    return recentTransactions
        .where((expense) => expense.exCId == categoryId)
        .take(5) // Last 5 transactions
        .toList();
  }

  factory CompleteBalanceData.fromJson(Map<String, dynamic> json) =>
      _$CompleteBalanceDataFromJson(json);
}

// Enums
enum ExpenseType {
  @JsonValue('income')
  income,
  @JsonValue('expense')
  expense,
}

enum ExpenseFrequency {
  @JsonValue('once')
  once,
  @JsonValue('daily')
  daily,
  @JsonValue('weekly')
  weekly,
  @JsonValue('monthly')
  monthly,
}

// Request DTOs for API calls
@freezed
class CreateExpenseRequest with _$CreateExpenseRequest {
  const factory CreateExpenseRequest({
    required double amount,
    String? description,
    required ExpenseType type,
    @Default(ExpenseFrequency.once) ExpenseFrequency frequency,
    DateTime? nextDueDate,
    required String exCId,      // category ID
    required String accountId,  // account ID
  }) = _CreateExpenseRequest;

  factory CreateExpenseRequest.fromJson(Map<String, dynamic> json) =>
      _$CreateExpenseRequestFromJson(json);
}

@freezed
class CreateBudgetRequest with _$CreateBudgetRequest {
  const factory CreateBudgetRequest({
    required String categoryId,
    required String accountId,
    required double budgetAmount,
    required DateTime startDate,
    required DateTime endDate,
  }) = _CreateBudgetRequest;

  factory CreateBudgetRequest.fromJson(Map<String, dynamic> json) =>
      _$CreateBudgetRequestFromJson(json);
}

@freezed
class UpdateAccountBalanceRequest with _$UpdateAccountBalanceRequest {
  const factory UpdateAccountBalanceRequest({
    required String accountId,
    required double newBalance,
  }) = _UpdateAccountBalanceRequest;

  factory UpdateAccountBalanceRequest.fromJson(Map<String, dynamic> json) =>
      _$UpdateAccountBalanceRequestFromJson(json);
}

// Response DTO for recent transactions with category info
@freezed
class RecentTransactionResponse with _$RecentTransactionResponse {
  const factory RecentTransactionResponse({
    required String expensesId,
    required double amount,
    String? description,
    required DateTime createdDate,
    required ExpenseType type,
    required String categoryId,
    required String categoryName,
    required String accountId,
  }) = _RecentTransactionResponse;

  factory RecentTransactionResponse.fromJson(Map<String, dynamic> json) =>
      _$RecentTransactionResponseFromJson(json);
}
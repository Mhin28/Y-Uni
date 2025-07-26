using System;
using System.Collections.Generic;

namespace Repositories.ViewModels.FinancialDashboardModel
{
    public class CompleteBalanceDataDto
    {
        public Guid UserId { get; set; }
        public decimal AvailableBalance { get; set; }
        public decimal MonthlyIncome { get; set; }
        public decimal MonthlyExpenses { get; set; }
        public decimal NetSavings { get; set; }
        public DateTime LastUpdated { get; set; }
        public List<EnhancedBudgetDto> Budgets { get; set; } = new List<EnhancedBudgetDto>();
        public List<FinancialAccountDto> Accounts { get; set; } = new List<FinancialAccountDto>();
    }

    public class EnhancedBudgetDto
    {
        public Guid BudgetId { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public Guid AccountId { get; set; }
        public decimal BudgetAmount { get; set; }
        public decimal SpentAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public double SpentPercentage { get; set; }
        public bool IsOverBudget { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class RecentTransactionDto
    {
        public Guid ExpenseId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public Guid? UserId { get; set; }
        public Guid? AccountId { get; set; }
    }

    public class FinancialAccountDto
    {
        public Guid AccountId { get; set; }
        public string AccountName { get; set; }
        public decimal Balance { get; set; }
        public string CurrencyCode { get; set; }
        public bool IsDefault { get; set; }
    }

    public class BudgetSummaryDto
    {
        public Guid UserId { get; set; }
        public int TotalBudgets { get; set; }
        public int OverBudgetCount { get; set; }
        public decimal TotalBudgetAmount { get; set; }
        public decimal TotalSpentAmount { get; set; }
        public decimal TotalRemainingAmount { get; set; }
        public double OverallSpentPercentage { get; set; }
        public List<EnhancedBudgetDto> BudgetDetails { get; set; } = new List<EnhancedBudgetDto>();
    }

    public class MonthlySummaryDto
    {
        public Guid UserId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetAmount { get; set; }
        public int TransactionCount { get; set; }
        public List<CategorySummaryDto> CategoryBreakdown { get; set; } = new List<CategorySummaryDto>();
        public List<RecentTransactionDto> Transactions { get; set; } = new List<RecentTransactionDto>();
    }

    public class CategorySummaryDto
    {
        public Guid? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public decimal TotalAmount { get; set; }
        public int TransactionCount { get; set; }
        public decimal BudgetAmount { get; set; }
        public decimal RemainingBudget { get; set; }
        public bool IsOverBudget { get; set; }
    }

    public class BudgetUtilizationDto
    {
        public Guid UserId { get; set; }
        public List<BudgetUtilizationItemDto> BudgetItems { get; set; } = new List<BudgetUtilizationItemDto>();
        public int OverBudgetCount { get; set; }
        public int NearLimitCount { get; set; } // Budgets at 80%+ utilization
        public int HealthyCount { get; set; } // Budgets under 80% utilization
        public int TotalBudgets { get; set; }
        public decimal TotalOverBudgetAmount { get; set; }
    }

    // Monthly Balance Carry-Over DTOs (from implementation guide)
    public class MonthlyBalanceDto
    {
        public Guid UserId { get; set; }
        public DateTime Month { get; set; }
        public decimal InitialBalance { get; set; }
        public decimal TotalBudgetAllocated { get; set; }
        public decimal RemainingForSavings { get; set; }
        public decimal TotalUnusedBudget { get; set; }
        public decimal TotalCarryOverToNextBalance { get; set; }
        public decimal TotalQueuedIncome { get; set; }
        public decimal NextMonthInitialBalance { get; set; }
        public List<EnhancedBudgetDto> CategoryBudgets { get; set; } = new List<EnhancedBudgetDto>();
        public List<QueuedIncomeDto> QueuedIncomes { get; set; } = new List<QueuedIncomeDto>();
        public bool IsActive { get; set; }
    }

    public class QueuedIncomeDto
    {
        public Guid IncomeId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime ExpectedDate { get; set; }
        public bool IsProcessed { get; set; }
        public string Source { get; set; }
    }

    // Enhanced Monthly Summary with Carry-Over Logic
    public class EnhancedMonthlySummaryDto
    {
        public Guid UserId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetAmount { get; set; }
        public int TransactionCount { get; set; }
        public List<CategorySummaryDto> CategoryBreakdown { get; set; } = new List<CategorySummaryDto>();
        public List<RecentTransactionDto> Transactions { get; set; } = new List<RecentTransactionDto>();
        
        // Carry-over calculations
        public decimal TotalUnusedBudget { get; set; }
        public decimal CarryOverToNext { get; set; }
        public decimal PreviousMonthCarryOver { get; set; }
        public decimal AdjustedNetAmount { get; set; }
    }

    public class BudgetUtilizationItemDto
    {
        public Guid BudgetId { get; set; }
        public string CategoryName { get; set; }
        public decimal BudgetAmount { get; set; }
        public decimal SpentAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public double UtilizationPercentage { get; set; }
        public string Status { get; set; } // "Safe", "Warning", "Over Budget"
        public int DaysRemaining { get; set; }
        public DateTime EndDate { get; set; }
    }
}
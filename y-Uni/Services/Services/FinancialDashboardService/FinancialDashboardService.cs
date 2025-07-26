using Repositories.Models;
using Repositories.Repositories;
using Repositories.ViewModels.FinancialDashboardModel;
using Repositories.ViewModels.ResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Services.Services.FinancialDashboardService
{
    public class FinancialDashboardService : IFinancialDashboardService
    {
        private readonly IBudgetRepo _budgetRepo;
        private readonly IExpenseRepo _expenseRepo;
        private readonly IFinancialAccountRepo _accountRepo;
        private readonly IExpensesCategoryRepo _categoryRepo;

        public FinancialDashboardService(
            IBudgetRepo budgetRepo,
            IExpenseRepo expenseRepo,
            IFinancialAccountRepo accountRepo,
            IExpensesCategoryRepo categoryRepo)
        {
            _budgetRepo = budgetRepo;
            _expenseRepo = expenseRepo;
            _accountRepo = accountRepo;
            _categoryRepo = categoryRepo;
        }

        public async Task<ResultModel> GetCompleteBalanceDataAsync(Guid userId)
        {
            var result = new ResultModel();
            try
            {
                // Get all required data sequentially to avoid DbContext threading issues
                var accounts = await _accountRepo.GetUserAccountsAsync(userId);
                var expenses = await _expenseRepo.GetUserExpensesForCurrentMonthAsync(userId);
                var budgets = await _budgetRepo.GetActiveBudgetsForUserAsync(userId, DateTime.Now);
                var categories = await _categoryRepo.GetAllAsync();

                // Calculate available balance (same as frontend logic)
                var availableBalance = accounts
                    .Where(a => a.IsDefault == true)
                    .FirstOrDefault()?.Balance ?? 0;

                // Calculate monthly totals (enhanced logic matching frontend)
                var monthlyIncome = expenses
                    .Where(e => e.ExC != null && e.ExC.Type == "income") // Income transactions
                    .Sum(e => e.Amount);

                var monthlyExpenses = expenses
                    .Where(e => e.ExC != null && e.ExC.Type == "expense") // Expense transactions
                    .Sum(e => e.Amount);

                // Enhance budgets with spent data (critical - same as frontend logic)
                var enhancedBudgets = await EnhanceBudgetsWithSpentData(budgets, expenses, categories, userId);

                // Map accounts to DTOs with enhanced data
                var accountDtos = accounts.Select(a => new FinancialAccountDto
                {
                    AccountId = a.AccountId,
                    AccountName = a.AccountName,
                    Balance = a.Balance ?? 0,
                    CurrencyCode = a.CurrencyCode ?? "VND",
                    IsDefault = a.IsDefault ?? false
                }).ToList();

                // Build comprehensive response (matching guide structure)
                var completeData = new CompleteBalanceDataDto
                {
                    UserId = userId,
                    AvailableBalance = availableBalance,
                    MonthlyIncome = monthlyIncome,
                    MonthlyExpenses = monthlyExpenses,
                    NetSavings = monthlyIncome - monthlyExpenses,
                    LastUpdated = DateTime.UtcNow,
                    Budgets = enhancedBudgets,
                    Accounts = accountDtos
                };

                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.OK;
                result.Data = completeData;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = (int)HttpStatusCode.InternalServerError;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<ResultModel> GetBudgetSummaryAsync(Guid userId)
        {
            var result = new ResultModel();
            try
            {
                var budgets = await _budgetRepo.GetUserBudgetsAsync(userId);
                var expenses = await _expenseRepo.GetUserExpensesAsync(userId);
                var categories = await _categoryRepo.GetAllAsync();

                var enhancedBudgets = await EnhanceBudgetsWithSpentData(budgets, expenses, categories, userId);

                var budgetSummary = new BudgetSummaryDto
                {
                    UserId = userId,
                    TotalBudgets = enhancedBudgets.Count,
                    OverBudgetCount = enhancedBudgets.Count(b => b.IsOverBudget),
                    TotalBudgetAmount = enhancedBudgets.Sum(b => b.BudgetAmount),
                    TotalSpentAmount = enhancedBudgets.Sum(b => b.SpentAmount),
                    TotalRemainingAmount = enhancedBudgets.Sum(b => b.RemainingAmount),
                    BudgetDetails = enhancedBudgets
                };

                budgetSummary.OverallSpentPercentage = budgetSummary.TotalBudgetAmount > 0 
                    ? (double)(budgetSummary.TotalSpentAmount / budgetSummary.TotalBudgetAmount) * 100 
                    : 0;

                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.OK;
                result.Data = budgetSummary;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = (int)HttpStatusCode.InternalServerError;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<ResultModel> GetMonthlySummaryAsync(Guid userId, int year, int month)
        {
            var result = new ResultModel();
            try
            {
                var expenses = await _expenseRepo.GetUserExpensesForMonthAsync(userId, year, month);
                var categories = await _categoryRepo.GetAllAsync();
                var budgets = await _budgetRepo.GetUserBudgetsForMonthAsync(userId, year, month);

                var totalIncome = expenses.Where(e => e.ExC != null && e.ExC.Type == "income").Sum(e => e.Amount);
                var totalExpenses = expenses.Where(e => e.ExC != null && e.ExC.Type == "expense").Sum(e => e.Amount);

                // Group expenses by category
                var categoryBreakdown = expenses
                    .Where(e => e.ExC != null && e.ExC.Type == "expense") // Only expenses, not income
                    .GroupBy(e => e.ExCid)
                    .Select(g =>
                    {
                        var category = categories.FirstOrDefault(c => c.ExCid == g.Key);
                        var budget = budgets.FirstOrDefault(b => b.CategoryId == g.Key);
                        var totalAmount = g.Sum(e => e.Amount);

                        return new CategorySummaryDto
                        {
                            CategoryId = g.Key,
                            CategoryName = category?.CategoryName ?? "Unknown",
                            TotalAmount = totalAmount,
                            TransactionCount = g.Count(),
                            BudgetAmount = budget?.BudgetAmount ?? 0,
                            RemainingBudget = (budget?.BudgetAmount ?? 0) - totalAmount,
                            IsOverBudget = totalAmount > (budget?.BudgetAmount ?? 0)
                        };
                    }).ToList();

                // Get transaction details - map expenses to transaction DTOs
                var transactions = expenses
                    .OrderByDescending(e => e.CreatedDate)
                    .Select(e =>
                    {
                        var category = categories.FirstOrDefault(c => c.ExCid == e.ExCid);
                        return new RecentTransactionDto
                        {
                            ExpenseId = e.ExpensesId,
                            Amount = e.Amount,
                            Description = e.Description ?? "",
                            CreatedDate = e.CreatedDate ?? DateTime.Now,
                            CategoryId = e.ExCid ?? Guid.Empty,
                            CategoryName = category?.CategoryName ?? "Unknown",
                            AccountId = e.AccountId ?? Guid.Empty,
                        };
                    }).ToList();

                var monthlySummary = new MonthlySummaryDto
                {
                    UserId = userId,
                    Year = year,
                    Month = month,
                    TotalIncome = totalIncome,
                    TotalExpenses = totalExpenses,
                    NetAmount = totalIncome - totalExpenses,
                    TransactionCount = expenses.Count,
                    CategoryBreakdown = categoryBreakdown,
                    Transactions = transactions
                };

                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.OK;
                result.Data = monthlySummary;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = (int)HttpStatusCode.InternalServerError;
                result.Message = ex.Message;
            }
            return result;
        }



        public async Task<ResultModel> GetBudgetUtilizationAsync(Guid userId)
        {
            var result = new ResultModel();
            try
            {
                var budgets = await _budgetRepo.GetActiveBudgetsForUserAsync(userId, DateTime.Now);
                var expenses = await _expenseRepo.GetUserExpensesAsync(userId);
                var categories = await _categoryRepo.GetAllAsync();

                var utilizationItems = new List<BudgetUtilizationItemDto>();
                var overBudgetCount = 0;
                var nearLimitCount = 0; // >80% of budget
                var healthyCount = 0;   // <80% of budget
                var totalOverBudgetAmount = 0m;

                // Calculate budget health (same as frontend logic)
                foreach (var budget in budgets)
                {
                    var category = categories.FirstOrDefault(c => c.ExCid == budget.CategoryId);
                    
                    // Calculate spent amount using same logic as EnhanceBudgetsWithSpentData
                    var relevantExpenses = expenses.Where(e =>
                        e.UserId == userId &&
                        e.ExCid == budget.CategoryId &&
                        e.ExC != null && e.ExC.Type == "expense" && // Only expense transactions
                        e.CreatedDate >= budget.StartDate.ToDateTime(TimeOnly.MinValue) &&
                        e.CreatedDate <= budget.EndDate.ToDateTime(TimeOnly.MaxValue).AddDays(1)
                    );

                    var spentAmount = relevantExpenses.Sum(e => e.Amount);
                    var remainingAmount = budget.BudgetAmount - spentAmount;
                    var utilizationPercentage = budget.BudgetAmount > 0 
                        ? (double)(spentAmount / budget.BudgetAmount) * 100 
                        : 0;

                    // Determine status based on utilization percentage
                    string status;
                    if (spentAmount > budget.BudgetAmount)
                    {
                        overBudgetCount++;
                        status = "Over Budget";
                        totalOverBudgetAmount += spentAmount - budget.BudgetAmount;
                    }
                    else if (utilizationPercentage > 80)
                    {
                        nearLimitCount++;
                        status = "Warning";
                    }
                    else
                    {
                        healthyCount++;
                        status = "Safe";
                    }

                    var daysRemaining = Math.Max(0, (budget.EndDate.ToDateTime(TimeOnly.MinValue) - DateTime.Now).Days);

                    utilizationItems.Add(new BudgetUtilizationItemDto
                    {
                        BudgetId = budget.BudgetId,
                        CategoryName = category?.CategoryName ?? "Unknown",
                        BudgetAmount = budget.BudgetAmount,
                        SpentAmount = spentAmount,
                        RemainingAmount = remainingAmount,
                        UtilizationPercentage = utilizationPercentage,
                        Status = status,
                        DaysRemaining = daysRemaining,
                        EndDate = budget.EndDate.ToDateTime(TimeOnly.MinValue)
                    });
                }

                // Build comprehensive utilization response
                var utilization = new BudgetUtilizationDto
                {
                    UserId = userId,
                    BudgetItems = utilizationItems.OrderByDescending(b => b.UtilizationPercentage).ToList(),
                    OverBudgetCount = overBudgetCount,
                    NearLimitCount = nearLimitCount,
                    HealthyCount = healthyCount,
                    TotalBudgets = budgets.Count(),
                    TotalOverBudgetAmount = totalOverBudgetAmount
                };

                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.OK;
                result.Data = utilization;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = (int)HttpStatusCode.InternalServerError;
                result.Message = ex.Message;
            }
            return result;
        }

        // Private helper methods
        private async Task<List<EnhancedBudgetDto>> EnhanceBudgetsWithSpentData(
            IEnumerable<Budget> budgets, 
            IEnumerable<Expense> expenses, 
            IEnumerable<ExpensesCategory> categories, 
            Guid userId)
        {
            var enhancedBudgets = new List<EnhancedBudgetDto>();

            foreach (var budget in budgets)
            {
                var category = categories.FirstOrDefault(c => c.ExCid == budget.CategoryId);
                
                // Calculate spent amount (exact same logic as frontend)
                var relevantExpenses = expenses.Where(e =>
                    e.UserId == userId &&
                    e.ExCid == budget.CategoryId &&
                    e.ExC != null && e.ExC.Type == "expense" && // Only expense transactions
                    e.CreatedDate >= budget.StartDate.ToDateTime(TimeOnly.MinValue) &&
                    e.CreatedDate <= budget.EndDate.ToDateTime(TimeOnly.MaxValue).AddDays(1) // Include end date
                ).ToList();

                var spentAmount = relevantExpenses.Sum(e => Math.Abs(e.Amount));
                
                // Calculate derived values (same as BudgetExtensions)
                var remainingAmount = budget.BudgetAmount - spentAmount;
                var spentPercentage = budget.BudgetAmount > 0 ? (double)(spentAmount / budget.BudgetAmount) * 100 : 0;
                var isOverBudget = spentAmount > budget.BudgetAmount;
                var unusedAmount = remainingAmount > 0 ? remainingAmount : 0;

                enhancedBudgets.Add(new EnhancedBudgetDto
                {
                    BudgetId = budget.BudgetId,
                    CategoryId = budget.CategoryId ?? Guid.Empty,
                    CategoryName = category?.CategoryName ?? "Unknown",
                    AccountId = budget.AccountId ?? Guid.Empty,
                    BudgetAmount = budget.BudgetAmount,
                    SpentAmount = spentAmount,
                    RemainingAmount = remainingAmount,
                    SpentPercentage = spentPercentage,
                    IsOverBudget = isOverBudget,
                    StartDate = budget.StartDate.ToDateTime(TimeOnly.MinValue),
                    EndDate = budget.EndDate.ToDateTime(TimeOnly.MaxValue)
                });
            }

            return enhancedBudgets.OrderByDescending(b => b.SpentPercentage).ToList();
        }

    }
}
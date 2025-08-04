using Repositories.ViewModels.FinancialDashboardModel;
using Repositories.ViewModels.ResultModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Services.FinancialDashboardService
{
    public interface IFinancialDashboardService
    {
        /// <summary>
        /// Get complete financial balance data for the authenticated user including budgets, expenses, and account information
        /// </summary>
        /// <returns>Complete balance data with calculated fields</returns>
        Task<ResultModel> GetCompleteBalanceDataAsync();

        /// <summary>
        /// Get budget summary with spent amounts and percentages for the authenticated user
        /// </summary>
        /// <returns>Budget summary with calculated spent amounts</returns>
        Task<ResultModel> GetBudgetSummaryAsync();

        /// <summary>
        /// Get monthly financial summary for a specific month and year for the authenticated user
        /// </summary>
        /// <param name="year">Year</param>
        /// <param name="month">Month (1-12)</param>
        /// <returns>Monthly income, expenses, and transaction summary</returns>
        Task<ResultModel> GetMonthlySummaryAsync(int year, int month);

        /// <summary>
        /// Get budget utilization data for all budgets of the authenticated user
        /// </summary>
        /// <returns>Budget utilization with over-budget alerts</returns>
        Task<ResultModel> GetBudgetUtilizationAsync();
    }
}
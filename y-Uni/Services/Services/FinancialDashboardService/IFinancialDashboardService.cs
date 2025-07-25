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
        /// Get complete financial balance data for a user including budgets, expenses, and account information
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Complete balance data with calculated fields</returns>
        Task<ResultModel> GetCompleteBalanceDataAsync(Guid userId);

        /// <summary>
        /// Get budget summary with spent amounts and percentages for a user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Budget summary with calculated spent amounts</returns>
        Task<ResultModel> GetBudgetSummaryAsync(Guid userId);

        /// <summary>
        /// Get monthly financial summary for a specific month and year
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="year">Year</param>
        /// <param name="month">Month (1-12)</param>
        /// <returns>Monthly income, expenses, and transaction summary</returns>
        Task<ResultModel> GetMonthlySummaryAsync(Guid userId, int year, int month);


        /// <summary>
        /// Get budget utilization data for all user budgets
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Budget utilization with over-budget alerts</returns>
        Task<ResultModel> GetBudgetUtilizationAsync(Guid userId);
    }
}
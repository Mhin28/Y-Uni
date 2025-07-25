using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Services.FinancialDashboardService;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinancialDashboardController : ControllerBase
    {
        private readonly IFinancialDashboardService _financialDashboardService;

        public FinancialDashboardController(IFinancialDashboardService financialDashboardService)
        {
            _financialDashboardService = financialDashboardService;
        }

        /// <summary>
        /// Get complete financial balance data for a user including budgets, expenses, and account information
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Complete balance data with calculated fields</returns>
        [HttpGet("complete-balance/{userId}")]
        public async Task<IActionResult> GetCompleteBalanceData(Guid userId)
        {
            var result = await _financialDashboardService.GetCompleteBalanceDataAsync(userId);
            return StatusCode(result.Code, result);
        }

        /// <summary>
        /// Get budget summary with spent amounts and percentages for a user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Budget summary with calculated spent amounts</returns>
        [HttpGet("budget-summary/{userId}")]
        public async Task<IActionResult> GetBudgetSummary(Guid userId)
        {
            var result = await _financialDashboardService.GetBudgetSummaryAsync(userId);
            return StatusCode(result.Code, result);
        }

        /// <summary>
        /// Get monthly financial summary for a specific month and year
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="year">Year</param>
        /// <param name="month">Month (1-12)</param>
        /// <returns>Monthly income, expenses, and transaction summary</returns>
        [HttpGet("monthly-summary/{userId}/{year}/{month}")]
        public async Task<IActionResult> GetMonthlySummary(Guid userId, int year, int month)
        {
            var result = await _financialDashboardService.GetMonthlySummaryAsync(userId, year, month);
            return StatusCode(result.Code, result);
        }



        /// <summary>
        /// Get budget utilization data for all user budgets
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Budget utilization with over-budget alerts</returns>
        [HttpGet("budget-utilization/{userId}")]
        public async Task<IActionResult> GetBudgetUtilization(Guid userId)
        {
            var result = await _financialDashboardService.GetBudgetUtilizationAsync(userId);
            return StatusCode(result.Code, result);
        }
    }
}
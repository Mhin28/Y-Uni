using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Services.FinancialDashboardService;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FinancialDashboardController : ControllerBase
    {
        private readonly IFinancialDashboardService _financialDashboardService;

        public FinancialDashboardController(IFinancialDashboardService financialDashboardService)
        {
            _financialDashboardService = financialDashboardService;
        }

        /// <summary>
        /// Get complete financial balance data for the authenticated user including budgets, expenses, and account information
        /// </summary>
        /// <returns>Complete balance data with calculated fields</returns>
        [HttpGet("complete-balance")]
        public async Task<IActionResult> GetCompleteBalanceData()
        {
            var result = await _financialDashboardService.GetCompleteBalanceDataAsync();
            return StatusCode(result.Code, result);
        }

        /// <summary>
        /// Get budget summary with spent amounts and percentages for the authenticated user
        /// </summary>
        /// <returns>Budget summary with calculated spent amounts</returns>
        [HttpGet("budget-summary")]
        public async Task<IActionResult> GetBudgetSummary()
        {
            var result = await _financialDashboardService.GetBudgetSummaryAsync();
            return StatusCode(result.Code, result);
        }

        /// <summary>
        /// Get monthly financial summary for a specific month and year for the authenticated user
        /// </summary>
        /// <param name="year">Year</param>
        /// <param name="month">Month (1-12)</param>
        /// <returns>Monthly income, expenses, and transaction summary</returns>
        [HttpGet("monthly-summary/{year}/{month}")]
        public async Task<IActionResult> GetMonthlySummary(int year, int month)
        {
            var result = await _financialDashboardService.GetMonthlySummaryAsync(year, month);
            return StatusCode(result.Code, result);
        }

        /// <summary>
        /// Get budget utilization data for all budgets of the authenticated user
        /// </summary>
        /// <returns>Budget utilization with over-budget alerts</returns>
        [HttpGet("budget-utilization")]
        public async Task<IActionResult> GetBudgetUtilization()
        {
            var result = await _financialDashboardService.GetBudgetUtilizationAsync();
            return StatusCode(result.Code, result);
        }
    }
}
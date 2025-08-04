using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Services.FinancialDashboardService;

namespace API.Controllers
{
    [Route("api/enhanced-financial-dashboard")]
    [ApiController]
    [Authorize]
    public class EnhancedFinancialDashboardController : ControllerBase
    {
        private readonly IFinancialDashboardService _financialDashboardService;

        public EnhancedFinancialDashboardController(IFinancialDashboardService financialDashboardService)
        {
            _financialDashboardService = financialDashboardService;
        }

        /// <summary>
        /// Get complete financial balance data with all calculations done server-side for the authenticated user
        /// This endpoint reduces frontend workload by performing all complex calculations on the backend
        /// </summary>
        /// <returns>Complete balance data with enhanced calculations</returns>
        [HttpGet("complete-balance")]
        public async Task<IActionResult> GetCompleteBalanceData()
        {
            var result = await _financialDashboardService.GetCompleteBalanceDataAsync();
            return StatusCode(result.Code, result);
        }

        /// <summary>
        /// Get budget health analysis with over-budget alerts and utilization percentages for the authenticated user
        /// Implements the budget health logic from the implementation guide
        /// </summary>
        /// <returns>Budget utilization with health status</returns>
        [HttpGet("budget-health")]
        public async Task<IActionResult> GetBudgetHealth()
        {
            var result = await _financialDashboardService.GetBudgetUtilizationAsync();
            return StatusCode(result.Code, result);
        }

        /// <summary>
        /// Get enhanced monthly summary with carry-over calculations for the authenticated user
        /// Implements monthly balance carry-over logic from the guide
        /// </summary>
        /// <param name="year">Year</param>
        /// <param name="month">Month (1-12)</param>
        /// <returns>Enhanced monthly summary with carry-over data</returns>
        [HttpGet("enhanced-monthly-summary/{year}/{month}")]
        public async Task<IActionResult> GetEnhancedMonthlySummary(int year, int month)
        {
            var result = await _financialDashboardService.GetMonthlySummaryAsync(year, month);
            return StatusCode(result.Code, result);
        }

        /// <summary>
        /// Get comprehensive budget summary with spent amounts and percentages for the authenticated user
        /// All calculations performed server-side to reduce frontend complexity
        /// </summary>
        /// <returns>Comprehensive budget summary</returns>
        [HttpGet("comprehensive-budget-summary")]
        public async Task<IActionResult> GetComprehensiveBudgetSummary()
        {
            var result = await _financialDashboardService.GetBudgetSummaryAsync();
            return StatusCode(result.Code, result);
        }


    }
}
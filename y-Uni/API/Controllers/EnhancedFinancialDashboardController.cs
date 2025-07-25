using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Services.FinancialDashboardService;

namespace API.Controllers
{
    [Route("api/enhanced-financial-dashboard")]
    [ApiController]
    public class EnhancedFinancialDashboardController : ControllerBase
    {
        private readonly IFinancialDashboardService _financialDashboardService;

        public EnhancedFinancialDashboardController(IFinancialDashboardService financialDashboardService)
        {
            _financialDashboardService = financialDashboardService;
        }

        /// <summary>
        /// Get complete financial balance data with all calculations done server-side
        /// This endpoint reduces frontend workload by performing all complex calculations on the backend
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Complete balance data with enhanced calculations</returns>
        [HttpGet("complete-balance/{userId}")]
        public async Task<IActionResult> GetCompleteBalanceData(Guid userId)
        {
            var result = await _financialDashboardService.GetCompleteBalanceDataAsync(userId);
            return StatusCode(result.Code, result);
        }

        /// <summary>
        /// Get budget health analysis with over-budget alerts and utilization percentages
        /// Implements the budget health logic from the implementation guide
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Budget utilization with health status</returns>
        [HttpGet("budget-health/{userId}")]
        public async Task<IActionResult> GetBudgetHealth(Guid userId)
        {
            var result = await _financialDashboardService.GetBudgetUtilizationAsync(userId);
            return StatusCode(result.Code, result);
        }

        /// <summary>
        /// Get enhanced monthly summary with carry-over calculations
        /// Implements monthly balance carry-over logic from the guide
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="year">Year</param>
        /// <param name="month">Month (1-12)</param>
        /// <returns>Enhanced monthly summary with carry-over data</returns>
        [HttpGet("enhanced-monthly-summary/{userId}/{year}/{month}")]
        public async Task<IActionResult> GetEnhancedMonthlySummary(Guid userId, int year, int month)
        {
            var result = await _financialDashboardService.GetMonthlySummaryAsync(userId, year, month);
            return StatusCode(result.Code, result);
        }

        /// <summary>
        /// Get comprehensive budget summary with spent amounts and percentages
        /// All calculations performed server-side to reduce frontend complexity
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Comprehensive budget summary</returns>
        [HttpGet("comprehensive-budget-summary/{userId}")]
        public async Task<IActionResult> GetComprehensiveBudgetSummary(Guid userId)
        {
            var result = await _financialDashboardService.GetBudgetSummaryAsync(userId);
            return StatusCode(result.Code, result);
        }


    }
}
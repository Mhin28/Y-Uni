using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repositories.ViewModels.BudgetModel;
using Services.Services.BudgetService;

namespace API.Controllers
{
    [Route("api/budget-lock")]
    [ApiController]
    public class BudgetLockController : ControllerBase
    {
        private readonly IBudgetService _budgetService;

        public BudgetLockController(IBudgetService budgetService)
        {
            _budgetService = budgetService;
        }

        /// <summary>
        /// Get budget carry-over summary between two months
        /// Shows which budgets can be locked and carried over
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="fromYear">Source year</param>
        /// <param name="fromMonth">Source month</param>
        /// <param name="toYear">Target year</param>
        /// <param name="toMonth">Target month</param>
        /// <returns>Budget carry-over summary</returns>
        [HttpGet("summary/{userId}/{fromYear}/{fromMonth}/to/{toYear}/{toMonth}")]
        public async Task<IActionResult> GetBudgetCarryOverSummary(
            Guid userId, 
            int fromYear, 
            int fromMonth, 
            int toYear, 
            int toMonth)
        {
            var result = await _budgetService.GetBudgetCarryOverSummaryAsync(userId, fromYear, fromMonth, toYear, toMonth);
            return StatusCode(result.Code, result);
        }

        /// <summary>
        /// Copy multiple budgets to next month (bulk lock operation)
        /// Frontend can use this when user selects multiple budgets to lock
        /// </summary>
        /// <param name="request">Budget carry-over request</param>
        /// <returns>Result of bulk copy operation</returns>
        [HttpPost("copy-budgets")]
        public async Task<IActionResult> CopyBudgetsToNextMonth([FromBody] BudgetCarryOverRequestDto request)
        {
            var result = await _budgetService.CopyBudgetsToNextMonthAsync(
                request.UserId, 
                request.BudgetIds, 
                request.TargetYear, 
                request.TargetMonth);
            return StatusCode(result.Code, result);
        }

        /// <summary>
        /// Lock a single budget from previous month
        /// Creates new budget with same amount for target month
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="previousBudgetId">Budget ID to copy from</param>
        /// <param name="targetYear">Target year</param>
        /// <param name="targetMonth">Target month</param>
        /// <returns>Created budget</returns>
        [HttpPost("lock-budget/{userId}/{previousBudgetId}/to/{targetYear}/{targetMonth}")]
        public async Task<IActionResult> LockBudgetFromPreviousMonth(
            Guid userId, 
            Guid previousBudgetId, 
            int targetYear, 
            int targetMonth)
        {
            var result = await _budgetService.CreateBudgetFromPreviousMonthAsync(userId, previousBudgetId, targetYear, targetMonth);
            return StatusCode(result.Code, result);
        }

        /// <summary>
        /// Get user budgets for a specific month
        /// Useful for frontend to show available budgets for locking
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="year">Year</param>
        /// <param name="month">Month</param>
        /// <returns>List of budgets for the specified month</returns>
        [HttpGet("budgets/{userId}/{year}/{month}")]
        public async Task<IActionResult> GetUserBudgetsForMonth(Guid userId, int year, int month)
        {
            var result = await _budgetService.GetUserBudgetsForMonthAsync(userId, year, month);
            return StatusCode(result.Code, result);
        }

        /// <summary>
        /// Quick lock all budgets from previous month
        /// Convenience endpoint for "lock all" functionality
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="fromYear">Source year</param>
        /// <param name="fromMonth">Source month</param>
        /// <param name="toYear">Target year</param>
        /// <param name="toMonth">Target month</param>
        /// <returns>Result of bulk lock operation</returns>
        [HttpPost("lock-all/{userId}/{fromYear}/{fromMonth}/to/{toYear}/{toMonth}")]
        public async Task<IActionResult> LockAllBudgetsFromPreviousMonth(
            Guid userId, 
            int fromYear, 
            int fromMonth, 
            int toYear, 
            int toMonth)
        {
            // First get all budgets from source month
            var sourceBudgetsResult = await _budgetService.GetUserBudgetsForMonthAsync(userId, fromYear, fromMonth);
            
            if (!sourceBudgetsResult.IsSuccess)
            {
                return StatusCode(sourceBudgetsResult.Code, sourceBudgetsResult);
            }

            var sourceBudgets = sourceBudgetsResult.Data as List<Repositories.Models.Budget>;
            if (sourceBudgets == null || !sourceBudgets.Any())
            {
                return Ok(new { Message = "No budgets found to lock from source month", LockedCount = 0 });
            }

            var budgetIds = sourceBudgets.Select(b => b.BudgetId).ToList();
            
            // Copy all budgets to target month
            var result = await _budgetService.CopyBudgetsToNextMonthAsync(userId, budgetIds, toYear, toMonth);
            return StatusCode(result.Code, result);
        }
    }
}
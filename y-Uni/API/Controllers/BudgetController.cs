using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Services.BudgetService;
using Repositories.ViewModels.BudgetModel;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BudgetController : ControllerBase
    {
        private readonly IBudgetService _budgetService;
        public BudgetController(IBudgetService budgetService)
        {
            _budgetService = budgetService;
        }
        
        #region CRUD Operations
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            string token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var result = await _budgetService.GetAllAsync(token);
            return StatusCode(result.Code, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            string token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var result = await _budgetService.GetByIdAsync(token, id);
            return StatusCode(result.Code, result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PostBudgetModel model)
        {
            string token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var result = await _budgetService.AddAsync(token, model);
            return StatusCode(result.Code, result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] BudgetModel model)
        {
            string token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var result = await _budgetService.UpdateAsync(token, model);
            return StatusCode(result.Code, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            string token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var result = await _budgetService.DeleteAsync(token, id);
            return StatusCode(result.Code, result);
        }
        #endregion

        #region Budget Lock/Carry-over Operations
        [HttpGet("user/{userId}/month/{year}/{month}")]
        public async Task<IActionResult> GetUserBudgetsForMonth(Guid userId, int year, int month)
        {
            string token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var result = await _budgetService.GetUserBudgetsForMonthAsync(token, userId, year, month);
            return StatusCode(result.Code, result);
        }

        [HttpPost("copy-to-next-month")]
        public async Task<IActionResult> CopyBudgetsToNextMonth([FromBody] BudgetCarryOverRequestDto request)
        {
            string token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var result = await _budgetService.CopyBudgetsToNextMonthAsync(token, request.UserId, request.BudgetIds, request.TargetYear, request.TargetMonth);
            return StatusCode(result.Code, result);
        }

        [HttpPost("create-from-previous")]
        public async Task<IActionResult> CreateBudgetFromPreviousMonth([FromBody] CreateBudgetFromPreviousRequest request)
        {
            string token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var result = await _budgetService.CreateBudgetFromPreviousMonthAsync(token, request.UserId, request.PreviousBudgetId, request.TargetYear, request.TargetMonth);
            return StatusCode(result.Code, result);
        }

        [HttpGet("carry-over-summary")]
        public async Task<IActionResult> GetBudgetCarryOverSummary([FromQuery] Guid userId, [FromQuery] int fromYear, [FromQuery] int fromMonth, [FromQuery] int toYear, [FromQuery] int toMonth)
        {
            string token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var result = await _budgetService.GetBudgetCarryOverSummaryAsync(token, userId, fromYear, fromMonth, toYear, toMonth);
            return StatusCode(result.Code, result);
        }
        #endregion
    }

    // Helper request models for the new endpoints
    public class CreateBudgetFromPreviousRequest
    {
        public Guid UserId { get; set; }
        public Guid PreviousBudgetId { get; set; }
        public int TargetYear { get; set; }
        public int TargetMonth { get; set; }
    }
} 
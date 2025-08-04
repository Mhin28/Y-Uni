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
            var result = await _budgetService.GetAllAsync();
            return StatusCode(result.Code, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _budgetService.GetByIdAsync(id);
            return StatusCode(result.Code, result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PostBudgetModel model)
        {
            var result = await _budgetService.AddAsync(model);
            return StatusCode(result.Code, result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] BudgetModel model)
        {
            var result = await _budgetService.UpdateAsync(model);
            return StatusCode(result.Code, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _budgetService.DeleteAsync(id);
            return StatusCode(result.Code, result);
        }
        #endregion

        #region Budget Lock/Carry-over Operations
        [HttpGet("month/{year}/{month}")]
        public async Task<IActionResult> GetUserBudgetsForMonth(int year, int month)
        {
            var result = await _budgetService.GetUserBudgetsForMonthAsync(year, month);
            return StatusCode(result.Code, result);
        }

        [HttpPost("copy-to-next-month")]
        public async Task<IActionResult> CopyBudgetsToNextMonth([FromBody] BudgetCarryOverRequestDto request)
        {
            var result = await _budgetService.CopyBudgetsToNextMonthAsync(request.BudgetIds, request.TargetYear, request.TargetMonth);
            return StatusCode(result.Code, result);
        }

        [HttpPost("create-from-previous")]
        public async Task<IActionResult> CreateBudgetFromPreviousMonth([FromBody] CreateBudgetFromPreviousRequest request)
        {
            var result = await _budgetService.CreateBudgetFromPreviousMonthAsync(request.PreviousBudgetId, request.TargetYear, request.TargetMonth);
            return StatusCode(result.Code, result);
        }

        [HttpGet("carry-over-summary")]
        public async Task<IActionResult> GetBudgetCarryOverSummary([FromQuery] int fromYear, [FromQuery] int fromMonth, [FromQuery] int toYear, [FromQuery] int toMonth)
        {
            var result = await _budgetService.GetBudgetCarryOverSummaryAsync(fromYear, fromMonth, toYear, toMonth);
            return StatusCode(result.Code, result);
        }
        #endregion
    }

    // Helper request models for the new endpoints
    public class CreateBudgetFromPreviousRequest
    {
        public Guid PreviousBudgetId { get; set; }
        public int TargetYear { get; set; }
        public int TargetMonth { get; set; }
    }
} 
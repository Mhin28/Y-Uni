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
        [HttpGet("my-budgets")]
        public async Task<IActionResult> GetMyBudgets([FromQuery] Guid? categoryId, [FromQuery] DateOnly? from, [FromQuery] DateOnly? to)
        {
            string token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var result = await _budgetService.GetBudgetsByUserAsync(token, categoryId, from, to);
            return StatusCode(result.Code, result);
        }
        [HttpPost]
        public async Task<IActionResult> CreateBudget([FromBody] PostBudgetModel model)
        {
            string token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var result = await _budgetService.CreateBudgetAsync(token, model);
            return StatusCode(result.Code, result);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBudget(Guid id, [FromBody] UpdateBudgetModel model)
        {
            string token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var result = await _budgetService.UpdateBudgetAsync(token, id, model);
            return StatusCode(result.Code, result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBudget(Guid id)
        {
            string token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var result = await _budgetService.DeleteBudgetAsync(token, id);
            return StatusCode(result.Code, result);
        }
    }
} 
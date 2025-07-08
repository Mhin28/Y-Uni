using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repositories.ViewModels.BudgetModel;
using Services.Services.BudgetService;

namespace API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
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
	}
}
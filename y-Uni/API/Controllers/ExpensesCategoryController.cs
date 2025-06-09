using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repositories.ViewModels.ExpensesCategoryModel;
using Services.Services.ExpensesCategoryService;

namespace API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ExpensesCategoryController : ControllerBase
	{
		private readonly IExpensesCategoryService _expensesCategoryService;
		public ExpensesCategoryController(IExpensesCategoryService expensesCategoryService)
		{
			_expensesCategoryService = expensesCategoryService;
		}

		#region CRUD Operations
		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var result = await _expensesCategoryService.GetAllAsync();
			return StatusCode(result.Code, result);
		}
		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(Guid id)
		{
			var result = await _expensesCategoryService.GetByIdAsync(id);
			return StatusCode(result.Code, result);
		}
		[HttpPost]
		public async Task<IActionResult> Create([FromBody] PostExpensesCategoryModel model)
		{
			var result = await _expensesCategoryService.AddAsync(model);
			return StatusCode(result.Code, result);
		}
		[HttpPut]
		public async Task<IActionResult> Update([FromBody] ExpensesCategoryModel model)
		{
			var result = await _expensesCategoryService.UpdateAsync(model);
			return StatusCode(result.Code, result);
		}
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(Guid id)
		{
			var result = await _expensesCategoryService.DeleteAsync(id);
			return StatusCode(result.Code, result);
		}
		#endregion
	}
}

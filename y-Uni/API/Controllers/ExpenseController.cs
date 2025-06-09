using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repositories.ViewModels.ExpenseModel;
using Services.Services.ExpenseService;

namespace API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ExpenseController : ControllerBase
	{
		private readonly IExpenseService _expenseService;
		public ExpenseController(IExpenseService expenseService)
		{
			_expenseService = expenseService;
		}
		#region CRUD Operations
		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var result = await _expenseService.GetAllAsync();
			return StatusCode(result.Code, result);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(Guid id)
		{
			var result = await _expenseService.GetByIdAsync(id);
			return StatusCode(result.Code, result);
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] PostExpenseModel model)
		{
			var result = await _expenseService.AddAsync(model);
			return StatusCode(result.Code, result);
		}

		[HttpPut]
		public async Task<IActionResult> Update([FromBody] ExpenseModel model)
		{
			var result = await _expenseService.UpdateAsync(model);
			return StatusCode(result.Code, result);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(Guid id)
		{
			var result = await _expenseService.DeleteAsync(id);
			return StatusCode(result.Code, result);
		}
		#endregion
	}
}

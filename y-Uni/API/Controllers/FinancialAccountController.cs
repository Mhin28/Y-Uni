using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repositories.ViewModels.FinancialAccountModel;
using Services.Services.FinancialAccountService;

namespace API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class FinancialAccountController : ControllerBase
	{
		private readonly IFinancialAccountService _financialAccountService;
		public FinancialAccountController(IFinancialAccountService financialAccountService)
		{
			_financialAccountService = financialAccountService;
		}

		#region CRUD Operations
		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var result = await _financialAccountService.GetAllAsync();
			return StatusCode(result.Code, result);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(Guid id)
		{
			var result = await _financialAccountService.GetByIdAsync(id);
			return StatusCode(result.Code, result);
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] PostFinancialAccModel model)
		{
			var result = await _financialAccountService.AddAsync(model);
			return StatusCode(result.Code, result);
		}

		[HttpPut]
		public async Task<IActionResult> Update([FromBody] FinancialAccModel model)
		{
			var result = await _financialAccountService.UpdateAsync(model);
			return StatusCode(result.Code, result);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(Guid id)
		{
			var result = await _financialAccountService.DeleteAsync(id);
			return StatusCode(result.Code, result);
		}
		#endregion
	}
}

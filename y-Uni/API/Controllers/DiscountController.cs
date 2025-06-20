using Microsoft.AspNetCore.Mvc;
using Repositories.ViewModels.DiscountModel;
using Services.Services.DiscountService;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class DiscountController : ControllerBase
	{
		private readonly IDiscountService _discountService;

		public DiscountController(IDiscountService discountService)
		{
			_discountService = discountService;
		}

		#region CRUD Operations

		// GET: api/Discount
		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var result = await _discountService.GetAllAsync();
			return StatusCode(result.Code, result);
		}

		// GET: api/Discount/{id}
		[HttpGet("{id:guid}")]
		public async Task<IActionResult> GetById(Guid id)
		{
			var result = await _discountService.GetByIdAsync(id);
			return StatusCode(result.Code, result);
		}

		// GET: api/Discount/active
		[HttpGet("active")]
		public async Task<IActionResult> GetActiveDiscounts()
		{
			var result = await _discountService.GetActiveAsync();
			return StatusCode(result.Code, result);
		}

		// GET: api/Discount/search/{name}
		[HttpGet("search/{name}")]
		public async Task<IActionResult> GetByName(string name)
		{
			var result = await _discountService.GetByNameAsync(name);
			return StatusCode(result.Code, result);
		}

		// POST: api/Discount
		[HttpPost]
		public async Task<IActionResult> Create([FromBody] PostDiscountModel model)
		{
			var result = await _discountService.AddAsync(model);
			return StatusCode(result.Code, result);
		}

		// PUT: api/Discount
		[HttpPut]
		public async Task<IActionResult> Update([FromBody] DiscountModel model)
		{
			var result = await _discountService.UpdateAsync(model);
			return StatusCode(result.Code, result);
		}

		// DELETE: api/Discount/{id}
		[HttpDelete("{id:guid}")]
		public async Task<IActionResult> Delete(Guid id)
		{
			var result = await _discountService.DeleteAsync(id);
			return StatusCode(result.Code, result);
		}

		#endregion
	}
}

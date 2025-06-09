using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repositories.ViewModels.PaymentMethodModel;
using Services.Services.PaymentMethodService;

namespace API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PaymentMethodController : ControllerBase
	{
		private readonly IPaymentMethodService _paymentMethodService;
		public PaymentMethodController(IPaymentMethodService paymentMethodService)
		{
			_paymentMethodService = paymentMethodService;
		}

		#region CRUD Operations
		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var result = await _paymentMethodService.GetAllAsync();
			return StatusCode(result.Code, result);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(Guid id)
		{
			var result = await _paymentMethodService.GetByIdAsync(id);
			return StatusCode(result.Code, result);
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] PostPaymentMethodModel model)
		{
			var result = await _paymentMethodService.AddAsync(model);
			return StatusCode(result.Code, result);
		}

		[HttpPut]
		public async Task<IActionResult> Update([FromBody] PaymentMethodModel model)
		{
			var result = await _paymentMethodService.UpdateAsync(model);
			return StatusCode(result.Code, result);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(Guid id)
		{
			var result = await _paymentMethodService.DeleteAsync(id);
			return StatusCode(result.Code, result);
		}
		#endregion
	}
}

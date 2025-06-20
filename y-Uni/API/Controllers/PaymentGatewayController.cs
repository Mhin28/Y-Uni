using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repositories.ViewModels.PaymentGatewayModel;
using Services.Services.PaymentGatewayService;

namespace API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PaymentGatewayController : ControllerBase
	{
		private readonly IPaymentGatewayService _paymentGatewayService;

		public PaymentGatewayController(IPaymentGatewayService paymentGatewayService)
		{
			_paymentGatewayService = paymentGatewayService;
		}

		#region CRUD Operations

		// GET: api/PaymentGateway
		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var result = await _paymentGatewayService.GetAllAsync();
			return StatusCode(result.Code, result);
		}

		// GET: api/PaymentGateway/{id}
		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(Guid id)
		{
			var result = await _paymentGatewayService.GetByIdAsync(id);
			return StatusCode(result.Code, result);
		}

		// POST: api/PaymentGateway
		[HttpPost]
		public async Task<IActionResult> Create([FromBody] PostPaymentGatewayModel model)
		{
			var result = await _paymentGatewayService.AddAsync(model);
			return StatusCode(result.Code, result);
		}

		// PUT: api/PaymentGateway
		[HttpPut]
		public async Task<IActionResult> Update([FromBody] PaymentGatewayModel model)
		{
			var result = await _paymentGatewayService.UpdateAsync(model);
			return StatusCode(result.Code, result);
		}

		// DELETE: api/PaymentGateway/{id}
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(Guid id)
		{
			var result = await _paymentGatewayService.DeleteAsync(id);
			return StatusCode(result.Code, result);
		}

		#endregion
	}
}

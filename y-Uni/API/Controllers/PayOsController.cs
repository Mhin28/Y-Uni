using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Net.payOS;
using Net.payOS.Types;
using Repositories.ViewModels.PayOsModel;
using Services.Services.PayOsService;

namespace API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PayOsController : ControllerBase
	{
		private readonly IPayOsService _payOsService;
		private readonly PayOS _payOS;
		public PayOsController(IPayOsService payOsService, PayOS payOS)
		{
			_payOsService = payOsService;
			_payOS = payOS;
		}

		#region PayOS Integration
		/// <summary>
		/// Tạo invoice mới và link thanh toán PayOS cùng lúc
		/// </summary>
		[HttpPost("create-payment-link")]
		public async Task<IActionResult> CreatePaymentLink([FromBody] CreatePaymentLinkRequestModel model)
		{
			var result = await _payOsService.CreatePaymentLinkAsync(model);
			return StatusCode(result.Code, result);
		}

		/// <summary>
		/// Lấy thông tin thanh toán PayOS của invoice
		/// </summary>
		[HttpGet("{id:guid}/payment-info")]
		public async Task<IActionResult> GetPaymentInfo(Guid id)
		{
			var result = await _payOsService.GetPaymentInfoAsync(id);
			return StatusCode(result.Code, result);
		}

		/// <summary>
		/// Hủy thanh toán PayOS của invoice
		/// </summary>
		[HttpPut("{id:guid}/cancel-payment")]
		public async Task<IActionResult> CancelPayment(Guid id)
		{
			var result = await _payOsService.CancelPaymentAsync(id);
			return StatusCode(result.Code, result);
		}

		///// <summary>
		///// Webhook để nhận thông báo từ PayOS khi thanh toán thay đổi trạng thái
		///// </summary>
		//[HttpPost("webhook-event-handler")]
		//[AllowAnonymous]
		//public async Task<IActionResult> HandlePayOSWebhook([FromBody] WebhookType body)
		//{
		//	try
		//	{
		//		WebhookData data = _payOS.verifyPaymentWebhookData(body);
		//		var result = await _payOsService.HandleWebhookAsync(body);
		//		return Ok();
		//	}
		//	catch (Exception ex)
		//	{
		//		// Log the exception
		//		//_logger.LogError(ex, "Error processing PayOS webhook");
		//		return StatusCode(500, new { message = "Internal server error" });
		//	}
		//}
		#endregion
	}
}

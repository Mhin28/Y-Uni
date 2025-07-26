using Microsoft.AspNetCore.Mvc;
using Net.payOS;
using Net.payOS.Types;
using Services.Services.PayOsService;

namespace API.Controllers
{
	public class CheckoutController : Controller
	{
		private readonly PayOS _payOS;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IPayOsService _payOsService;


		public CheckoutController(PayOS payOS, IHttpContextAccessor httpContextAccessor, IPayOsService payOsService)
		{
			_payOS = payOS;
			_httpContextAccessor = httpContextAccessor;
			_payOsService = payOsService;
		}

		[HttpGet("/")]
		public IActionResult Index()
		{
			// Trả về trang HTML có tên "MyView.cshtml"
			return View("index");
		}
		[HttpGet("/cancel")]
		public async Task<IActionResult> Cancel()
		{
			// Extract query parameters from the URL
			var code = HttpContext.Request.Query["code"].ToString();
			var status = HttpContext.Request.Query["status"].ToString();
			var orderCode = string.IsNullOrEmpty(HttpContext.Request.Query["orderCode"]) ? 0 : long.Parse(HttpContext.Request.Query["orderCode"]);
			var id = HttpContext.Request.Query["id"].ToString();

			try
			{
				// Validate query parameters
				if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(status) || orderCode == 0)
				{
					return View("error", new { message = "Invalid payment data." });
				}

				// Construct WebhookData with all required fields
				var webhookData = new WebhookData(
					orderCode: orderCode,
					amount: 0, // Amount not provided in query, fetch from invoice or PayOS API if needed
					description: "Payment via PayOS", // Default description, adjust as needed
					accountNumber: "", // Not provided in query, set to empty or fetch from context
					reference: id,
					transactionDateTime: DateTime.UtcNow.AddHours(7).ToString("yyyy-MM-dd HH:mm:ss"), // 06:11 PM, July 26, 2025
					currency: "VND",
					paymentLinkId: id,
					code: code,
					desc: status == "PAID" ? "PAID" : "CANCELLED",
					counterAccountBankId: null, // Not in query, set to null or fetch if available
					counterAccountBankName: null,
					counterAccountName: null,
					counterAccountNumber: null,
					virtualAccountName: null, // Not in query, set to null or fetch if available
					virtualAccountNumber: null
				);

				// Assume success based on code '00' (PayOS success code)
				bool success = code == "00";

				// Signature should be validated (placeholder, implement actual validation with PayOS key)
				string signature = ""; // Check headers for signature

				// Create WebhookType object
				var webhookType = new WebhookType(
					code: code,
					desc: status == "PAID" ? "PAID" : "CANCELLED",
					success: success,
					data: webhookData,
					signature: signature
				);

				// Call the service to update invoice status
				var result = await _payOsService.HandleWebhookAsync(webhookType);

				if (!result.IsSuccess)
				{
					return View("error", new { message = "Failed to update invoice status." });
				}
			}
			catch (Exception ex)
			{
				return View("error", new { message = $"Error processing payment: {ex.Message}" });
			}
			// Trả về trang HTML có tên "MyView.cshtml"
			return View("cancel");
		}
		//[HttpGet("/success")]
		//public IActionResult Success()
		//{
		//	// Trả về trang HTML có tên "MyView.cshtml"
		//	return View("success");
		//}
		[HttpGet("/success")]
		public async Task<IActionResult> Success()
		{
			// Extract query parameters from the URL
			var code = HttpContext.Request.Query["code"].ToString();
			var status = HttpContext.Request.Query["status"].ToString();
			var orderCode = string.IsNullOrEmpty(HttpContext.Request.Query["orderCode"]) ? 0 : long.Parse(HttpContext.Request.Query["orderCode"]);
			var id = HttpContext.Request.Query["id"].ToString();

			try
			{
				// Validate query parameters
				if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(status) || orderCode == 0)
				{
					return View("error", new { message = "Invalid payment data." });
				}

				// Construct WebhookData with all required fields
				var webhookData = new WebhookData(
					orderCode: orderCode,
					amount: 0, // Amount not provided in query, fetch from invoice or PayOS API if needed
					description: "Payment via PayOS", // Default description, adjust as needed
					accountNumber: "", // Not provided in query, set to empty or fetch from context
					reference: id,
					transactionDateTime: DateTime.UtcNow.AddHours(7).ToString("yyyy-MM-dd HH:mm:ss"), // 06:11 PM, July 26, 2025
					currency: "VND",
					paymentLinkId: id,
					code: code,
					desc: status == "PAID" ? "PAID" : "CANCELLED",
					counterAccountBankId: null, // Not in query, set to null or fetch if available
					counterAccountBankName: null,
					counterAccountName: null,
					counterAccountNumber: null,
					virtualAccountName: null, // Not in query, set to null or fetch if available
					virtualAccountNumber: null
				);

				// Assume success based on code '00' (PayOS success code)
				bool success = code == "00";

				// Signature should be validated (placeholder, implement actual validation with PayOS key)
				string signature = ""; // Check headers for signature

				// Create WebhookType object
				var webhookType = new WebhookType(
					code: code,
					desc: status == "PAID" ? "PAID" : "CANCELLED",
					success: success,
					data: webhookData,
					signature: signature
				);

				// Call the service to update invoice status
				var result = await _payOsService.HandleWebhookAsync(webhookType);

				if (!result.IsSuccess)
				{
					return View("error", new { message = "Failed to update invoice status." });
				}
			}
			catch (Exception ex)
			{
				return View("error", new { message = $"Error processing payment: {ex.Message}" });
			}

			return View("success");
		}
	}
}

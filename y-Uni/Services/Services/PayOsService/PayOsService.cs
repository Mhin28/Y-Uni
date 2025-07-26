using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Net.payOS;
using Net.payOS.Types;
using Repositories.Models;
using Repositories.Repositories;
using Repositories.ViewModels.PayOsModel;
using Repositories.ViewModels.ResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.PayOsService
{
	public class PayOsService : IPayOsService
	{
		private readonly IInvoiceRepo _repo;
		private readonly PayOS _payOS;
		private readonly IConfiguration _configuration;
		private readonly ILogger<PayOsService> _logger;


		public PayOsService(IInvoiceRepo repo, PayOS payOS, IConfiguration configuration, ILogger<PayOsService> logger)
		{
			_repo = repo;
			_payOS = payOS;
			_configuration = configuration;
			_logger = logger;
		}

		public async Task<ResultModel> CreatePaymentLinkAsync(CreatePaymentLinkRequestModel model)
		{
			var result = new ResultModel();
			try
			{
				// Tạo invoice mới
				var invoice = new Invoice
				{
					InvoiceId = Guid.NewGuid(),
					UserId = model.UserId,
					DiscountId = model.DiscountId,
					PaymentMethodId = model.PaymentMethodId,
					MembershipPlanId = model.MembershipPlanId,
					Amount = model.Amount,
					TaxAmount = model.TaxAmount,
					DiscountAmount = model.DiscountAmount,
					TotalAmount = model.TotalAmount,
					InvoiceStatus = "PENDING",
					CreatedDate = DateTime.UtcNow.AddHours(7)
				};

				// Tạo order code unique từ invoice ID và timestamp
				long orderCode = Math.Abs((long)invoice.InvoiceId.GetHashCode() + DateTimeOffset.Now.ToUnixTimeSeconds());

				// Tạo item data
				ItemData item = new ItemData(model.ProductName, 1, (int)invoice.TotalAmount.GetValueOrDefault(invoice.Amount));
				List<ItemData> items = new List<ItemData> { item };

				// Tạo payment data
				PaymentData paymentData = new PaymentData(
					orderCode,
					(int)invoice.TotalAmount.GetValueOrDefault(invoice.Amount),
					model.Description,
					items,
					model.CancelUrl,
					model.ReturnUrl
				);

				// Tạo payment link với PayOS
				CreatePaymentResult createPayment = await _payOS.createPaymentLink(paymentData);

				// Cập nhật invoice với thông tin PayOS (sử dụng trường có sẵn)
				// GatewayTransactionId: Lưu order code của PayOS
				invoice.GatewayTransactionId = $"PAYOS_{orderCode}_{createPayment.paymentLinkId}";

				// Lưu invoice vào database
				await _repo.CreateAsync(invoice);

				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.Created;
				result.Data = new PaymentLinkResponseModel
				{
					PaymentUrl = createPayment.checkoutUrl,
					PaymentLinkId = createPayment.paymentLinkId,
					OrderCode = orderCode,
					QrCode = createPayment.qrCode,
					InvoiceId = invoice.InvoiceId
				};
				result.Message = "Invoice and payment link created successfully.";
			}
			catch (Exception ex)
			{
				result.IsSuccess = false;
				result.Code = (int)HttpStatusCode.InternalServerError;
				result.Message = ex.Message;
			}
			return result;
		}

		public async Task<ResultModel> GetPaymentInfoAsync(Guid invoiceId)
		{
			var result = new ResultModel();
			try
			{
				var invoice = await _repo.GetByIdAsync(invoiceId);
				if (invoice == null)
				{
					result.IsSuccess = false;
					result.Code = (int)HttpStatusCode.NotFound;
					result.Message = "Invoice not found.";
					return result;
				}

				// Lấy order code từ GatewayTransactionId
				if (string.IsNullOrEmpty(invoice.GatewayTransactionId) || !invoice.GatewayTransactionId.StartsWith("PAYOS_"))
				{
					result.IsSuccess = false;
					result.Code = (int)HttpStatusCode.BadRequest;
					result.Message = "PayOS payment not found for this invoice.";
					return result;
				}

				// Parse order code từ GatewayTransactionId format: "PAYOS_{orderCode}_{paymentLinkId}"
				var parts = invoice.GatewayTransactionId.Split('_');
				if (parts.Length < 3 || !long.TryParse(parts[1], out long orderCode))
				{
					result.IsSuccess = false;
					result.Code = (int)HttpStatusCode.BadRequest;
					result.Message = "Invalid PayOS order code format.";
					return result;
				}

				PaymentLinkInformation paymentInfo = await _payOS.getPaymentLinkInformation(orderCode);

				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.OK;
				result.Data = new
				{
					PaymentInfo = paymentInfo,
					Invoice = invoice
				};
			}
			catch (Exception ex)
			{
				result.IsSuccess = false;
				result.Code = (int)HttpStatusCode.InternalServerError;
				result.Message = ex.Message;
			}
			return result;
		}

		public async Task<ResultModel> CancelPaymentAsync(Guid invoiceId)
		{
			var result = new ResultModel();
			try
			{
				var invoice = await _repo.GetByIdAsync(invoiceId);
				if (invoice == null)
				{
					result.IsSuccess = false;
					result.Code = (int)HttpStatusCode.NotFound;
					result.Message = "Invoice not found.";
					return result;
				}

				// Lấy order code từ GatewayTransactionId
				if (string.IsNullOrEmpty(invoice.GatewayTransactionId) || !invoice.GatewayTransactionId.StartsWith("PAYOS_"))
				{
					result.IsSuccess = false;
					result.Code = (int)HttpStatusCode.BadRequest;
					result.Message = "PayOS payment not found for this invoice.";
					return result;
				}

				var parts = invoice.GatewayTransactionId.Split('_');
				if (parts.Length < 3 || !long.TryParse(parts[1], out long orderCode))
				{
					result.IsSuccess = false;
					result.Code = (int)HttpStatusCode.BadRequest;
					result.Message = "Invalid PayOS order code format.";
					return result;
				}

				PaymentLinkInformation cancelledPayment = await _payOS.cancelPaymentLink(orderCode);

				// Cập nhật trạng thái invoice
				invoice.InvoiceStatus = "CANCELLED";
				invoice.UpdatedDate = DateTime.UtcNow;

				await _repo.UpdateAsync(invoice);

				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.OK;
				result.Data = cancelledPayment;
				result.Message = "Payment cancelled successfully.";
			}
			catch (Exception ex)
			{
				result.IsSuccess = false;
				result.Code = (int)HttpStatusCode.InternalServerError;
				result.Message = ex.Message;
			}
			return result;
		}

		public async Task<ResultModel> HandleWebhookAsync(WebhookType body)
		{
			var result = new ResultModel();
			try
			{
				_logger.LogInformation($"Processing PayOS webhook - OrderCode: {body.data.orderCode}, Code: {body.code}");

				// Validate webhook data
				if (body?.data?.orderCode == null)
				{
					result.IsSuccess = false;
					result.Code = (int)HttpStatusCode.BadRequest;
					result.Message = "Invalid webhook data: missing order code";
					return result;
				}

				// Tìm invoice theo order code trong GatewayTransactionId
				var invoices = await _repo.GetAllAsync();
				var invoice = invoices.FirstOrDefault(i =>
					!string.IsNullOrEmpty(i.GatewayTransactionId) &&
					i.GatewayTransactionId.Contains($"PAYOS_{body.data.orderCode}_"));

				if (invoice == null)
				{
					_logger.LogWarning($"Invoice not found for PayOS OrderCode: {body.data.orderCode}");
					result.IsSuccess = false;
					result.Code = (int)HttpStatusCode.NotFound;
					result.Message = $"Invoice not found for PayOS OrderCode: {body.data.orderCode}";
					return result;
				}

				// Lưu trạng thái cũ để log
				var oldStatus = invoice.InvoiceStatus;

				// Cập nhật trạng thái dựa trên webhook
				switch (body.code)
				{
					case "00": 
						if (body.desc == "CANCELLED") invoice.InvoiceStatus = "UNPAID";
						else if (body.desc == "PAID") invoice.InvoiceStatus = "PAID";
						// Cập nhật GatewayTransactionId với reference từ PayOS
						if (!string.IsNullOrEmpty(body.data.reference))
						{
							invoice.GatewayTransactionId = $"{invoice.GatewayTransactionId}_REF_{body.data.reference}";
						}
						_logger.LogInformation($"Payment for Invoice {invoice.InvoiceId}, Reference: {body.data.reference}");
						break;

					case "01": // Thất bại
						invoice.InvoiceStatus = "FAILED";
						_logger.LogWarning($"Payment failed for Invoice {invoice.InvoiceId}");
						break;

					case "02": // Hủy
						invoice.InvoiceStatus = "CANCELLED";
						_logger.LogInformation($"Payment cancelled for Invoice {invoice.InvoiceId}");
						break;

					default:
						invoice.InvoiceStatus = "PROCESSING";
						_logger.LogInformation($"Payment processing for Invoice {invoice.InvoiceId}, Code: {body.code}");
						break;
				}

				invoice.UpdatedDate = DateTime.UtcNow;
				await _repo.UpdateAsync(invoice);

				_logger.LogInformation($"Invoice {invoice.InvoiceId} status updated: {oldStatus} -> {invoice.InvoiceStatus}");

				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.OK;
				result.Data = new
				{
					InvoiceId = invoice.InvoiceId,
					OldStatus = oldStatus,
					NewStatus = invoice.InvoiceStatus,
					PayOSOrderCode = body.data.orderCode,
					Reference = body.data.reference,
					Amount = body.data.amount,
					TransactionDateTime = body.data.transactionDateTime
				};
				result.Message = "Webhook processed successfully.";
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error processing webhook for OrderCode: {body?.data?.orderCode}");
				result.IsSuccess = false;
				result.Code = (int)HttpStatusCode.InternalServerError;
				result.Message = ex.Message;
			}
			return result;
		}
	}
}

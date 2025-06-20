using Repositories.Models;
using Repositories.Repositories;
using Repositories.ViewModels.InvoiceModel;
using Repositories.ViewModels.ResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.InvoiceService
{
	public class InvoiceService : IInvoiceService
	{
		private readonly IInvoiceRepo _repo;
		public InvoiceService(IInvoiceRepo repo)
		{
			_repo = repo;
		}

		public async Task<ResultModel> GetAllAsync()
		{
			var result = new ResultModel();
			try
			{
				var invoices = await _repo.GetAllAsync(i => i.User, i => i.Discount, i => i.PaymentMethod, i => i.MembershipPlan);
				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.OK;
				result.Data = invoices;
			}
			catch (Exception ex)
			{
				result.IsSuccess = false;
				result.Code = (int)HttpStatusCode.InternalServerError;
				result.Message = ex.Message;
			}
			return result;
		}

		public async Task<ResultModel> GetByIdAsync(Guid id)
		{
			var result = new ResultModel();
			try
			{
				var invoice = await _repo.GetByIdAsync(id);
				if (invoice == null)
				{
					result.IsSuccess = false;
					result.Code = (int)HttpStatusCode.NotFound;
					result.Message = "Invoice not found.";
					return result;
				}
				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.OK;
				result.Data = invoice;
			}
			catch (Exception ex)
			{
				result.IsSuccess = false;
				result.Code = (int)HttpStatusCode.InternalServerError;
				result.Message = ex.Message;
			}
			return result;
		}

		public async Task<ResultModel> AddAsync(PostInvoiceModel model)
		{
			var result = new ResultModel()
			{
				IsSuccess = false,
				Code = (int)HttpStatusCode.BadRequest,
				Message = "Invalid invoice data."
			};
			try
			{
				var invoice = new Invoice
				{
					InvoiceId = Guid.NewGuid(),
					UserId = model.UserId,
					DiscountId = model.DiscountId,
					PaymentMethodId = model.PaymentMethodId,
					MembershipPlanId = model.MembershipPlanId,
					Amount = model.Amount,
					InvoiceStatus = model.InvoiceStatus,
					CreatedDate = DateTime.UtcNow,
					TaxAmount = model.TaxAmount,
					DiscountAmount = model.DiscountAmount,
					GatewayTransactionId = model.GatewayTransactionId,
					TotalAmount = model.TotalAmount
				};
				await _repo.CreateAsync(invoice);
				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.Created;
				result.Data = invoice;
				result.Message = "Invoice created successfully.";
			}
			catch (Exception ex)
			{
				result.IsSuccess = false;
				result.Code = (int)HttpStatusCode.InternalServerError;
				result.Message = ex.Message;
			}
			return result;
		}

		public async Task<ResultModel> UpdateAsync(InvoiceModel model)
		{
			var result = new ResultModel()
			{
				IsSuccess = false,
				Code = (int)HttpStatusCode.BadRequest,
				Message = "Invalid invoice data."
			};
			try
			{
				var invoice = await _repo.GetByIdAsync(model.InvoiceId);
				if (invoice == null)
				{
					result.IsSuccess = false;
					result.Code = (int)HttpStatusCode.NotFound;
					result.Message = "Invoice not found.";
					return result;
				}
				if (model.Amount.HasValue)
					invoice.Amount = model.Amount.Value;

				if (model.TaxAmount.HasValue)
					invoice.TaxAmount = model.TaxAmount.Value;

				if (model.DiscountAmount.HasValue)
					invoice.DiscountAmount = model.DiscountAmount.Value;

				if (model.TotalAmount.HasValue)
					invoice.TotalAmount = model.TotalAmount.Value;

				if (model.PaymentMethodId.HasValue)
					invoice.PaymentMethodId = model.PaymentMethodId.Value;

				if (model.GatewayTransactionId != null)
					invoice.GatewayTransactionId = model.GatewayTransactionId;

				if (model.InvoiceStatus != null)
					invoice.InvoiceStatus = model.InvoiceStatus;

				if (model.UserId.HasValue)
					invoice.UserId = model.UserId.Value;

				if (model.DiscountId.HasValue)
					invoice.DiscountId = model.DiscountId.Value;

				if (model.MembershipPlanId.HasValue)
					invoice.MembershipPlanId = model.MembershipPlanId.Value;

				invoice.UpdatedDate = DateTime.UtcNow;


				await _repo.UpdateAsync(invoice);
				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.OK;
				result.Data = invoice;
				result.Message = "Invoice updated successfully.";
			}
			catch (Exception ex)
			{
				result.IsSuccess = false;
				result.Code = (int)HttpStatusCode.InternalServerError;
				result.Message = ex.Message;
			}
			return result;
		}

		public async Task<ResultModel> DeleteAsync(Guid id)
		{
			var result = new ResultModel();
			try
			{
				var invoice = await _repo.GetByIdAsync(id);
				if (invoice == null)
				{
					result.IsSuccess = false;
					result.Code = (int)HttpStatusCode.NotFound;
					result.Message = "Invoice not found.";
					return result;
				}
				await _repo.RemoveAsync(invoice);
				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.OK;
				result.Message = "Invoice deleted successfully.";
			}
			catch (Exception ex)
			{
				result.IsSuccess = false;
				result.Code = (int)HttpStatusCode.InternalServerError;
				result.Message = ex.Message;
			}
			return result;
		}

		public async Task<ResultModel> GetInvoicesByUserIdAsync(Guid userId)
		{
			var result = new ResultModel();
			try
			{
				var invoices = await _repo.GetInvoicesByUserIdAsync(userId);
				if (invoices == null || !invoices.Any())
				{
					result.IsSuccess = false;
					result.Code = (int)HttpStatusCode.NotFound;
					result.Message = "No invoices found for the specified user.";
					return result;
				}
				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.OK;
				result.Data = invoices;
			}
			catch (Exception ex)
			{
				result.IsSuccess = false;
				result.Code = (int)HttpStatusCode.InternalServerError;
				result.Message = ex.Message;
			}
			return result;
		}
		public async Task<ResultModel> GetInvoicesByMembershipPlanAsync(Guid membershipPlanId)
		{
			var result = new ResultModel();
			try
			{
				var invoices = await _repo.GetInvoicesByMembershipPlanAsync(membershipPlanId);
				if (invoices == null || !invoices.Any())
				{
					result.IsSuccess = false;
					result.Code = (int)HttpStatusCode.NotFound;
					result.Message = "No invoices found for the specified membership plan.";
					return result;
				}
				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.OK;
				result.Data = invoices;
			}
			catch (Exception ex)
			{
				result.IsSuccess = false;
				result.Code = (int)HttpStatusCode.InternalServerError;
				result.Message = ex.Message;
			}
			return result;
		}

		public async Task<ResultModel> GetInvoicesByPaymentMethodAsync(Guid paymentMethodId)
		{
			var result = new ResultModel();
			try
			{
				var invoices = await _repo.GetInvoicesByPaymentMethodAsync(paymentMethodId);
				if (invoices == null || !invoices.Any())
				{
					result.IsSuccess = false;
					result.Code = (int)HttpStatusCode.NotFound;
					result.Message = "No invoices found for the specified payment method.";
					return result;
				}
				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.OK;
				result.Data = invoices;
			}
			catch (Exception ex)
			{
				result.IsSuccess = false;
				result.Code = (int)HttpStatusCode.InternalServerError;
				result.Message = ex.Message;
			}
			return result;
		}

		public async Task<ResultModel> GetInvoicesByDiscountAsync(Guid discountId)
		{
			var result = new ResultModel();
			try
			{
				var invoices = await _repo.GetInvoicesByDiscountAsync(discountId);
				if (invoices == null || !invoices.Any())
				{
					result.IsSuccess = false;
					result.Code = (int)HttpStatusCode.NotFound;
					result.Message = "No invoices found for the specified discount.";
					return result;
				}
				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.OK;
				result.Data = invoices;
			}
			catch (Exception ex)
			{
				result.IsSuccess = false;
				result.Code = (int)HttpStatusCode.InternalServerError;
				result.Message = ex.Message;
			}
			return result;
		}
	}
}

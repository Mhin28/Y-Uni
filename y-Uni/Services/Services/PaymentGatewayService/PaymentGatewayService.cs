using Repositories.Models;
using Repositories.Repositories;
using Repositories.ViewModels.PaymentGatewayModel;
using Repositories.ViewModels.ResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.PaymentGatewayService
{
	public class PaymentGatewayService : IPaymentGatewayService
	{
		private readonly IPaymentGatewayRepo _repo;

		public PaymentGatewayService(IPaymentGatewayRepo repo)
		{
			_repo = repo;
		}

		public async Task<ResultModel> GetAllAsync()
		{
			var result = new ResultModel();
			try
			{
				var paymentGateways = await _repo.GetAllAsync();
				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.OK;
				result.Data = paymentGateways;
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
				var paymentGateway = await _repo.GetByIdAsync(id);
				if (paymentGateway == null)
				{
					result.IsSuccess = false;
					result.Code = (int)HttpStatusCode.NotFound;
					result.Message = "Payment gateway not found.";
					return result;
				}
				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.OK;
				result.Data = paymentGateway;
			}
			catch (Exception ex)
			{
				result.IsSuccess = false;
				result.Code = (int)HttpStatusCode.InternalServerError;
				result.Message = ex.Message;
			}
			return result;
		}

		public async Task<ResultModel> AddAsync(PostPaymentGatewayModel model)
		{
			var result = new ResultModel
			{
				IsSuccess = false,
				Code = (int)HttpStatusCode.BadRequest,
				Message = "Invalid request"
			};
			
			try
			{
				
				var paymentGateway = new PaymentGateway
				{
					GatewayId = Guid.NewGuid(),
					GatewayName = model.GatewayName,
					ApiKey = model.ApiKey,
					IsActive = model.IsActive
				};
				await _repo.CreateAsync(paymentGateway);
				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.Created;
				result.Data = paymentGateway;
			}
			catch (Exception ex)
			{
				result.IsSuccess = false;
				result.Code = (int)HttpStatusCode.InternalServerError;
				result.Message = ex.Message;
			}
			return result;
		}

		public async Task<ResultModel> UpdateAsync(PaymentGatewayModel model)
		{
			var result = new ResultModel
			{
				IsSuccess = false,
				Code = (int)HttpStatusCode.BadRequest,
				Message = "Invalid request"
			};
			try
			{
				var updatedPaymentGateway = await _repo.GetByIdAsync(model.GatewayId);
				if (updatedPaymentGateway == null)
				{
					result.IsSuccess = false;
					result.Code = (int)HttpStatusCode.NotFound;
					result.Message = "Payment gateway not found.";
					return result;
				}
				if (!string.IsNullOrEmpty(model.GatewayName))
					updatedPaymentGateway.GatewayName = model.GatewayName;

				if (!string.IsNullOrEmpty(model.ApiKey))
					updatedPaymentGateway.ApiKey = model.ApiKey;

				if (model.IsActive.HasValue)
					updatedPaymentGateway.IsActive = model.IsActive.Value;


				await _repo.UpdateAsync(updatedPaymentGateway);
				if (updatedPaymentGateway == null)
				{
					result.IsSuccess = false;
					result.Code = (int)HttpStatusCode.NotFound;
					result.Message = "Payment gateway not found.";
					return result;
				}
				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.OK;
				result.Data = updatedPaymentGateway;
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
			var result = new ResultModel
			{
				IsSuccess = false,
				Code = (int)HttpStatusCode.BadRequest,
				Message = "Invalid request"
			};
			try
			{
				var model = await _repo.GetByIdAsync(id);
				if (model == null)
				{
					result.IsSuccess = false;
					result.Code = (int)HttpStatusCode.NotFound;
					result.Message = "Payment gateway not found.";
					return result;
				}
				var isDeleted = await _repo.RemoveAsync(model);
				
				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.NoContent;
				result.Message = isDeleted ? "Payment gateway deleted successfully." : "Failed to delete payment gateway.";
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

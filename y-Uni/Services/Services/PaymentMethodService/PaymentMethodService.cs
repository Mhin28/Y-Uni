using Repositories.Models;
using Repositories.Repositories;
using Repositories.ViewModels.PaymentMethodModel;
using Repositories.ViewModels.ResultModels;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Services.Services.PaymentMethodService
{
	public class PaymentMethodService : IPaymentMethodService
	{
		private readonly IPaymentMethodRepo _repo;

		public PaymentMethodService(IPaymentMethodRepo repo)
		{
			_repo = repo;
		}

		public async Task<ResultModel> GetAllAsync()
		{
			var result = new ResultModel();
			try
			{
				var paymentMethods = await _repo.GetAllAsync(pm => pm.Invoices);
				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.OK;
				result.Data = paymentMethods;
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
				var paymentMethod = await _repo.GetByIdAsync(id);
				if (paymentMethod == null)
				{
					result.IsSuccess = false;
					result.Code = (int)HttpStatusCode.NotFound;
					result.Message = "Audit log not found.";
					return result;
				}
				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.OK;
				result.Data = paymentMethod;
			}
			catch (Exception ex)
			{
				result.IsSuccess = false;
				result.Code = (int)HttpStatusCode.InternalServerError;
				result.Message = ex.Message;
			}
			return result;
		}

		public async Task<ResultModel> AddAsync(PostPaymentMethodModel model)
		{
			var result = new ResultModel
			{
				IsSuccess = false,
				Code = (int)HttpStatusCode.BadRequest,
				Message = "Invalid request"
			};

			try
			{
				var paymentMethod = new PaymentMethod
				{
					MethodId = Guid.NewGuid(),
					MethodName = model.MethodName,
					IsActive = model.IsActive
				};
				await _repo.CreateAsync(paymentMethod);
				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.Created;
				result.Data = model;
				result.Message = "Payment method added successfully";
			}
			catch (Exception ex)
			{
				result.Message = ex.Message;
			}

			return result;
		}

		public async Task<ResultModel> UpdateAsync(PaymentMethodModel model)
		{
			var result = new ResultModel
			{
				IsSuccess = false,
				Code = (int)HttpStatusCode.BadRequest,
				Message = "Update failed"
			};

			try
			{
				var existing = await _repo.GetByIdAsync(model.MethodId);
				if (existing == null)
				{
					result.IsSuccess = false;
					result.Code = (int)HttpStatusCode.NotFound;
					result.Message = "Payment method not found";
					return result;
				}

				if (!string.IsNullOrEmpty(model.MethodName))
					existing.MethodName = model.MethodName;

				if (model.IsActive.HasValue)
					existing.IsActive = model.IsActive.Value;

				await _repo.UpdateAsync(existing);

				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.OK;
				result.Data = existing;
				result.Message = "Payment method updated successfully";
			}
			catch (Exception ex)
			{
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
				Message = "Delete failed"
			};

			try
			{
				var model = await _repo.GetByIdAsync(id);
				if (model == null)
				{
					result.IsSuccess = false;
					result.Code = (int)HttpStatusCode.NotFound;
					result.Message = "Payment method not found";
					return result;
				}

				await _repo.RemoveAsync(model);

				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.OK;
				result.Message = "Deleted successfully";
			}
			catch (Exception ex)
			{
				result.Message = ex.Message;
			}

			return result;
		}
	}
}

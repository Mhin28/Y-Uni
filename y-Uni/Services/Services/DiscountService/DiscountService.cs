using Repositories.Models;
using Repositories.Repositories;
using Repositories.ViewModels.DiscountModel;
using Repositories.ViewModels.ResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.DiscountService
{
	public class DiscountService : IDiscountService
	{
		private readonly IDiscountRepo _repo;
		public DiscountService(IDiscountRepo repo) 
		{
			_repo = repo;
		}

		public async Task<ResultModel> GetAllAsync()
		{
			var result = new ResultModel();
			try
			{
				var discounts = await _repo.GetAllAsync();
				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.OK;
				result.Data = discounts;
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
				var discount = await _repo.GetByIdAsync(id);
				if (discount == null)
				{
					result.IsSuccess = false;
					result.Code = (int)HttpStatusCode.NotFound;
					result.Message = "Discount not found.";
					return result;
				}
				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.OK;
				result.Data = discount;
			}
			catch (Exception ex)
			{
				result.IsSuccess = false;
				result.Code = (int)HttpStatusCode.InternalServerError;
				result.Message = ex.Message;
			}
			return result;
		}

		public async Task<ResultModel> AddAsync(PostDiscountModel model)
		{
			var result = new ResultModel
			{
				IsSuccess = false,
				Code = (int)HttpStatusCode.BadRequest,
				Message = "Invalid request"
			};
			try
			{
				var discount = new Discount
				{
					DiscountId = Guid.NewGuid(),
					DiscountName = model.DiscountName,
					DiscountPercentage = model.DiscountPercentage,
					IsActive = model.IsActive

				};
				await _repo.CreateAsync(discount);
				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.Created;
				result.Data = discount;
			}
			catch (Exception ex)
			{
				result.IsSuccess = false;
				result.Code = (int)HttpStatusCode.InternalServerError;
				result.Message = ex.Message;
			}
			return result;
		}

		public async Task<ResultModel> UpdateAsync(DiscountModel model)
		{
			var result = new ResultModel()
			{
				IsSuccess = false,
				Code = (int)HttpStatusCode.BadRequest,
				Message = "Invalid request"
			};
			try
			{
				var discount = await _repo.GetByIdAsync(model.DiscountId);
				if (discount == null)
				{
					result.IsSuccess = false;
					result.Code = (int)HttpStatusCode.NotFound;
					result.Message = "Discount not found.";
					return result;
				}
				if (model.DiscountName != null)
					discount.DiscountName = model.DiscountName;

				if (model.DiscountPercentage.HasValue)
					discount.DiscountPercentage = model.DiscountPercentage.Value;

				if (model.IsActive.HasValue)
					discount.IsActive = model.IsActive.Value;
				await _repo.UpdateAsync(discount);
				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.OK;
				result.Data = discount;
				result.Message = "Discount updated successfully.";
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
				var discount = await _repo.GetByIdAsync(id);
				if (discount == null)
				{
					result.IsSuccess = false;
					result.Code = (int)HttpStatusCode.NotFound;
					result.Message = "Discount not found.";
					return result;
				}
				await _repo.RemoveAsync(discount);
				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.OK;
				result.Message = "Discount deleted successfully.";
			}
			catch (Exception ex)
			{
				result.IsSuccess = false;
				result.Code = (int)HttpStatusCode.InternalServerError;
				result.Message = ex.Message;
			}
			return result;
		}

		public async Task<ResultModel> GetActiveAsync()
		{
			var result = new ResultModel();
			try
			{
				var discounts = await _repo.GetActiveDiscountsAsync();
				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.OK;
				result.Data = discounts;
			}
			catch (Exception ex)
			{
				result.IsSuccess = false;
				result.Code = (int)HttpStatusCode.InternalServerError;
				result.Message = ex.Message;
			}
			return result;
		}

		public async Task<ResultModel> GetByNameAsync(string name)
		{
			var result = new ResultModel();
			try
			{
				var discounts = await _repo.GetDiscountsByNameAsync(name);
				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.OK;
				result.Data = discounts;
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

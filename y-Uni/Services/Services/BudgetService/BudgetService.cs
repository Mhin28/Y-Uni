using Repositories.Models;
using Repositories.Repositories;
using Repositories.ViewModels.BudgetModel;
using Repositories.ViewModels.ResultModels;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Services.Services.BudgetService
{
	public class BudgetService : IBudgetService
	{
		private readonly IBudgetRepo _repo;

		public BudgetService(IBudgetRepo repo)
		{
			_repo = repo;
		}

		public async Task<ResultModel> GetAllAsync()
		{
			var result = new ResultModel();
			try
			{
				var budgets = await _repo.GetAllAsync(b => b.Account, b => b.Category, b => b.User);
				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.OK;
				result.Data = budgets;
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
				var budget = await _repo.GetByIdAsync(id);
				if (budget == null)
				{
					result.IsSuccess = false;
					result.Code = (int)HttpStatusCode.NotFound;
					result.Message = "Budget not found.";
					return result;
				}
				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.OK;
				result.Data = budget;
			}
			catch (Exception ex)
			{
				result.IsSuccess = false;
				result.Code = (int)HttpStatusCode.InternalServerError;
				result.Message = ex.Message;
			}
			return result;
		}

		public async Task<ResultModel> AddAsync(PostBudgetModel model)
		{
			var result = new ResultModel
			{
				IsSuccess = false,
				Code = (int)HttpStatusCode.BadRequest,
				Message = "Invalid request"
			};

			try
			{
				var budget = new Budget
				{
					BudgetId = Guid.NewGuid(),
					CategoryId = model.CategoryId,
					AccountId = model.AccountId,
					BudgetAmount = model.BudgetAmount,
					StartDate = model.StartDate,
					EndDate = model.EndDate,
					UserId = model.UserId
				};
				await _repo.CreateAsync(budget);
				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.Created;
				result.Data = model;
				result.Message = "Budget created successfully";
			}
			catch (Exception ex)
			{
				result.Message = ex.Message;
			}

			return result;
		}

		public async Task<ResultModel> UpdateAsync(BudgetModel model)
		{
			var result = new ResultModel
			{
				IsSuccess = false,
				Code = (int)HttpStatusCode.BadRequest,
				Message = "Update failed"
			};

			try
			{
				var existing = await _repo.GetByIdAsync(model.BudgetId);
				if (existing == null)
				{
					result.Message = "Budget not found";
					return result;
				}

				if (model.CategoryId.HasValue)
					existing.CategoryId = model.CategoryId.Value;

				if (model.AccountId.HasValue)
					existing.AccountId = model.AccountId.Value;

				if (model.BudgetAmount.HasValue)
					existing.BudgetAmount = model.BudgetAmount.Value;

				if (model.StartDate.HasValue)
					existing.StartDate = model.StartDate.Value;

				if (model.EndDate.HasValue)
					existing.EndDate = model.EndDate.Value;

				if (model.UserId.HasValue)
					existing.UserId = model.UserId.Value;

				await _repo.UpdateAsync(existing);

				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.OK;
				result.Data = existing;
				result.Message = "Budget updated successfully";
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
					result.Message = "Budget not found";
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
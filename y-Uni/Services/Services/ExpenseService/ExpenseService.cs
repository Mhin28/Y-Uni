using Repositories.Models;
using Repositories.Repositories;
using Repositories.ViewModels.ExpenseModel;
using Repositories.ViewModels.ResultModels;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Services.Services.ExpenseService
{
	public class ExpenseService : IExpenseService
	{
		private readonly IExpenseRepo _repo;

		public ExpenseService(IExpenseRepo repo)
		{
			_repo = repo;
		}

		public async Task<ResultModel> GetAllAsync()
		{
			var result = new ResultModel();
			try
			{
				var expenses = await _repo.GetAllAsync(e => e.Account, e => e.ExC, e => e.User);
				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.OK;
				result.Data = expenses;
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
				var expense = await _repo.GetByIdAsync(id);
				if (expense == null)
				{
					result.IsSuccess = false;
					result.Code = (int)HttpStatusCode.NotFound;
					result.Message = "Expense not found.";
					return result;
				}
				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.OK;
				result.Data = expense;
			}
			catch (Exception ex)
			{
				result.IsSuccess = false;
				result.Code = (int)HttpStatusCode.InternalServerError;
				result.Message = ex.Message;
			}
			return result;
		}

		public async Task<ResultModel> AddAsync(PostExpenseModel model)
		{
			var result = new ResultModel
			{
				IsSuccess = false,
				Code = (int)HttpStatusCode.BadRequest,
				Message = "Invalid request"
			};

			try
			{
				var expense = new Expense
				{
					ExpensesId = Guid.NewGuid(),
					Amount = model.Amount,
					Description = model.Description,
					CreatedDate = DateTime.UtcNow,
					Type = model.Type,
					Frequency = model.Frequency,
					NextDueDate = model.NextDueDate,
					ExCid = model.ExCid,
					AccountId = model.AccountId,
					UserId = model.UserId
				};
				await _repo.CreateAsync(expense);
				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.Created;
				result.Data = model;
				result.Message = "Expense created successfully";
			}
			catch (Exception ex)
			{
				result.Message = ex.Message;
			}

			return result;
		}

		public async Task<ResultModel> UpdateAsync(ExpenseModel model)
		{
			var result = new ResultModel
			{
				IsSuccess = false,
				Code = (int)HttpStatusCode.BadRequest,
				Message = "Update failed"
			};

			try
			{
				var existing = await _repo.GetByIdAsync(model.ExpensesId);
				if (existing == null)
				{
					result.Message = "Expense not found";
					return result;
				}

				if (model.Amount.HasValue)
					existing.Amount = model.Amount.Value;

				if (!string.IsNullOrEmpty(model.Description))
					existing.Description = model.Description;

				if (model.CreatedDate.HasValue)
					existing.CreatedDate = model.CreatedDate.Value;

				if (!string.IsNullOrEmpty(model.Type))
					existing.Type = model.Type;

				if (!string.IsNullOrEmpty(model.Frequency))
					existing.Frequency = model.Frequency;

				if (model.NextDueDate.HasValue)
					existing.NextDueDate = model.NextDueDate.Value;

				if (model.ExCid.HasValue)
					existing.ExCid = model.ExCid.Value;

				if (model.AccountId.HasValue)
					existing.AccountId = model.AccountId.Value;

				if (model.UserId.HasValue)
					existing.UserId = model.UserId.Value;


				await _repo.UpdateAsync(existing);

				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.OK;
				result.Data = existing;
				result.Message = "Expense updated successfully";
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
					result.Message = "Expense not found";
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

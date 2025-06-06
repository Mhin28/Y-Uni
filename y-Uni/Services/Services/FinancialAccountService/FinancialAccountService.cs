using Repositories.Models;
using Repositories.Repositories;
using Repositories.ViewModels.FinancialAccountModel;
using Repositories.ViewModels.ResultModels;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Services.Services.FinancialAccountService
{
	public class FinancialAccountService : IFinancialAccountService
	{
		private readonly FinancialAccountRepo _repo;

		public FinancialAccountService(FinancialAccountRepo repo)
		{
			_repo = repo;
		}

		public async Task<ResultModel> GetAllAsync()
		{
			var result = new ResultModel();
			try
			{
				var accounts = await _repo.GetAllAsync(fa => fa.User, fa => fa.Budgets, fa => fa.Expenses);
				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.OK;
				result.Data = accounts;
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
				var account = await _repo.GetByIdAsync(id);
				if (account == null)
				{
					result.IsSuccess = false;
					result.Code = (int)HttpStatusCode.NotFound;
					result.Message = "FinancialAccount not found.";
					return result;
				}
				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.OK;
				result.Data = account;
			}
			catch (Exception ex)
			{
				result.IsSuccess = false;
				result.Code = (int)HttpStatusCode.InternalServerError;
				result.Message = ex.Message;
			}
			return result;
		}

		public async Task<ResultModel> AddAsync(PostFinancialAccModel model)
		{
			var result = new ResultModel
			{
				IsSuccess = false,
				Code = (int)HttpStatusCode.BadRequest,
				Message = "Invalid request"
			};

			try
			{
				var financialAccount = new FinancialAccount
				{
					AccountName = model.AccountName,
					Balance = model.Balance,
					CurrencyCode = model.CurrencyCode,
					UserId = model.UserId,
					IsDefault = model.IsDefault
				};
				await _repo.CreateAsync(financialAccount);

				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.Created;
				result.Data = model;
			}
			catch (Exception ex)
			{
				result.Message = ex.Message;
			}

			return result;
		}

		public async Task<ResultModel> UpdateAsync(FinancialAccModel model)
		{
			var result = new ResultModel
			{
				IsSuccess = false,
				Code = (int)HttpStatusCode.BadRequest,
				Message = "Update failed"
			};

			try
			{
				var existing = await _repo.GetByIdAsync(model.AccountId);
				if (existing == null)
				{
					result.Message = "FinancialAccount not found";
					return result;
				}

				existing.AccountName = model.AccountName;
				existing.Balance = model.Balance;
				existing.CurrencyCode = model.CurrencyCode;
				existing.UserId = model.UserId;
				existing.IsDefault = model.IsDefault;

				await _repo.UpdateAsync(existing);

				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.OK;
				result.Data = existing;
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
					result.Message = "FinancialAccount not found";
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

		public async Task<ResultModel> GetByUserIdAsync(Guid userId)
		{
			var result = new ResultModel();
			try
			{
				var accounts = await _repo.GetByUserIdAsync(userId);
				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.OK;
				result.Data = accounts;
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

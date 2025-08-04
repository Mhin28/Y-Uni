using Microsoft.IdentityModel.Tokens;
using Repositories.Models;
using Repositories.Repositories;
using Repositories.ViewModels.FinancialAccountModel;
using Repositories.ViewModels.ResultModels;
using Services.Services.UserContextService;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Services.Services.FinancialAccountService
{
	public class FinancialAccountService : IFinancialAccountService
	{
		private readonly IFinancialAccountRepo _repo;
		private readonly IUserContextService _userContextService;

		public FinancialAccountService(IFinancialAccountRepo repo, IUserContextService userContextService)
		{
			_repo = repo;
			_userContextService = userContextService;
		}

		public async Task<ResultModel> GetAllAsync()
		{
			var result = new ResultModel();
			try
			{
				var userId = _userContextService.GetCurrentUserId();
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

		public async Task<ResultModel> GetByIdAsync(Guid id)
		{
			var result = new ResultModel();
			try
			{
				var userId = _userContextService.GetCurrentUserId();
				var account = await _repo.GetByIdAsync(id);
				if (account == null)
				{
					result.IsSuccess = false;
					result.Code = (int)HttpStatusCode.NotFound;
					result.Message = "FinancialAccount not found.";
					return result;
				}
				
				// Check if the account belongs to the current user
				if (account.UserId != userId)
				{
					result.IsSuccess = false;
					result.Code = (int)HttpStatusCode.Forbidden;
					result.Message = "Access denied. You can only access your own financial accounts.";
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
				var userId = _userContextService.GetCurrentUserId();
				var financialAccount = new FinancialAccount
				{
					AccountId = Guid.NewGuid(),
					AccountName = model.AccountName,
					Balance = model.Balance,
					CurrencyCode = model.CurrencyCode,
					UserId = userId,
					IsDefault = model.IsDefault
				};
				await _repo.CreateAsync(financialAccount);

				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.Created;
				result.Data = financialAccount;
				result.Message = "FinancialAccount created successfully";
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
				var userId = _userContextService.GetCurrentUserId();
				var existing = await _repo.GetByIdAsync(model.AccountId);
				if (existing == null)
				{
					result.Message = "FinancialAccount not found";
					return result;
				}
				
				// Check if the account belongs to the current user
				if (existing.UserId != userId)
				{
					result.Code = (int)HttpStatusCode.Forbidden;
					result.Message = "Access denied. You can only update your own financial accounts.";
					return result;
				}

				if (!string.IsNullOrEmpty(model.AccountName))
					existing.AccountName = model.AccountName;

				if (model.Balance.HasValue)
					existing.Balance = model.Balance.Value;

				if (!string.IsNullOrEmpty(model.CurrencyCode))
					existing.CurrencyCode = model.CurrencyCode;

				if (model.UserId.HasValue)
					existing.UserId = model.UserId.Value;

				if (model.IsDefault.HasValue)
					existing.IsDefault = model.IsDefault.Value;

				await _repo.UpdateAsync(existing);

				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.OK;
				result.Data = existing;
				result.Message = "FinancialAccount updated successfully";
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
				var userId = _userContextService.GetCurrentUserId();
				var model = await _repo.GetByIdAsync(id);
				if (model == null)
				{
					result.Message = "FinancialAccount not found";
					return result;
				}
				
				// Check if the account belongs to the current user
				if (model.UserId != userId)
				{
					result.Code = (int)HttpStatusCode.Forbidden;
					result.Message = "Access denied. You can only delete your own financial accounts.";
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
				var currentUserId = _userContextService.GetCurrentUserId();
				
				// Only allow users to get their own accounts
				if (userId != currentUserId)
				{
					result.IsSuccess = false;
					result.Code = (int)HttpStatusCode.Forbidden;
					result.Message = "Access denied. You can only access your own financial accounts.";
					return result;
				}
				
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

using Repositories.Models;
using Repositories.Repositories;
using Repositories.ViewModels.ExpenseModel;
using Repositories.ViewModels.ResultModels;
using Repositories.ViewModels.FinancialDashboardModel;
using Services.Services.UserContextService;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Linq;

namespace Services.Services.ExpenseService
{
	public class ExpenseService : IExpenseService
	{
		private readonly IExpenseRepo _repo;
		private readonly IExpensesCategoryRepo _categoryRepo;
		private readonly IUserContextService _userContextService;

		public ExpenseService(IExpenseRepo repo, IExpensesCategoryRepo categoryRepo, IUserContextService userContextService)
		{
			_repo = repo;
			_categoryRepo = categoryRepo;
			_userContextService = userContextService;
		}

		public async Task<ResultModel> GetAllAsync()
		{
			var result = new ResultModel();
			try
			{
				var userId = _userContextService.GetCurrentUserId();
				var expenses = await _repo.GetByUserIdAsync(userId);
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
				var userId = _userContextService.GetCurrentUserId();
				var expense = await _repo.GetByIdAsync(id);
				if (expense == null)
				{
					result.IsSuccess = false;
					result.Code = (int)HttpStatusCode.NotFound;
					result.Message = "Expense not found.";
					return result;
				}
				
				// Check if the expense belongs to the current user
				if (expense.UserId != userId)
				{
					result.IsSuccess = false;
					result.Code = (int)HttpStatusCode.Forbidden;
					result.Message = "Access denied. You can only access your own expenses.";
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
				var userId = _userContextService.GetCurrentUserId();
				var expense = new Expense
				{
					ExpensesId = Guid.NewGuid(),
					Amount = model.Amount,
					Description = model.Description,
					CreatedDate = DateTime.UtcNow,
					ExCid = model.ExCid,
					AccountId = model.AccountId,
					UserId = userId
				};
				await _repo.CreateAsync(expense);
				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.Created;
				result.Data = expense;
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
				var userId = _userContextService.GetCurrentUserId();
				var existing = await _repo.GetByIdAsync(model.ExpensesId);
				if (existing == null)
				{
					result.Message = "Expense not found";
					return result;
				}
				
				// Check if the expense belongs to the current user
				if (existing.UserId != userId)
				{
					result.Code = (int)HttpStatusCode.Forbidden;
					result.Message = "Access denied. You can only update your own expenses.";
					return result;
				}

				if (model.Amount.HasValue)
					existing.Amount = model.Amount.Value;

				if (!string.IsNullOrEmpty(model.Description))
					existing.Description = model.Description;

				if (model.CreatedDate.HasValue)
					existing.CreatedDate = model.CreatedDate.Value;

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
				var userId = _userContextService.GetCurrentUserId();
				var model = await _repo.GetByIdAsync(id);
				if (model == null)
				{
					result.Message = "Expense not found";
					return result;
				}
				
				// Check if the expense belongs to the current user
				if (model.UserId != userId)
				{
					result.Code = (int)HttpStatusCode.Forbidden;
					result.Message = "Access denied. You can only delete your own expenses.";
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

		public async Task<ResultModel> GetRecentTransactionsAsync()
		{
			var result = new ResultModel();
			try
			{
				var userId = _userContextService.GetCurrentUserId();
				var currentDate = DateTime.Now;
				var expenses = await _repo.GetUserExpensesForMonthAsync(userId, currentDate.Year, currentDate.Month);
				var categories = await _categoryRepo.GetAllAsync();

				// Get only expense transactions (filter out income)
				var transactions = expenses
					.Where(e => e.ExC != null && e.ExC.Type == "expense")
					.Select(e =>
					{
						var category = categories.FirstOrDefault(c => c.ExCid == e.ExCid);
						return new RecentTransactionDto
						{
							ExpensesId = e.ExpensesId,
							Amount = e.Amount,
							Description = e.Description ?? "",
							CreatedDate = e.CreatedDate ?? DateTime.Now,
							ExCid = e.ExCid ?? Guid.Empty,
							CategoryName = category?.CategoryName ?? "Unknown",
							UserId = e.UserId,
							AccountId = e.AccountId ?? Guid.Empty,
						};
					}).ToList();

				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.OK;
				result.Data = transactions;
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

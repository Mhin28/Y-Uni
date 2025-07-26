using Repositories.Models;
using Repositories.Repositories;
using Repositories.ViewModels.BudgetModel;
using Repositories.ViewModels.ResultModels;
using Services.Services.TokenService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Services.Services.BudgetService
{
    public class BudgetService : IBudgetService
    {
        private readonly IBudgetRepo _repo;
        private readonly ITokenService _tokenService;

        public BudgetService(IBudgetRepo repo, ITokenService tokenService)
        {
            _repo = repo;
            _tokenService = tokenService;
        }

        public async Task<ResultModel> GetAllAsync(string token)
        {
            var decoded = _tokenService.decode(token);
            if (decoded == null || string.IsNullOrEmpty(decoded.userid) || decoded.role != "2")
                return new ResultModel { IsSuccess = false, Code = (int)HttpStatusCode.Forbidden, Message = "Access denied" };

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

        public async Task<ResultModel> GetByIdAsync(string token, Guid id)
        {
            var decoded = _tokenService.decode(token);
            if (decoded == null || string.IsNullOrEmpty(decoded.userid) || decoded.role != "2")
                return new ResultModel { IsSuccess = false, Code = (int)HttpStatusCode.Forbidden, Message = "Access denied" };

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

        public async Task<ResultModel> AddAsync(string token, PostBudgetModel model)
        {
            var decoded = _tokenService.decode(token);
            if (decoded == null || string.IsNullOrEmpty(decoded.userid) || decoded.role != "2")
                return new ResultModel { IsSuccess = false, Code = (int)HttpStatusCode.Forbidden, Message = "Access denied" };

            var result = new ResultModel
            {
                IsSuccess = false,
                Code = (int)HttpStatusCode.BadRequest,
                Message = "Invalid request"
            };

            try
            {
                var today = DateTime.Today;
                var budget = new Budget
                {
                    BudgetId = Guid.NewGuid(),
                    CategoryId = model.CategoryId,
                    AccountId = model.AccountId,
                    BudgetAmount = model.BudgetAmount,
                    StartDate = DateOnly.FromDateTime(new DateTime(today.Year, today.Month, 1)),
                    EndDate = DateOnly.FromDateTime(new DateTime(today.Year, today.Month + 1, 1).AddDays(-1)),
                    UserId = model.UserId
                };
                await _repo.CreateAsync(budget);
                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.Created;
                result.Data = budget;
                result.Message = "Budget created successfully";
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }

        public async Task<ResultModel> UpdateAsync(string token, BudgetModel model)
        {
            var decoded = _tokenService.decode(token);
            if (decoded == null || string.IsNullOrEmpty(decoded.userid) || decoded.role != "2")
                return new ResultModel { IsSuccess = false, Code = (int)HttpStatusCode.Forbidden, Message = "Access denied" };

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

        public async Task<ResultModel> DeleteAsync(string token, Guid id)
        {
            var decoded = _tokenService.decode(token);
            if (decoded == null || string.IsNullOrEmpty(decoded.userid) || decoded.role != "2")
                return new ResultModel { IsSuccess = false, Code = (int)HttpStatusCode.Forbidden, Message = "Access denied" };

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

        // Budget Lock/Carry-over functionality implementation
        public async Task<ResultModel> GetUserBudgetsForMonthAsync(string token, Guid userId, int year, int month)
        {
            var decoded = _tokenService.decode(token);
            if (decoded == null || string.IsNullOrEmpty(decoded.userid) || decoded.role != "2")
                return new ResultModel { IsSuccess = false, Code = (int)HttpStatusCode.Forbidden, Message = "Access denied" };

            var result = new ResultModel();
            try
            {
                var budgets = await _repo.GetUserBudgetsForMonthAsync(userId, year, month);
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

        public async Task<ResultModel> CopyBudgetsToNextMonthAsync(string token, Guid userId, List<Guid> budgetIds, int targetYear, int targetMonth)
        {
            var decoded = _tokenService.decode(token);
            if (decoded == null || string.IsNullOrEmpty(decoded.userid) || decoded.role != "2")
                return new ResultModel { IsSuccess = false, Code = (int)HttpStatusCode.Forbidden, Message = "Access denied" };

            var result = new ResultModel();
            try
            {
                var copiedBudgets = new List<Budget>();
                var targetStartDate = new DateOnly(targetYear, targetMonth, 1);
                var targetEndDate = targetStartDate.AddMonths(1).AddDays(-1);

                foreach (var budgetId in budgetIds)
                {
                    var originalBudget = await _repo.GetByIdAsync(budgetId);
                    if (originalBudget == null || originalBudget.UserId != userId)
                        continue;

                    // Check if budget already exists for this category in target month
                    var existingBudgets = await _repo.GetUserBudgetsForMonthAsync(userId, targetYear, targetMonth);
                    var existingBudget = existingBudgets.FirstOrDefault(b => b.CategoryId == originalBudget.CategoryId);

                    if (existingBudget != null)
                        continue; // Skip if budget already exists for this category

                    // Create new budget with same amount for next month
                    var newBudget = new Budget
                    {
                        BudgetId = Guid.NewGuid(),
                        CategoryId = originalBudget.CategoryId,
                        AccountId = originalBudget.AccountId,
                        BudgetAmount = originalBudget.BudgetAmount,
                        StartDate = targetStartDate,
                        EndDate = targetEndDate,
                        UserId = userId
                    };

                    await _repo.CreateAsync(newBudget);
                    copiedBudgets.Add(newBudget);
                }

                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.Created;
                result.Data = copiedBudgets;
                result.Message = $"Successfully copied {copiedBudgets.Count} budgets to {targetYear}/{targetMonth}";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = (int)HttpStatusCode.InternalServerError;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<ResultModel> CreateBudgetFromPreviousMonthAsync(string token, Guid userId, Guid previousBudgetId, int targetYear, int targetMonth)
        {
            var decoded = _tokenService.decode(token);
            if (decoded == null || string.IsNullOrEmpty(decoded.userid) || decoded.role != "2")
                return new ResultModel { IsSuccess = false, Code = (int)HttpStatusCode.Forbidden, Message = "Access denied" };

            var result = new ResultModel();
            try
            {
                var previousBudget = await _repo.GetByIdAsync(previousBudgetId);
                if (previousBudget == null || previousBudget.UserId != userId)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "Previous budget not found or access denied";
                    return result;
                }

                var targetStartDate = new DateOnly(targetYear, targetMonth, 1);
                var targetEndDate = targetStartDate.AddMonths(1).AddDays(-1);

                // Check if budget already exists for this category in target month
                var existingBudgets = await _repo.GetUserBudgetsForMonthAsync(userId, targetYear, targetMonth);
                var existingBudget = existingBudgets.FirstOrDefault(b => b.CategoryId == previousBudget.CategoryId);

                if (existingBudget != null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.Conflict;
                    result.Message = "Budget already exists for this category in the target month";
                    return result;
                }

                // Create locked budget with same amount
                var newBudget = new Budget
                {
                    BudgetId = Guid.NewGuid(),
                    CategoryId = previousBudget.CategoryId,
                    AccountId = previousBudget.AccountId,
                    BudgetAmount = previousBudget.BudgetAmount, // Locked amount from previous month
                    StartDate = targetStartDate,
                    EndDate = targetEndDate,
                    UserId = userId
                };

                await _repo.CreateAsync(newBudget);

                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.Created;
                result.Data = newBudget;
                result.Message = "Budget successfully locked and carried over to next month";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = (int)HttpStatusCode.InternalServerError;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<ResultModel> GetBudgetCarryOverSummaryAsync(string token, Guid userId, int fromYear, int fromMonth, int toYear, int toMonth)
        {
            var decoded = _tokenService.decode(token);
            if (decoded == null || string.IsNullOrEmpty(decoded.userid) || decoded.role != "2")
                return new ResultModel { IsSuccess = false, Code = (int)HttpStatusCode.Forbidden, Message = "Access denied" };

            var result = new ResultModel();
            try
            {
                var fromBudgets = await _repo.GetUserBudgetsForMonthAsync(userId, fromYear, fromMonth);
                var toBudgets = await _repo.GetUserBudgetsForMonthAsync(userId, toYear, toMonth);

                var summary = new BudgetCarryOverSummaryDto
                {
                    UserId = userId,
                    FromYear = fromYear,
                    FromMonth = fromMonth,
                    ToYear = toYear,
                    ToMonth = toMonth,
                    AvailableBudgets = fromBudgets.Select(b => new BudgetCarryOverItemDto
                    {
                        BudgetId = b.BudgetId,
                        CategoryId = b.CategoryId ?? Guid.Empty,
                        BudgetAmount = b.BudgetAmount,
                        IsAlreadyCarriedOver = toBudgets.Any(tb => tb.CategoryId == b.CategoryId)
                    }).ToList(),
                    TotalAvailableBudgets = fromBudgets.Count,
                    TotalCarriedOverBudgets = toBudgets.Count(tb => fromBudgets.Any(fb => fb.CategoryId == tb.CategoryId)),
                    TotalCarryOverAmount = toBudgets
                        .Where(tb => fromBudgets.Any(fb => fb.CategoryId == tb.CategoryId))
                        .Sum(tb => tb.BudgetAmount)
                };

                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.OK;
                result.Data = summary;
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
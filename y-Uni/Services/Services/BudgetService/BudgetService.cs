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
        private readonly IBudgetRepo _budgetRepo;
        private readonly ITokenService _tokenService;
        public BudgetService(IBudgetRepo budgetRepo, ITokenService tokenService)
        {
            _budgetRepo = budgetRepo;
            _tokenService = tokenService;
        }
        public async Task<ResultModel> GetBudgetsByUserAsync(string token, Guid? categoryId = null, DateOnly? from = null, DateOnly? to = null)
        {
            var decoded = _tokenService.decode(token);
            if (decoded == null || string.IsNullOrEmpty(decoded.userid) || decoded.role != "2")
                return new ResultModel { IsSuccess = false, Code = (int)HttpStatusCode.Forbidden, Message = "Không có quyền truy cập" };
            Guid userId = Guid.Parse(decoded.userid);
            var budgets = await _budgetRepo.GetBudgetsByUserIdAsync(userId, categoryId, from, to);
            var data = budgets.Select(b => new BudgetModel
            {
                BudgetId = b.BudgetId,
                CategoryId = b.CategoryId,
                AccountId = b.AccountId,
                BudgetAmount = b.BudgetAmount,
                StartDate = b.StartDate,
                EndDate = b.EndDate,
                UserId = b.UserId
            }).ToList();
            return new ResultModel { IsSuccess = true, Code = (int)HttpStatusCode.OK, Data = data, Message = "Lấy danh sách ngân sách thành công" };
        }
        public async Task<ResultModel> CreateBudgetAsync(string token, PostBudgetModel model)
        {
            var decoded = _tokenService.decode(token);
            if (decoded == null || string.IsNullOrEmpty(decoded.userid) || decoded.role != "2")
                return new ResultModel { IsSuccess = false, Code = (int)HttpStatusCode.Forbidden, Message = "Không có quyền truy cập" };
            Guid userId = Guid.Parse(decoded.userid);
            var budget = new Budget
            {
                BudgetId = Guid.NewGuid(),
                CategoryId = model.CategoryId,
                AccountId = model.AccountId,
                BudgetAmount = model.BudgetAmount,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                UserId = userId
            };
            var created = await _budgetRepo.AddBudgetAsync(budget);
            if (created == null)
                return new ResultModel { IsSuccess = false, Code = (int)HttpStatusCode.InternalServerError, Message = "Tạo ngân sách thất bại" };
            return new ResultModel { IsSuccess = true, Code = (int)HttpStatusCode.Created, Message = "Tạo ngân sách thành công", Data = new BudgetModel {
                BudgetId = created.BudgetId,
                CategoryId = created.CategoryId,
                AccountId = created.AccountId,
                BudgetAmount = created.BudgetAmount,
                StartDate = created.StartDate,
                EndDate = created.EndDate,
                UserId = created.UserId
            }};
        }
        public async Task<ResultModel> UpdateBudgetAsync(string token, Guid budgetId, UpdateBudgetModel model)
        {
            var decoded = _tokenService.decode(token);
            if (decoded == null || string.IsNullOrEmpty(decoded.userid) || decoded.role != "2")
                return new ResultModel { IsSuccess = false, Code = (int)HttpStatusCode.Forbidden, Message = "Không có quyền truy cập" };
            Guid userId = Guid.Parse(decoded.userid);
            var budget = await _budgetRepo.GetBudgetByIdAsync(budgetId);
            if (budget == null || budget.UserId != userId)
                return new ResultModel { IsSuccess = false, Code = (int)HttpStatusCode.NotFound, Message = "Ngân sách không tồn tại hoặc không thuộc quyền sở hữu" };
            budget.CategoryId = model.CategoryId;
            budget.AccountId = model.AccountId;
            budget.BudgetAmount = model.BudgetAmount;
            budget.StartDate = model.StartDate;
            budget.EndDate = model.EndDate;
            var success = await _budgetRepo.UpdateBudgetAsync(budget);
            if (!success)
                return new ResultModel { IsSuccess = false, Code = (int)HttpStatusCode.InternalServerError, Message = "Cập nhật ngân sách thất bại" };
            return new ResultModel { IsSuccess = true, Code = (int)HttpStatusCode.OK, Message = "Cập nhật ngân sách thành công" };
        }
        public async Task<ResultModel> DeleteBudgetAsync(string token, Guid budgetId)
        {
            var decoded = _tokenService.decode(token);
            if (decoded == null || string.IsNullOrEmpty(decoded.userid) || decoded.role != "2")
                return new ResultModel { IsSuccess = false, Code = (int)HttpStatusCode.Forbidden, Message = "Không có quyền truy cập" };
            Guid userId = Guid.Parse(decoded.userid);
            var budget = await _budgetRepo.GetBudgetByIdAsync(budgetId);
            if (budget == null || budget.UserId != userId)
                return new ResultModel { IsSuccess = false, Code = (int)HttpStatusCode.NotFound, Message = "Ngân sách không tồn tại hoặc không thuộc quyền sở hữu" };
            var success = await _budgetRepo.DeleteBudgetAsync(budget);
            if (!success)
                return new ResultModel { IsSuccess = false, Code = (int)HttpStatusCode.InternalServerError, Message = "Xóa ngân sách thất bại" };
            return new ResultModel { IsSuccess = true, Code = (int)HttpStatusCode.OK, Message = "Xóa ngân sách thành công" };
        }
    }
} 
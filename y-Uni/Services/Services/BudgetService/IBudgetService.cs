using Repositories.Models;
using Repositories.ViewModels.BudgetModel;
using Repositories.ViewModels.ResultModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Services.BudgetService
{
    public interface IBudgetService
    {
        Task<ResultModel> GetAllAsync(string token);
        Task<ResultModel> GetByIdAsync(string token, Guid id);
        Task<ResultModel> AddAsync(string token, PostBudgetModel model);
        Task<ResultModel> UpdateAsync(string token, BudgetModel model);
        Task<ResultModel> DeleteAsync(string token, Guid id);
        Task<ResultModel> GetUserBudgetsForMonthAsync(string token, Guid userId, int year, int month);
        Task<ResultModel> CopyBudgetsToNextMonthAsync(string token, Guid userId, List<Guid> budgetIds, int targetYear, int targetMonth);
        Task<ResultModel> CreateBudgetFromPreviousMonthAsync(string token, Guid userId, Guid previousBudgetId, int targetYear, int targetMonth);
        Task<ResultModel> GetBudgetCarryOverSummaryAsync(string token, Guid userId, int fromYear, int fromMonth, int toYear, int toMonth);
    }
} 
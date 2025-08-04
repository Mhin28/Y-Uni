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
        Task<ResultModel> GetAllAsync();
        Task<ResultModel> GetByIdAsync(Guid id);
        Task<ResultModel> AddAsync(PostBudgetModel model);
        Task<ResultModel> UpdateAsync(BudgetModel model);
        Task<ResultModel> DeleteAsync(Guid id);
        Task<ResultModel> GetUserBudgetsForMonthAsync(int year, int month);
        Task<ResultModel> CopyBudgetsToNextMonthAsync(List<Guid> budgetIds, int targetYear, int targetMonth);
        Task<ResultModel> CreateBudgetFromPreviousMonthAsync(Guid previousBudgetId, int targetYear, int targetMonth);
        Task<ResultModel> GetBudgetCarryOverSummaryAsync(int fromYear, int fromMonth, int toYear, int toMonth);
    }
} 
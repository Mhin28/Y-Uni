using Repositories.ViewModels.BudgetModel;
using Repositories.ViewModels.ResultModels;
using System;
using System.Threading.Tasks;

namespace Services.Services.BudgetService
{
    public interface IBudgetService
    {
        Task<ResultModel> GetBudgetsByUserAsync(string token, Guid? categoryId = null, DateOnly? from = null, DateOnly? to = null);
        Task<ResultModel> CreateBudgetAsync(string token, PostBudgetModel model);
        Task<ResultModel> UpdateBudgetAsync(string token, Guid budgetId, UpdateBudgetModel model);
        Task<ResultModel> DeleteBudgetAsync(string token, Guid budgetId);
    }
} 
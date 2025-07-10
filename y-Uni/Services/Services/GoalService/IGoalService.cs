using Repositories.ViewModels.GoalModel;
using Repositories.ViewModels.ResultModels;
using System;
using System.Threading.Tasks;

namespace Services.Services.GoalService
{
    public interface IGoalService
    {
        Task<ResultModel> GetGoalsByUserAsync(string token, string status = null, DateOnly? from = null, DateOnly? to = null);
        Task<ResultModel> UpdateGoalAsync(string token, Guid goalId, UpdateGoalModel model);
        Task<ResultModel> DeleteGoalAsync(string token, Guid goalId);
        Task<ResultModel> CreateGoalAsync(string token, PostGoalModel model);
    }
} 
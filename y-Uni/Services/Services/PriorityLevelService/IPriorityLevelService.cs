using Repositories.ViewModels.PriorityLevelModel;
using Repositories.ViewModels.ResultModels;
using System;
using System.Threading.Tasks;

namespace Services.Services.PriorityLevelService
{
    public interface IPriorityLevelService
    {
        Task<ResultModel> GetAllAsync();
        Task<ResultModel> GetByIdAsync(byte id);
        Task<ResultModel> AddAsync(PostPriorityLevelModel model);
        Task<ResultModel> UpdateAsync(PriorityLevelModel model);
        Task<ResultModel> DeleteAsync(byte id);
    }
} 
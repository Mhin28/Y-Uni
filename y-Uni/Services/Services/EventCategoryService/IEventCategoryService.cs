using Repositories.ViewModels.EventCategoryModel;
using Repositories.ViewModels.ResultModels;
using System;
using System.Threading.Tasks;

namespace Services.Services.EventCategoryService
{
    public interface IEventCategoryService
    {
        Task<ResultModel> GetAllAsync();
        Task<ResultModel> GetByIdAsync(Guid id);
        Task<ResultModel> AddAsync(PostEventCategoryModel model);
        Task<ResultModel> UpdateAsync(EventCategoryModel model);
        Task<ResultModel> DeleteAsync(Guid id);
    }
} 
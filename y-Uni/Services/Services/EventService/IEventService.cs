using Repositories.ViewModels.EventModel;
using Repositories.ViewModels.ResultModels;
using System;
using System.Threading.Tasks;

namespace Services.Services.EventService
{
    public interface IEventService
    {
        Task<ResultModel> GetAllAsync();
        Task<ResultModel> GetByIdAsync(Guid id);
        Task<ResultModel> GetByUserIdAsync(Guid userId);
        Task<ResultModel> GetUpcomingByUserIdAsync(Guid userId, DateTime startDate, DateTime endDate);
        Task<ResultModel> GetByCategoryAsync(Guid categoryId);
        Task<ResultModel> AddAsync(PostEventModel model);
        Task<ResultModel> UpdateAsync(EventModel model);
        Task<ResultModel> DeleteAsync(Guid id);
    }
} 
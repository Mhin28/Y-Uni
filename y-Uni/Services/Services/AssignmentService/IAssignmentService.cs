using Repositories.ViewModels.AssignmentModel;
using Repositories.ViewModels.ResultModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Services.AssignmentService
{
    public interface IAssignmentService
    {
        Task<ResultModel> GetAllAsync();
        Task<ResultModel> GetByIdAsync(Guid id);
        Task<ResultModel> GetByUserIdAsync(Guid userId);
        Task<ResultModel> GetBySubjectAsync(Guid subjectId);
        Task<ResultModel> GetUpcomingByUserIdAsync(Guid userId, DateTime? dueDate = null);
        Task<ResultModel> GetByStatusAsync(Guid userId, string status);
        Task<ResultModel> AddAsync(PostAssignmentModel model);
        Task<ResultModel> UpdateAsync(AssignmentModel model);
        Task<ResultModel> UpdateStatusAsync(Guid id, string status);
        Task<ResultModel> CompleteAssignmentAsync(Guid id);
        Task<ResultModel> DeleteAsync(Guid id);
    }
} 
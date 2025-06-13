using Repositories.ViewModels.ResultModels;
using Repositories.ViewModels.SubjectModel;
using System;
using System.Threading.Tasks;

namespace Services.Services.SubjectService
{
    public interface ISubjectService
    {
        Task<ResultModel> GetAllAsync();
        Task<ResultModel> GetByIdAsync(Guid id);
        Task<ResultModel> AddAsync(PostSubjectModel model);
        Task<ResultModel> UpdateAsync(SubjectModel model);
        Task<ResultModel> DeleteAsync(Guid id);
    }
} 
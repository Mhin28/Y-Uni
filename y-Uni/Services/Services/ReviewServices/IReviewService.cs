using Repositories.Models;
using Repositories.ViewModels.ResultModels;
using Repositories.ViewModels.ReviewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.ReviewServices
{
    public interface IReviewService
    {
        Task<ResultModel> GetAllAsync();
        Task<ResultModel> GetByUserIdAsync(Guid userId);
        Task<ResultModel> CreateOrUpdateAsync(Guid userId, int? rating, string comment);
        Task<ResultModel> DeleteAsync(Guid reviewId);
    }
}

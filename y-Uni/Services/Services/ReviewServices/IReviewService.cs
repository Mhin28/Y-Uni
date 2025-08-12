using Repositories.Models;
using Repositories.ViewModels.ResultModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.ReviewServices
{
    public interface IReviewService
    {
        Task<ResultModel> GetAllReviewsAsync();
        Task<ResultModel> GetReviewsByUserIdAsync(Guid userId);
        Task<ResultModel> GetReviewsByTokenAsync(string token);
        Task<ResultModel> CreateReviewAsync(string token, int rating, string comment);
        Task<ResultModel> UpdateReviewAsync(string token, Guid reviewId, int rating, string comment);
    }
}

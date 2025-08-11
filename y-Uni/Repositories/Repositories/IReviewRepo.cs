using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public interface IReviewRepo
    {
        Task<IEnumerable<Review>> GetAllAsync();
        Task<Review> GetByUserIdAsync(Guid userId);
        Task<Review> GetByIdAsync(Guid reviewId);
        Task AddAsync(Review review);
        Task UpdateAsync(Review review);
        Task DeleteAsync(Review review);
        Task SaveChangesAsync();
    }
}

using Microsoft.EntityFrameworkCore;
using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public class ReviewRepo : IReviewRepo
    {
        private readonly YuniBuddyContext _context;

        public ReviewRepo(YuniBuddyContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Review>> GetAllAsync()
        {
            return await _context.Reviews
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Reviews
                .Where(r => r.UserId == userId)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<Review> CreateAsync(Review review)
        {
            review.ReviewId = Guid.NewGuid();
            review.CreatedAt = DateTime.UtcNow;
            review.UpdatedAt = DateTime.UtcNow;

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<Review> UpdateAsync(Review review)
        {
            var existingReview = await _context.Reviews.FindAsync(review.ReviewId);
            if (existingReview == null) return null;

            existingReview.Rating = review.Rating;
            existingReview.Comment = review.Comment;
            existingReview.UpdatedAt = DateTime.UtcNow;

            _context.Reviews.Update(existingReview);
            await _context.SaveChangesAsync();
            return existingReview;
        }

        public async Task<Review> GetByIdAsync(Guid reviewId)
        {
            return await _context.Reviews
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.ReviewId == reviewId);
        }
    }
}
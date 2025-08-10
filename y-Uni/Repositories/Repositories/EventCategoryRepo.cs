using Microsoft.EntityFrameworkCore;
using Repositories.Base;
using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public class EventCategoryRepo : GenericRepository<EventCategory>, IEventCategoryRepo
    {
        public EventCategoryRepo(YuniBuddyContext context) : base(context) { }

        public async Task<EventCategory> GetByIdAsync(Guid categoryId)
        {
            return await _context.EventCategories
                .FirstOrDefaultAsync(c => c.EvCategoryId == categoryId);
        }

        public async Task<List<EventCategory>> GetAllOrderedByNameAsync()
        {
            return await _context.EventCategories
                .OrderBy(c => c.CategoryName)
                .ToListAsync();
        }

        public async Task<bool> IsCategoryInUseAsync(Guid categoryId)
        {
            return await _context.Events
                .AnyAsync(e => e.EvCategoryId == categoryId);
        }

        public async Task<List<EventCategory>> GetCategoriesByUserIdAsync(Guid userId)
        {
            return await _context.EventCategories
                .Where(c => c.UserId == userId)
                .OrderBy(c => c.CategoryName)
                .ToListAsync();
        }

        public async Task<bool> CategoryNameExistsForUserAsync(string categoryName, Guid userId)
        {
            return await _context.EventCategories
                .AnyAsync(c => c.CategoryName.ToLower() == categoryName.ToLower() && c.UserId == userId);
        }
    }
}
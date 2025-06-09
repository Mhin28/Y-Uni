using Microsoft.EntityFrameworkCore;
using Repositories.Base;
using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public class EventRepo : GenericRepository<Event>, IEventRepo
    {
        public EventRepo(YUniContext context) : base(context) { }

        public async Task<List<Event>> GetEventsByUserIdAsync(Guid userId)
        {
            return await _context.Events
                .Where(e => e.UserId == userId)
                .Include(e => e.EvCategory)
                .OrderBy(e => e.StartDateTime)
                .ToListAsync();
        }

        public async Task<List<Event>> GetUpcomingEventsByUserIdAsync(Guid userId, DateTime startDate, DateTime endDate)
        {
            return await _context.Events
                .Where(e => e.UserId == userId && 
                           e.StartDateTime >= startDate && 
                           e.StartDateTime <= endDate)
                .Include(e => e.EvCategory)
                .OrderBy(e => e.StartDateTime)
                .ToListAsync();
        }

        public async Task<List<Event>> GetEventsByCategoryAsync(Guid categoryId)
        {
            return await _context.Events
                .Where(e => e.EvCategoryId == categoryId)
                .OrderBy(e => e.StartDateTime)
                .ToListAsync();
        }
    }
} 
using Microsoft.EntityFrameworkCore;
using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public class GoalRepo : IGoalRepo
    {
        private readonly YUniContext _context;
        public GoalRepo(YUniContext context)
        {
            _context = context;
        }
        public async Task<List<Goal>> GetGoalsByUserIdAsync(Guid userId, string status = null, DateOnly? from = null, DateOnly? to = null)
        {
            var query = _context.Goals.Where(g => g.UserId == userId);
            if (!string.IsNullOrEmpty(status))
                query = query.Where(g => g.Status == status);
            if (from.HasValue)
                query = query.Where(g => g.TargetDate >= from.Value);
            if (to.HasValue)
                query = query.Where(g => g.TargetDate <= to.Value);
            return await query.ToListAsync();
        }
        public async Task<Goal> GetGoalByIdAsync(Guid goalId)
        {
            return await _context.Goals.FirstOrDefaultAsync(g => g.GoalId == goalId);
        }
        public async Task<bool> UpdateGoalAsync(Goal goal)
        {
            _context.Goals.Update(goal);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> DeleteGoalAsync(Goal goal)
        {
            _context.Goals.Remove(goal);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<Goal> AddGoalAsync(Goal goal)
        {
            _context.Goals.Add(goal);
            await _context.SaveChangesAsync();
            return goal;
        }
    }
} 
using Microsoft.EntityFrameworkCore;
using Repositories.Base;
using Repositories.Models;

namespace Repositories.Repositories
{
    public class BudgetRepo : GenericRepository<Budget>, IBudgetRepo
    {
        public BudgetRepo(YUniContext context) : base(context) { }

        public async Task<List<Budget>> GetUserBudgetsAsync(Guid userId)
        {
            return await _context.Budgets
                .Where(b => b.UserId == userId)
                .Include(b => b.Category)
                .Include(b => b.Account)
                .ToListAsync();
        }

        public async Task<List<Budget>> GetActiveBudgetsForUserAsync(Guid userId, DateTime date)
        {
            var dateOnly = DateOnly.FromDateTime(date);
            return await _context.Budgets
                .Where(b => b.UserId == userId &&
                           b.StartDate <= dateOnly &&
                           b.EndDate >= dateOnly)
                .Include(b => b.Category)
                .Include(b => b.Account)
                .ToListAsync();
        }

        public async Task<List<Budget>> GetUserBudgetsForMonthAsync(Guid userId, int year, int month)
        {
            var startDate = new DateOnly(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            return await _context.Budgets
                .Where(b => b.UserId == userId &&
                           ((b.StartDate <= startDate && b.EndDate >= startDate) ||
                            (b.StartDate <= endDate && b.EndDate >= endDate) ||
                            (b.StartDate >= startDate && b.EndDate <= endDate)))
                .Include(b => b.Category)
                .Include(b => b.Account)
                .ToListAsync();
        }

        public async Task<Budget?> GetBudgetByCategoryAsync(Guid userId, Guid categoryId)
        {
            return await _context.Budgets
                .Where(b => b.UserId == userId && b.CategoryId == categoryId)
                .Include(b => b.Category)
                .Include(b => b.Account)
                .FirstOrDefaultAsync();
        }
    }
} 
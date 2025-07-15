using Microsoft.EntityFrameworkCore;
using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public class BudgetRepo : IBudgetRepo
    {
        private readonly YUniContext _context;
        public BudgetRepo(YUniContext context)
        {
            _context = context;
        }
        public async Task<List<Budget>> GetBudgetsByUserIdAsync(Guid userId, Guid? categoryId = null, DateOnly? from = null, DateOnly? to = null)
        {
            var query = _context.Budgets.Where(b => b.UserId == userId);
            if (categoryId.HasValue)
                query = query.Where(b => b.CategoryId == categoryId.Value);
            if (from.HasValue)
                query = query.Where(b => b.StartDate >= from.Value);
            if (to.HasValue)
                query = query.Where(b => b.EndDate <= to.Value);
            return await query.ToListAsync();
        }
        public async Task<Budget> GetBudgetByIdAsync(Guid budgetId)
        {
            return await _context.Budgets.FirstOrDefaultAsync(b => b.BudgetId == budgetId);
        }
        public async Task<Budget> AddBudgetAsync(Budget budget)
        {
            _context.Budgets.Add(budget);
            await _context.SaveChangesAsync();
            return budget;
        }
        public async Task<bool> UpdateBudgetAsync(Budget budget)
        {
            _context.Budgets.Update(budget);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> DeleteBudgetAsync(Budget budget)
        {
            _context.Budgets.Remove(budget);
            return await _context.SaveChangesAsync() > 0;
        }
    }
} 
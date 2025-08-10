using Microsoft.EntityFrameworkCore;
using Repositories.Base;
using Repositories.Models;

namespace Repositories.Repositories
{
	public class ExpenseRepo : GenericRepository<Expense>, IExpenseRepo
	{
		public ExpenseRepo(YuniBuddyContext context) : base(context) { }

		public async Task<List<Expense>> GetUserExpensesAsync(Guid userId)
		{
			return await _context.Expenses
				.Where(e => e.UserId == userId)
				.Include(e => e.ExC)
				.Include(e => e.Account)
				.OrderByDescending(e => e.CreatedDate)
				.ToListAsync();
		}

		public async Task<List<Expense>> GetByUserIdAsync(Guid userId)
		{
			return await GetUserExpensesAsync(userId);
		}

		public async Task<List<Expense>> GetUserExpensesForCurrentMonthAsync(Guid userId)
		{
			var now = DateTime.UtcNow;
			var startOfMonth = new DateTime(now.Year, now.Month, 1);
			var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

			return await _context.Expenses
				.Where(e => e.UserId == userId && 
						   e.CreatedDate >= startOfMonth && 
						   e.CreatedDate <= endOfMonth)
				.Include(e => e.ExC)
				.Include(e => e.Account)
				.OrderByDescending(e => e.CreatedDate)
				.ToListAsync();
		}

		public async Task<List<Expense>> GetUserExpensesForMonthAsync(Guid userId, int year, int month)
		{
			var startOfMonth = new DateTime(year, month, 1);
			var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

			return await _context.Expenses
				.Where(e => e.UserId == userId && 
						   e.CreatedDate >= startOfMonth && 
						   e.CreatedDate <= endOfMonth)
				.Include(e => e.ExC)
				.OrderByDescending(e => e.CreatedDate)
				.ToListAsync();
		}

		public async Task<List<Expense>> GetRecentExpensesAsync(Guid userId, int limit = 10)
		{
			return await _context.Expenses
				.Where(e => e.UserId == userId)
				.Include(e => e.ExC)
				.OrderByDescending(e => e.CreatedDate)
				.Take(limit)
				.ToListAsync();
		}

		public async Task<List<Expense>> GetCategoryExpensesAsync(Guid userId, Guid categoryId, int limit = 10)
		{
			return await _context.Expenses
				.Where(e => e.UserId == userId && e.ExCid == categoryId)
				.Include(e => e.ExC)
				.Include(e => e.Account)
				.OrderByDescending(e => e.CreatedDate)
				.Take(limit)
				.ToListAsync();
		}

		public async Task<decimal> GetTotalSpentInCategoryAsync(Guid userId, Guid categoryId, DateTime startDate, DateTime endDate)
		{
			return await _context.Expenses
				.Where(e => e.UserId == userId && 
						   e.ExCid == categoryId && 
						   e.CreatedDate >= startDate && 
						   e.CreatedDate <= endDate &&
						   e.ExC != null && e.ExC.Type == "expense") // Only expenses, not income
				.SumAsync(e => e.Amount);
		}

		public async Task<List<Expense>> GetExpensesByDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate)
		{
			return await _context.Expenses
				.Where(e => e.UserId == userId && 
						   e.CreatedDate >= startDate && 
						   e.CreatedDate <= endDate)
				.Include(e => e.ExC)
				.Include(e => e.Account)
				.OrderByDescending(e => e.CreatedDate)
				.ToListAsync();
		}
	}
}

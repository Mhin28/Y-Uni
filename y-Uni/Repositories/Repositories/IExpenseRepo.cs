using Repositories.Base;
using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
	public interface IExpenseRepo : IGenericRepository<Expense>
	{
		/// <summary>
		/// Get all expenses for a specific user
		/// </summary>
		/// <param name="userId">User ID</param>
		/// <returns>List of user expenses</returns>
		Task<List<Expense>> GetUserExpensesAsync(Guid userId);

		/// <summary>
		/// Get user expenses for current month
		/// </summary>
		/// <param name="userId">User ID</param>
		/// <returns>List of current month expenses</returns>
		Task<List<Expense>> GetUserExpensesForCurrentMonthAsync(Guid userId);

		/// <summary>
		/// Get user expenses for a specific month and year
		/// </summary>
		/// <param name="userId">User ID</param>
		/// <param name="year">Year</param>
		/// <param name="month">Month (1-12)</param>
		/// <returns>List of expenses for the specified month</returns>
		Task<List<Expense>> GetUserExpensesForMonthAsync(Guid userId, int year, int month);

		/// <summary>
		/// Get recent expenses for a user
		/// </summary>
		/// <param name="userId">User ID</param>
		/// <param name="limit">Number of expenses to return</param>
		/// <returns>List of recent expenses</returns>
		Task<List<Expense>> GetRecentExpensesAsync(Guid userId, int limit = 10);

		/// <summary>
		/// Get expenses for a specific category
		/// </summary>
		/// <param name="userId">User ID</param>
		/// <param name="categoryId">Category ID</param>
		/// <param name="limit">Number of expenses to return</param>
		/// <returns>List of category expenses</returns>
		Task<List<Expense>> GetCategoryExpensesAsync(Guid userId, Guid categoryId, int limit = 10);

		/// <summary>
		/// Get total spent amount in a category for a date range
		/// </summary>
		/// <param name="userId">User ID</param>
		/// <param name="categoryId">Category ID</param>
		/// <param name="startDate">Start date</param>
		/// <param name="endDate">End date</param>
		/// <returns>Total spent amount</returns>
		Task<decimal> GetTotalSpentInCategoryAsync(Guid userId, Guid categoryId, DateTime startDate, DateTime endDate);

		/// <summary>
		/// Get expenses by date range
		/// </summary>
		/// <param name="userId">User ID</param>
		/// <param name="startDate">Start date</param>
		/// <param name="endDate">End date</param>
		/// <returns>List of expenses in date range</returns>
		Task<List<Expense>> GetExpensesByDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate);
	}
}

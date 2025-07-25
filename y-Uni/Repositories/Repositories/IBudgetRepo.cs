using Repositories.Base;
using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
	public interface IBudgetRepo : IGenericRepository<Budget>
	{
		/// <summary>
		/// Get all budgets for a specific user
		/// </summary>
		/// <param name="userId">User ID</param>
		/// <returns>List of user budgets</returns>
		Task<List<Budget>> GetUserBudgetsAsync(Guid userId);

		/// <summary>
		/// Get active budgets for a user at a specific date
		/// </summary>
		/// <param name="userId">User ID</param>
		/// <param name="date">Date to check</param>
		/// <returns>List of active budgets</returns>
		Task<List<Budget>> GetActiveBudgetsForUserAsync(Guid userId, DateTime date);

		/// <summary>
		/// Get budgets for a specific month and year
		/// </summary>
		/// <param name="userId">User ID</param>
		/// <param name="year">Year</param>
		/// <param name="month">Month</param>
		/// <returns>List of budgets active in the specified month</returns>
		Task<List<Budget>> GetUserBudgetsForMonthAsync(Guid userId, int year, int month);

		/// <summary>
		/// Get budget by category and user
		/// </summary>
		/// <param name="userId">User ID</param>
		/// <param name="categoryId">Category ID</param>
		/// <returns>Budget for the specified category</returns>
		Task<Budget?> GetBudgetByCategoryAsync(Guid userId, Guid categoryId);
	}
}
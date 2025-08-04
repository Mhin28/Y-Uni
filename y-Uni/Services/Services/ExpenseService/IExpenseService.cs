using Repositories.Models;
using Repositories.ViewModels.ExpenseModel;
using Repositories.ViewModels.ResultModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Services.ExpenseService
{
	public interface IExpenseService
	{
		Task<ResultModel> GetAllAsync();
		Task<ResultModel> GetByIdAsync(Guid id);
		Task<ResultModel> AddAsync(PostExpenseModel model);
		Task<ResultModel> UpdateAsync(ExpenseModel model);
		Task<ResultModel> DeleteAsync(Guid id);
		
		/// <summary>
		/// Get all expense transactions for the current month for the authenticated user with category information
		/// </summary>
		/// <returns>All expense transactions for current month with category names</returns>
		Task<ResultModel> GetRecentTransactionsAsync();
	}
}

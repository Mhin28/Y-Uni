using Repositories.Models;
using Repositories.ViewModels.BudgetModel;
using Repositories.ViewModels.ResultModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Services.BudgetService
{
	public interface IBudgetService
	{
		Task<ResultModel> GetAllAsync();
		Task<ResultModel> GetByIdAsync(Guid id);
		Task<ResultModel> AddAsync(PostBudgetModel model);
		Task<ResultModel> UpdateAsync(BudgetModel model);
		Task<ResultModel> DeleteAsync(Guid id);
		
		// Budget Lock/Carry-over functionality
		Task<ResultModel> GetUserBudgetsForMonthAsync(Guid userId, int year, int month);
		Task<ResultModel> CopyBudgetsToNextMonthAsync(Guid userId, List<Guid> budgetIds, int targetYear, int targetMonth);
		Task<ResultModel> CreateBudgetFromPreviousMonthAsync(Guid userId, Guid previousBudgetId, int targetYear, int targetMonth);
		Task<ResultModel> GetBudgetCarryOverSummaryAsync(Guid userId, int fromYear, int fromMonth, int toYear, int toMonth);
	}
}
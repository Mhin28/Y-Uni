using Repositories.Models;
using Repositories.ViewModels.ExpensesCategoryModel;
using Repositories.ViewModels.ResultModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Services.ExpensesCategoryService
{
	public interface IExpensesCategoryService
	{
		Task<ResultModel> GetAllAsync();
		Task<ResultModel> GetByIdAsync(Guid id);
		Task<ResultModel> AddAsync(PostExpensesCategoryModel model);
		Task<ResultModel> UpdateAsync(ExpensesCategoryModel model);
		Task<ResultModel> DeleteAsync(Guid id);
	}
}

using Repositories.Models;
using Repositories.ViewModels.FinancialAccountModel;
using Repositories.ViewModels.ResultModels;
using System;
using System.Threading.Tasks;

namespace Services.Services.FinancialAccountService
{
	public interface IFinancialAccountService
	{
		Task<ResultModel> GetAllAsync();
		Task<ResultModel> GetByIdAsync(Guid id);
		Task<ResultModel> AddAsync(PostFinancialAccModel model);
		Task<ResultModel> UpdateAsync(FinancialAccModel model);
		Task<ResultModel> DeleteAsync(Guid id);
		Task<ResultModel> GetByUserIdAsync(Guid userId);
	}
}

using Repositories.ViewModels.DiscountModel;
using Repositories.ViewModels.ResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.DiscountService
{
	public interface IDiscountService
	{
		Task<ResultModel> GetAllAsync();
		Task<ResultModel> GetByIdAsync(Guid id);
		Task<ResultModel> AddAsync(PostDiscountModel model);
		Task<ResultModel> UpdateAsync(DiscountModel model);
		Task<ResultModel> DeleteAsync(Guid id);
		Task<ResultModel> GetActiveAsync();
		Task<ResultModel> GetByNameAsync(string name);
	}
}

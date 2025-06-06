using Repositories.Models;
using Repositories.ViewModels.PaymentMethodModel;
using Repositories.ViewModels.ResultModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Services.PaymentMethodService
{
	public interface IPaymentMethodService
	{
		Task<ResultModel> GetAllAsync();
		Task<ResultModel> GetByIdAsync(Guid id);
		Task<ResultModel> AddAsync(PostPaymentMethodModel model);
		Task<ResultModel> UpdateAsync(PaymentMethodModel model);
		Task<ResultModel> DeleteAsync(Guid id);
	}
}

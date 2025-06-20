using Repositories.ViewModels.PaymentGatewayModel;
using Repositories.ViewModels.PaymentMethodModel;
using Repositories.ViewModels.ResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.PaymentGatewayService
{
	public interface IPaymentGatewayService
	{
		Task<ResultModel> GetAllAsync();
		Task<ResultModel> GetByIdAsync(Guid id);
		Task<ResultModel> AddAsync(PostPaymentGatewayModel model);
		Task<ResultModel> UpdateAsync(PaymentGatewayModel model);
		Task<ResultModel> DeleteAsync(Guid id);
	}
}

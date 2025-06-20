using Repositories.Models;
using Repositories.ViewModels.InvoiceModel;
using Repositories.ViewModels.ResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.InvoiceService
{
	public interface IInvoiceService
	{
		Task<ResultModel> GetAllAsync();
		Task<ResultModel> GetByIdAsync(Guid id);
		Task<ResultModel> AddAsync(PostInvoiceModel model);
		Task<ResultModel> UpdateAsync(InvoiceModel model);
		Task<ResultModel> DeleteAsync(Guid id);
		Task<ResultModel> GetInvoicesByUserIdAsync(Guid customerId);
		Task<ResultModel> GetInvoicesByPaymentMethodAsync(Guid paymentMethodId);
		Task<ResultModel> GetInvoicesByMembershipPlanAsync(Guid membershipPlanId);
		Task<ResultModel> GetInvoicesByDiscountAsync(Guid discountId);
	}
}

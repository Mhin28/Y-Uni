using Repositories.Base;
using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
	public interface IInvoiceRepo : IGenericRepository<Invoice>
	{
		Task<List<Invoice>> GetInvoicesByUserIdAsync(Guid userId);
		Task<List<Invoice>> GetInvoicesByPaymentMethodAsync(Guid paymentMethodId);
		Task<List<Invoice>> GetInvoicesByMembershipPlanAsync(Guid membershipPlanId);
		Task<List<Invoice>> GetInvoicesByDiscountAsync(Guid discountId);
	}
}

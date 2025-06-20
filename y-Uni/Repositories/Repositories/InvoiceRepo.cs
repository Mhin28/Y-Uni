using Microsoft.EntityFrameworkCore;
using Repositories.Base;
using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
	public class InvoiceRepo : GenericRepository<Invoice>, IInvoiceRepo
	{
		public InvoiceRepo(YUniContext context) : base(context)
		{
		}

		public async Task<List<Invoice>> GetInvoicesByUserIdAsync(Guid userId)
		{
			return await _context.Invoices
				.Where(i => i.UserId == userId)
				.ToListAsync();
		}
		public async Task<List<Invoice>> GetInvoicesByPaymentMethodAsync(Guid paymentMethodId)
		{
			return await _context.Invoices
				.Where(i => i.PaymentMethodId == paymentMethodId)
				.ToListAsync();
		}
		public async Task<List<Invoice>> GetInvoicesByMembershipPlanAsync(Guid membershipPlanId)
		{
			return await _context.Invoices
				.Where(i => i.MembershipPlanId == membershipPlanId)
				.ToListAsync();
		}
		public async Task<List<Invoice>> GetInvoicesByDiscountAsync(Guid discountId)
		{
			return await _context.Invoices
				.Where(i => i.DiscountId == discountId)
				.ToListAsync();
		}
	}
	
}

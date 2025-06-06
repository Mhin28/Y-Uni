using Repositories.Base;
using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
	public class PaymentMethodRepo : GenericRepository<PaymentMethod>, IPaymentMethodRepo
	{
		public PaymentMethodRepo(YUniContext context) : base(context) { }

		public async Task<List<PaymentMethod>> GetActiveAsync()
		{
			return await GetAllAsync(pm => pm.IsActive == true);
		}
	}
}

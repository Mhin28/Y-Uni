using Repositories.Base;
using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
	public class FinancialAccountRepo : GenericRepository<FinancialAccount>, IFinancialAccountRepo
	{
		public FinancialAccountRepo(YUniContext context) : base(context) { }

		public async Task<List<FinancialAccount>> GetByUserIdAsync(Guid userId)
		{
			return await GetAllAsync(fa => fa.UserId == userId);
		}
	}
}

using Microsoft.EntityFrameworkCore;
using Repositories.Base;
using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
	public class FinancialAccountRepo : GenericRepository<FinancialAccount>, IFinancialAccountRepo
	{
		public FinancialAccountRepo(YuniBuddyContext context) : base(context) { }

		public async Task<List<FinancialAccount>> GetByUserIdAsync(Guid userId)
		{
			return await _context.FinancialAccounts
				.Where(fa => fa.UserId == userId)
				.ToListAsync();
		}

		public async Task<List<FinancialAccount>> GetUserAccountsAsync(Guid userId)
		{
			return await _context.FinancialAccounts
				.Where(fa => fa.UserId == userId)
				.ToListAsync();
		}

		public async Task<FinancialAccount?> GetDefaultAccountAsync(Guid userId)
		{
			return await _context.FinancialAccounts
				.Where(x => x.UserId == userId && x.IsDefault == true)
				.FirstOrDefaultAsync();
		}

		public async Task<FinancialAccount> UpdateBalanceAsync(Guid accountId, decimal newBalance)
		{
			var account = await _context.FinancialAccounts.FindAsync(accountId);
			if (account == null)
				throw new ArgumentException("Account not found", nameof(accountId));

			account.Balance = newBalance;
			await _context.SaveChangesAsync();
			return account;
		}
	}
}

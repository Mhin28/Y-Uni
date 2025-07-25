using Repositories.Base;
using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
	public interface IFinancialAccountRepo : IGenericRepository<FinancialAccount>
	{
		/// <summary>
		/// Get all accounts for a specific user
		/// </summary>
		/// <param name="userId">User ID</param>
		/// <returns>List of user accounts</returns>
		Task<List<FinancialAccount>> GetByUserIdAsync(Guid userId);

		/// <summary>
		/// Get all accounts for a specific user (alias for consistency)
		/// </summary>
		/// <param name="userId">User ID</param>
		/// <returns>List of user accounts</returns>
		Task<List<FinancialAccount>> GetUserAccountsAsync(Guid userId);

		/// <summary>
		/// Get the default account for a user
		/// </summary>
		/// <param name="userId">User ID</param>
		/// <returns>Default account or null if not found</returns>
		Task<FinancialAccount?> GetDefaultAccountAsync(Guid userId);

		/// <summary>
		/// Update account balance
		/// </summary>
		/// <param name="accountId">Account ID</param>
		/// <param name="newBalance">New balance amount</param>
		/// <returns>Updated account</returns>
		Task<FinancialAccount> UpdateBalanceAsync(Guid accountId, decimal newBalance);
	}
}

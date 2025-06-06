using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.ViewModels.FinancialAccountModel
{
	public class FinancialAccModel
	{
		public Guid AccountId { get; set; }

		public string AccountName { get; set; }

		public decimal? Balance { get; set; }

		public string CurrencyCode { get; set; }

		public Guid? UserId { get; set; }

		public bool? IsDefault { get; set; }
	}
}

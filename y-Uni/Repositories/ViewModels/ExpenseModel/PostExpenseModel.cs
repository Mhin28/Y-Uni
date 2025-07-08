using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.ViewModels.ExpenseModel
{
	public class PostExpenseModel
	{
		public decimal Amount { get; set; }

		public string Description { get; set; }

		//public DateTime? CreatedDate { get; set; }

		public Guid? ExCid { get; set; }

		public Guid? AccountId { get; set; }

		public Guid? UserId { get; set; }
	}
}

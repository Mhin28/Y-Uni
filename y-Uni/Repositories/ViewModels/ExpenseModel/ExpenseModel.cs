using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.ViewModels.ExpenseModel
{
	public class ExpenseModel
	{
		public Guid ExpensesId { get; set; }

		public decimal Amount { get; set; }

		public string Description { get; set; }

		public DateTime? CreatedDate { get; set; }

		public string Type { get; set; }

		public string Frequency { get; set; }

		public DateOnly? NextDueDate { get; set; }

		public Guid? ExCid { get; set; }

		public Guid? AccountId { get; set; }

		public Guid? UserId { get; set; }
	}
}

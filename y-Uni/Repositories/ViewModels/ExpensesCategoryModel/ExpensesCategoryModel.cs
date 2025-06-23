using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.ViewModels.ExpensesCategoryModel
{
	public class ExpensesCategoryModel
	{
		public Guid ExCid { get; set; }

		public string? CategoryName { get; set; }

		public string? Description { get; set; }
	}
}

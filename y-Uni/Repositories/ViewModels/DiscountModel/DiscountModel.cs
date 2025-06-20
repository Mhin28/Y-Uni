using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.ViewModels.DiscountModel
{
	public class DiscountModel
	{
		public Guid DiscountId { get; set; }

		public string? DiscountName { get; set; }

		public decimal? DiscountPercentage { get; set; }

		public bool? IsActive { get; set; }

	}
}

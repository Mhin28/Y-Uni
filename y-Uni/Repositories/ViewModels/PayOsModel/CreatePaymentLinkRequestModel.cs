using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.ViewModels.PayOsModel
{
	public class CreatePaymentLinkRequestModel
	{
		// Invoice information
		public Guid? UserId { get; set; }
		public Guid? DiscountId { get; set; }
		public Guid? PaymentMethodId { get; set; }
		public Guid? MembershipPlanId { get; set; }
		public decimal Amount { get; set; }
		public decimal? TaxAmount { get; set; }
		public decimal? DiscountAmount { get; set; }
		public decimal? TotalAmount { get; set; }

		// PayOS information
		public string ProductName { get; set; }
		public string Description { get; set; }
		public string ReturnUrl { get; set; }
		public string CancelUrl { get; set; }
	}
}

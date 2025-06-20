using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.ViewModels.InvoiceModel
{
	public class PostInvoiceModel
	{

		public decimal Amount { get; set; }

		public decimal? TaxAmount { get; set; }

		public decimal? DiscountAmount { get; set; }

		public decimal? TotalAmount { get; set; }

		public Guid? PaymentMethodId { get; set; }

		public string GatewayTransactionId { get; set; }

		public DateTime? CreatedDate { get; set; }

		public DateTime? UpdatedDate { get; set; }

		public string InvoiceStatus { get; set; }

		public Guid? UserId { get; set; }

		public Guid? DiscountId { get; set; }

		public Guid? MembershipPlanId { get; set; }
	}
}

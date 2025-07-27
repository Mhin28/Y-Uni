using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.ViewModels.PayOsModel
{
	public class PaymentLinkResponseModel
	{
		public string PaymentUrl { get; set; }
		public string PaymentLinkId { get; set; }
		public long OrderCode { get; set; }
		public string QrCode { get; set; }
		public Guid InvoiceId { get; set; }
	}

}

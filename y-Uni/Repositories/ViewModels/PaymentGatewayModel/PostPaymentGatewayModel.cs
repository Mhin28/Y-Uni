using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.ViewModels.PaymentGatewayModel
{
	public class PostPaymentGatewayModel
	{
		public string GatewayName { get; set; }

		public string ApiKey { get; set; }

		public bool? IsActive { get; set; }
	}
}

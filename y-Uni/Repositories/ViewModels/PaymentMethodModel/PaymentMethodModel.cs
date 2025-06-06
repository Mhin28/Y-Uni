using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.ViewModels.PaymentMethodModel
{
	public class PaymentMethodModel
	{
		public Guid MethodId { get; set; }

		public string MethodName { get; set; }

		public bool? IsActive { get; set; }

	}
}

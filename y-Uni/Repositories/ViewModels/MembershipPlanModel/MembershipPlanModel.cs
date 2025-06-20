using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.ViewModels.MembershipPlanModel
{
	public class MembershipPlanModel
	{
		public Guid MPid { get; set; }

		public string PlanName { get; set; }

		public decimal Price { get; set; }

		public int DurationDays { get; set; }
	}
}

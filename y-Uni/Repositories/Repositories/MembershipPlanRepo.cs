using Microsoft.EntityFrameworkCore;
using Repositories.Base;
using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
	public class MembershipPlanRepo : GenericRepository<MembershipPlan>, IMembershipPlanRepo
	{
		public MembershipPlanRepo(YUniContext context) : base(context)
		{
		}
		public async Task<List<MembershipPlan>> GetMembershipPlansByNameAsync(string planName)
		{
			return await _context.MembershipPlans
				.Where(mp => mp.PlanName.Contains(planName))
				.ToListAsync();
		}
	}
}

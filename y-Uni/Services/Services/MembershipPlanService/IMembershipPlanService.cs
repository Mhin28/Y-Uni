using Repositories.ViewModels.MembershipPlanModel;
using Repositories.ViewModels.ResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.MembershipPlanService
{
	public interface IMembershipPlanService
	{
		//crud
		Task<ResultModel> GetAllAsync();
		Task<ResultModel> GetByIdAsync(Guid id);
		Task<ResultModel> AddAsync(PostMembershipPlanModel model);
		Task<ResultModel> UpdateAsync(MembershipPlanModel model);
		Task<ResultModel> DeleteAsync(Guid id);
		Task<ResultModel> GetByNameAsync(string name);
	}
}

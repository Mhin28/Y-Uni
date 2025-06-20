using Repositories.Models;
using Repositories.Repositories;
using Repositories.ViewModels.MembershipPlanModel;
using Repositories.ViewModels.ResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.MembershipPlanService
{
	public class MembershipPlanService : IMembershipPlanService
	{
		private readonly IMembershipPlanRepo _repo;
		public MembershipPlanService(IMembershipPlanRepo repo)
		{
			_repo = repo;
		}

		public async Task<ResultModel> GetAllAsync()
		{
			var result = new ResultModel();
			try
			{
				var membershipPlans = await _repo.GetAllAsync();
				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.OK;
				result.Data = membershipPlans;
			}
			catch (Exception ex)
			{
				result.IsSuccess = false;
				result.Code = (int)HttpStatusCode.InternalServerError;
				result.Message = ex.Message;
			}
			return result;
		}

		public async Task<ResultModel> GetByIdAsync(Guid id)
		{
			var result = new ResultModel();
			try
			{
				var membershipPlan = await _repo.GetByIdAsync(id);
				if (membershipPlan == null)
				{
					result.IsSuccess = false;
					result.Code = (int)HttpStatusCode.NotFound;
					result.Message = "Membership plan not found.";
					return result;
				}
				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.OK;
				result.Data = membershipPlan;
			}
			catch (Exception ex)
			{
				result.IsSuccess = false;
				result.Code = (int)HttpStatusCode.InternalServerError;
				result.Message = ex.Message;
			}
			return result;
		}

		public async Task<ResultModel> AddAsync(PostMembershipPlanModel model)
		{
			var result = new ResultModel() 
			{
				IsSuccess = false,
				Code = (int)HttpStatusCode.BadRequest,
				Message = "Invalid request"
			};
			try
			{
				var membershipPlan = new MembershipPlan
				{
					MPid = Guid.NewGuid(),
					PlanName = model.PlanName,
					DurationDays = model.DurationDays,
					Price = model.Price

				};
				await _repo.CreateAsync(membershipPlan);
				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.Created;
				result.Data = membershipPlan;
				result.Message = "Membership plan created successfully.";
			}
			catch (Exception ex)
			{
				result.IsSuccess = false;
				result.Code = (int)HttpStatusCode.InternalServerError;
				result.Message = ex.Message;
			}
			return result;
		}

		public async Task<ResultModel> UpdateAsync(MembershipPlanModel model)
		{
			var result = new ResultModel() 
			{
				IsSuccess = false,
				Code = (int)HttpStatusCode.BadRequest,
				Message = "Invalid request"
			};
			try
			{
				var membershipPlan = await _repo.GetByIdAsync(model.MPid);
				if (membershipPlan == null)
				{
					result.IsSuccess = false;
					result.Code = (int)HttpStatusCode.NotFound;
					result.Message = "Membership plan not found.";
					return result;
				}
				if (!string.IsNullOrEmpty(model.PlanName))
					membershipPlan.PlanName = model.PlanName;

				if (model.DurationDays.HasValue)
					membershipPlan.DurationDays = model.DurationDays.Value;

				if (model.Price.HasValue)
					membershipPlan.Price = model.Price.Value;
				await _repo.UpdateAsync(membershipPlan);
				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.OK;
				result.Data = membershipPlan;
				result.Message = "Membership plan updated successfully.";
			}
			catch (Exception ex)
			{
				result.IsSuccess = false;
				result.Code = (int)HttpStatusCode.InternalServerError;
				result.Message = ex.Message;
			}
			return result;
		}

		public async Task<ResultModel> DeleteAsync(Guid id)
		{
			var result = new ResultModel();
			try
			{
				var membershipPlan = await _repo.GetByIdAsync(id);
				if (membershipPlan == null)
				{
					result.IsSuccess = false;
					result.Code = (int)HttpStatusCode.NotFound;
					result.Message = "Membership plan not found.";
					return result;
				}
				await _repo.RemoveAsync(membershipPlan);
				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.OK;
				result.Message = "Membership plan deleted successfully.";
			}
			catch (Exception ex)
			{
				result.IsSuccess = false;
				result.Code = (int)HttpStatusCode.InternalServerError;
				result.Message = ex.Message;
			}
			return result;
		}

		public async Task<ResultModel> GetByNameAsync(string name)
		{
			var result = new ResultModel();
			try
			{
				var membershipPlan = await _repo.GetMembershipPlansByNameAsync(name);
				if (membershipPlan == null)
				{
					result.IsSuccess = false;
					result.Code = (int)HttpStatusCode.NotFound;
					result.Message = "Membership plan not found.";
					return result;
				}
				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.OK;
				result.Data = membershipPlan;
			}
			catch (Exception ex)
			{
				result.IsSuccess = false;
				result.Code = (int)HttpStatusCode.InternalServerError;
				result.Message = ex.Message;
			}
			return result;
		}
	}
}

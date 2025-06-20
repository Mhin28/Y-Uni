using Microsoft.AspNetCore.Mvc;
using Repositories.ViewModels.MembershipPlanModel;
using Services.Services.MembershipPlanService;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class MembershipPlanController : ControllerBase
	{
		private readonly IMembershipPlanService _membershipPlanService;

		public MembershipPlanController(IMembershipPlanService membershipPlanService)
		{
			_membershipPlanService = membershipPlanService;
		}

		#region CRUD Operations

		// GET: api/MembershipPlan
		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var result = await _membershipPlanService.GetAllAsync();
			return StatusCode(result.Code, result);
		}

		// GET: api/MembershipPlan/{id}
		[HttpGet("{id:guid}")]
		public async Task<IActionResult> GetById(Guid id)
		{
			var result = await _membershipPlanService.GetByIdAsync(id);
			return StatusCode(result.Code, result);
		}

		// GET: api/MembershipPlan/search/{name}
		[HttpGet("search/{name}")]
		public async Task<IActionResult> GetByName(string name)
		{
			var result = await _membershipPlanService.GetByNameAsync(name);
			return StatusCode(result.Code, result);
		}

		// POST: api/MembershipPlan
		[HttpPost]
		public async Task<IActionResult> Create([FromBody] PostMembershipPlanModel model)
		{
			var result = await _membershipPlanService.AddAsync(model);
			return StatusCode(result.Code, result);
		}

		// PUT: api/MembershipPlan
		[HttpPut]
		public async Task<IActionResult> Update([FromBody] MembershipPlanModel model)
		{
			var result = await _membershipPlanService.UpdateAsync(model);
			return StatusCode(result.Code, result);
		}

		// DELETE: api/MembershipPlan/{id}
		[HttpDelete("{id:guid}")]
		public async Task<IActionResult> Delete(Guid id)
		{
			var result = await _membershipPlanService.DeleteAsync(id);
			return StatusCode(result.Code, result);
		}

		#endregion
	}
}

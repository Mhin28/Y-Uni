using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories.ViewModels.AssignmentModel;
using Services.Services.AssignmentService;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class AssignmentController : ControllerBase
	{
		private readonly IAssignmentService _assignmentService;

		public AssignmentController(IAssignmentService assignmentService)
		{
			_assignmentService = assignmentService;
		}

		private Guid GetUserIdFromToken()
		{
			var userIdClaim = User.FindFirst("userid")?.Value;
			return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
		}

		#region CRUD Operations

		// GET: api/Assignment - Get current user's assignments
		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var userId = GetUserIdFromToken();
			if (userId == Guid.Empty)
				return Unauthorized("Invalid token");

			var result = await _assignmentService.GetByUserIdAsync(userId);
			return StatusCode(result.Code, result);
		}

		// GET: api/Assignment/{id}
		[HttpGet("{id:guid}")]
		public async Task<IActionResult> GetById(Guid id)
		{
			var result = await _assignmentService.GetByIdAsync(id);
			return StatusCode(result.Code, result);
		}

		// POST: api/Assignment
		[HttpPost]
		public async Task<IActionResult> Create([FromBody] PostAssignmentModel model)
		{
			var userId = GetUserIdFromToken();
			if (userId == Guid.Empty)
				return Unauthorized("Invalid token");

			var result = await _assignmentService.AddAsync(model, userId);
			return StatusCode(result.Code, result);
		}

		// PUT: api/Assignment
		[HttpPut]
		public async Task<IActionResult> Update([FromBody] AssignmentModel model)
		{
			var result = await _assignmentService.UpdateAsync(model);
			return StatusCode(result.Code, result);
		}

		// DELETE: api/Assignment/{id}
		[HttpDelete("{id:guid}")]
		public async Task<IActionResult> Delete(Guid id)
		{
			var result = await _assignmentService.DeleteAsync(id);
			return StatusCode(result.Code, result);
		}

		#endregion

		#region Filtering Endpoints

		// GET: api/Assignment/my-assignments - Get current user's assignments
		[HttpGet("my-assignments")]
		public async Task<IActionResult> GetMyAssignments()
		{
			var userId = GetUserIdFromToken();
			if (userId == Guid.Empty)
				return Unauthorized("Invalid token");

			var result = await _assignmentService.GetByUserIdAsync(userId);
			return StatusCode(result.Code, result);
		}

		// GET: api/Assignment/subject/{subjectId}
		[HttpGet("subject/{subjectId:guid}")]
		public async Task<IActionResult> GetBySubject(Guid subjectId)
		{
			var result = await _assignmentService.GetBySubjectAsync(subjectId);
			return StatusCode(result.Code, result);
		}

		// GET: api/Assignment/upcoming?dueDate=2023-12-31
		[HttpGet("upcoming")]
		public async Task<IActionResult> GetUpcoming([FromQuery] DateTime? dueDate = null)
		{
			var userId = GetUserIdFromToken();
			if (userId == Guid.Empty)
				return Unauthorized("Invalid token");

			var result = await _assignmentService.GetUpcomingByUserIdAsync(userId, dueDate);
			return StatusCode(result.Code, result);
		}

		// GET: api/Assignment/status/{status}
		[HttpGet("status/{status}")]
		public async Task<IActionResult> GetByStatus(string status)
		{
			var userId = GetUserIdFromToken();
			if (userId == Guid.Empty)
				return Unauthorized("Invalid token");

			var result = await _assignmentService.GetByStatusAsync(userId, status);
			return StatusCode(result.Code, result);
		}

		// PUT: api/Assignment/status/{id}/{status}
		[HttpPut("status/{id:guid}/{status}")]
		public async Task<IActionResult> UpdateStatus(Guid id, string status)
		{
			var result = await _assignmentService.UpdateStatusAsync(id, status);
			return StatusCode(result.Code, result);
		}

		// PUT: api/Assignment/complete/{id}
		[HttpPut("complete/{id:guid}")]
		public async Task<IActionResult> CompleteAssignment(Guid id)
		{
			var result = await _assignmentService.CompleteAssignmentAsync(id);
			return StatusCode(result.Code, result);
		}

		#endregion
	}
} 
using Microsoft.AspNetCore.Mvc;
using Repositories.ViewModels.AssignmentModel;
using Services.Services.AssignmentService;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AssignmentController : ControllerBase
	{
		private readonly IAssignmentService _assignmentService;

		public AssignmentController(IAssignmentService assignmentService)
		{
			_assignmentService = assignmentService;
		}

		#region CRUD Operations

		// GET: api/Assignment
		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var result = await _assignmentService.GetAllAsync();
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
			var result = await _assignmentService.AddAsync(model);
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

		// GET: api/Assignment/user/{userId}
		[HttpGet("user/{userId:guid}")]
		public async Task<IActionResult> GetByUserId(Guid userId)
		{
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

		// GET: api/Assignment/upcoming/{userId}?dueDate=2023-12-31
		[HttpGet("upcoming/{userId:guid}")]
		public async Task<IActionResult> GetUpcoming(Guid userId, [FromQuery] DateTime? dueDate = null)
		{
			var result = await _assignmentService.GetUpcomingByUserIdAsync(userId, dueDate);
			return StatusCode(result.Code, result);
		}

		// GET: api/Assignment/status/{userId}/{status}
		[HttpGet("status/{userId:guid}/{status}")]
		public async Task<IActionResult> GetByStatus(Guid userId, string status)
		{
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
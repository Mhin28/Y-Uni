using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories.ViewModels.EventModel;
using Services.Services.EventService;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class EventController : ControllerBase
	{
		private readonly IEventService _eventService;

		public EventController(IEventService eventService)
		{
			_eventService = eventService;
		}

		private Guid GetUserIdFromToken()
		{
			var userIdClaim = User.FindFirst("userid")?.Value;
			return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
		}

		#region CRUD Operations

		// GET: api/Event - Get current user's events
		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var userId = GetUserIdFromToken();
			if (userId == Guid.Empty)
				return Unauthorized("Invalid token");

			var result = await _eventService.GetByUserIdAsync(userId);
			return StatusCode(result.Code, result);
		}

		// GET: api/Event/{id}
		[HttpGet("{id:guid}")]
		public async Task<IActionResult> GetById(Guid id)
		{
			var result = await _eventService.GetByIdAsync(id);
			return StatusCode(result.Code, result);
		}

		// POST: api/Event
		[HttpPost]
		public async Task<IActionResult> Create([FromBody] PostEventModel model)
		{
			var userId = GetUserIdFromToken();
			if (userId == Guid.Empty)
				return Unauthorized("Invalid token");

			var result = await _eventService.AddAsync(model, userId);
			return StatusCode(result.Code, result);
		}

		// PUT: api/Event
		[HttpPut]
		public async Task<IActionResult> Update([FromBody] UpdateEventModel model)
		{
			var result = await _eventService.UpdateAsync(model);
			return StatusCode(result.Code, result);
		}

		// DELETE: api/Event/{id}
		[HttpDelete("{id:guid}")]
		public async Task<IActionResult> Delete(Guid id)
		{
			var result = await _eventService.DeleteAsync(id);
			return StatusCode(result.Code, result);
		}

		#endregion

		#region Filtering Endpoints

		// GET: api/Event/my-events - Get current user's events
		[HttpGet("my-events")]
		public async Task<IActionResult> GetMyEvents()
		{
			var userId = GetUserIdFromToken();
			if (userId == Guid.Empty)
				return Unauthorized("Invalid token");

			var result = await _eventService.GetByUserIdAsync(userId);
			return StatusCode(result.Code, result);
		}

		// GET: api/Event/upcoming
		[HttpGet("upcoming")]
		public async Task<IActionResult> GetUpcoming([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
		{
			var userId = GetUserIdFromToken();
			if (userId == Guid.Empty)
				return Unauthorized("Invalid token");

			var result = await _eventService.GetUpcomingByUserIdAsync(userId, startDate, endDate);
			return StatusCode(result.Code, result);
		}

		// GET: api/Event/category/{categoryId}
		[HttpGet("category/{categoryId:guid}")]
		public async Task<IActionResult> GetByCategory(Guid categoryId)
		{
			var result = await _eventService.GetByCategoryAsync(categoryId);
			return StatusCode(result.Code, result);
		}

		#endregion
	}
} 
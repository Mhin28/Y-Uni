using Microsoft.AspNetCore.Mvc;
using Repositories.ViewModels.EventModel;
using Services.Services.EventService;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class EventController : ControllerBase
	{
		private readonly IEventService _eventService;

		public EventController(IEventService eventService)
		{
			_eventService = eventService;
		}

		#region CRUD Operations

		// GET: api/Event
		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var result = await _eventService.GetAllAsync();
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
			var result = await _eventService.AddAsync(model);
			return StatusCode(result.Code, result);
		}

		// PUT: api/Event
		[HttpPut]
		public async Task<IActionResult> Update([FromBody] EventModel model)
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

		// GET: api/Event/user/{userId}
		[HttpGet("user/{userId:guid}")]
		public async Task<IActionResult> GetByUserId(Guid userId)
		{
			var result = await _eventService.GetByUserIdAsync(userId);
			return StatusCode(result.Code, result);
		}

		// GET: api/Event/upcoming/{userId}
		[HttpGet("upcoming/{userId:guid}")]
		public async Task<IActionResult> GetUpcoming(Guid userId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
		{
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
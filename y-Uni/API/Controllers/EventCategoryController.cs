using Microsoft.AspNetCore.Mvc;
using Repositories.ViewModels.EventCategoryModel;
using Services.Services.EventCategoryService;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class EventCategoryController : ControllerBase
	{
		private readonly IEventCategoryService _eventCategoryService;

		public EventCategoryController(IEventCategoryService eventCategoryService)
		{
			_eventCategoryService = eventCategoryService;
		}

		#region CRUD Operations

		// GET: api/EventCategory
		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var result = await _eventCategoryService.GetAllAsync();
			return StatusCode(result.Code, result);
		}

		// GET: api/EventCategory/{id}
		[HttpGet("{id:guid}")]
		public async Task<IActionResult> GetById(Guid id)
		{
			var result = await _eventCategoryService.GetByIdAsync(id);
			return StatusCode(result.Code, result);
		}

		// POST: api/EventCategory
		[HttpPost]
		public async Task<IActionResult> Create([FromBody] PostEventCategoryModel model)
		{
			var result = await _eventCategoryService.AddAsync(model);
			return StatusCode(result.Code, result);
		}

		// PUT: api/EventCategory
		[HttpPut]
		public async Task<IActionResult> Update([FromBody] EventCategoryModel model)
		{
			var result = await _eventCategoryService.UpdateAsync(model);
			return StatusCode(result.Code, result);
		}

		// DELETE: api/EventCategory/{id}
		[HttpDelete("{id:guid}")]
		public async Task<IActionResult> Delete(Guid id)
		{
			var result = await _eventCategoryService.DeleteAsync(id);
			return StatusCode(result.Code, result);
		}

		#endregion
	}
} 
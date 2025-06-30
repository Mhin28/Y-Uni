using Microsoft.AspNetCore.Mvc;
using Repositories.ViewModels.PriorityLevelModel;
using Services.Services.PriorityLevelService;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PriorityLevelController : ControllerBase
	{
		private readonly IPriorityLevelService _priorityLevelService;

		public PriorityLevelController(IPriorityLevelService priorityLevelService)
		{
			_priorityLevelService = priorityLevelService;
		}

		#region CRUD Operations

		// GET: api/PriorityLevel
		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var result = await _priorityLevelService.GetAllAsync();
			return StatusCode(result.Code, result);
		}

		// GET: api/PriorityLevel/{id}
		[HttpGet("{id:byte}")]
		public async Task<IActionResult> GetById(byte id)
		{
			var result = await _priorityLevelService.GetByIdAsync(id);
			return StatusCode(result.Code, result);
		}

		// POST: api/PriorityLevel
		[HttpPost]
		public async Task<IActionResult> Create([FromBody] PostPriorityLevelModel model)
		{
			var result = await _priorityLevelService.AddAsync(model);
			return StatusCode(result.Code, result);
		}

		// PUT: api/PriorityLevel
		[HttpPut]
		public async Task<IActionResult> Update([FromBody] PriorityLevelModel model)
		{
			var result = await _priorityLevelService.UpdateAsync(model);
			return StatusCode(result.Code, result);
		}

		// DELETE: api/PriorityLevel/{id}
		[HttpDelete("{id:byte}")]
		public async Task<IActionResult> Delete(byte id)
		{
			var result = await _priorityLevelService.DeleteAsync(id);
			return StatusCode(result.Code, result);
		}

		#endregion
	}
} 
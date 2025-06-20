using Microsoft.AspNetCore.Mvc;
using Repositories.ViewModels.ReminderTemplateModel;
using Services.Services.ReminderTemplateService;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ReminderTemplateController : ControllerBase
	{
		private readonly IReminderTemplateService _templateService;

		public ReminderTemplateController(IReminderTemplateService templateService)
		{
			_templateService = templateService;
		}

		#region CRUD Operations

		// GET: api/ReminderTemplate
		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var result = await _templateService.GetAllAsync();
			return StatusCode(result.Code, result);
		}

		// GET: api/ReminderTemplate/{id}
		[HttpGet("{id:guid}")]
		public async Task<IActionResult> GetById(Guid id)
		{
			var result = await _templateService.GetByIdAsync(id);
			return StatusCode(result.Code, result);
		}

		// POST: api/ReminderTemplate
		[HttpPost]
		public async Task<IActionResult> Create([FromBody] PostReminderTemplateModel model)
		{
			var result = await _templateService.CreateAsync(model);
			return StatusCode(result.Code, result);
		}

		// PUT: api/ReminderTemplate
		[HttpPut]
		public async Task<IActionResult> Update([FromBody] ReminderTemplateModel model)
		{
			var result = await _templateService.UpdateAsync(model);
			return StatusCode(result.Code, result);
		}

		// DELETE: api/ReminderTemplate/{id}
		[HttpDelete("{id:guid}")]
		public async Task<IActionResult> Delete(Guid id)
		{
			var result = await _templateService.DeleteAsync(id);
			return StatusCode(result.Code, result);
		}

		#endregion
	}
}

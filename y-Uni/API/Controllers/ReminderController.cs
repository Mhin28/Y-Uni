using Microsoft.AspNetCore.Mvc;
using Repositories.ViewModels.ReminderModel;
using Services.Services.ReminderService;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ReminderController : ControllerBase
	{
		private readonly IReminderService _reminderService;

		public ReminderController(IReminderService reminderService)
		{
			_reminderService = reminderService;
		}

		#region CRUD Operations

		// GET: api/Reminder
		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var result = await _reminderService.GetAllAsync();
			return StatusCode(result.Code, result);
		}

		// GET: api/Reminder/{id}
		[HttpGet("{id:guid}")]
		public async Task<IActionResult> GetById(Guid id)
		{
			var result = await _reminderService.GetByIdAsync(id);
			return StatusCode(result.Code, result);
		}

		// POST: api/Reminder
		[HttpPost]
		public async Task<IActionResult> Create([FromBody] PostReminderModel model)
		{
			var result = await _reminderService.CreateAsync(model);
			return StatusCode(result.Code, result);
		}

		// PUT: api/Reminder
		[HttpPut]
		public async Task<IActionResult> Update([FromBody] ReminderModel model)
		{
			var result = await _reminderService.UpdateAsync(model);
			return StatusCode(result.Code, result);
		}

		// DELETE: api/Reminder/{id}
		[HttpDelete("{id:guid}")]
		public async Task<IActionResult> Delete(Guid id)
		{
			var result = await _reminderService.DeleteAsync(id);
			return StatusCode(result.Code, result);
		}

		#endregion

		#region Filtering Endpoints

		// GET: api/Reminder/user/{userId}
		[HttpGet("user/{userId:guid}")]
		public async Task<IActionResult> GetByUserId(Guid userId)
		{
			var result = await _reminderService.GetRemindersByUserIdAsync(userId);
			return StatusCode(result.Code, result);
		}

		// GET: api/Reminder/event/{eventId}
		[HttpGet("event/{eventId:guid}")]
		public async Task<IActionResult> GetByEventId(Guid eventId)
		{
			var result = await _reminderService.GetRemindersByEventIdAsync(eventId);
			return StatusCode(result.Code, result);
		}

		// GET: api/Reminder/assignment/{assignmentId}
		[HttpGet("assignment/{assignmentId:guid}")]
		public async Task<IActionResult> GetByAssignmentId(Guid assignmentId)
		{
			var result = await _reminderService.GetRemindersByAssignmentIdAsync(assignmentId);
			return StatusCode(result.Code, result);
		}

		// GET: api/Reminder/template/{templateId}
		[HttpGet("template/{templateId:guid}")]
		public async Task<IActionResult> GetByTemplateId(Guid templateId)
		{
			var result = await _reminderService.GetRemindersByTemplateIdAsync(templateId);
			return StatusCode(result.Code, result);
		}

		// GET: api/Reminder/status/{status}
		[HttpGet("status/{status}")]
		public async Task<IActionResult> GetByStatus(string status)
		{
			var result = await _reminderService.GetRemindersByStatusAsync(status);
			return StatusCode(result.Code, result);
		}

		// GET: api/Reminder/channel/{notificationChannel}
		[HttpGet("channel/{notificationChannel}")]
		public async Task<IActionResult> GetByNotificationChannel(string notificationChannel)
		{
			var result = await _reminderService.GetRemindersByNotificationChannelAsync(notificationChannel);
			return StatusCode(result.Code, result);
		}

		#endregion
	}
}

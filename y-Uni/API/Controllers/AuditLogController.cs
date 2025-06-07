using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repositories.ViewModels.AuditLogsModel;
using Services.Services.AuditLogService;

namespace API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuditLogController : ControllerBase
	{
		private readonly IAuditLogsService _auditLogService;
		public AuditLogController(IAuditLogsService auditLogService)
		{
			_auditLogService = auditLogService;
		}

		#region CRUD Operations
		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var result = await _auditLogService.GetAllAsync();
			return StatusCode(result.Code, result);
		}
		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(Guid id)
		{
			var result = await _auditLogService.GetByIdAsync(id);
			return StatusCode(result.Code, result);
		}
		[HttpGet("user/{userId}")]
		public async Task<IActionResult> GetByUserId(Guid userId)
		{
			var result = await _auditLogService.GetByUserIdAsync(userId);
			return StatusCode(result.Code, result);
		}
		[HttpPost]
		public async Task<IActionResult> Create([FromBody] PostAuditLogModel model)
		{
			var result = await _auditLogService.AddAuditLogAsync(model);
			return StatusCode(result.Code, result);
		}
		[HttpPut]
		public async Task<IActionResult> Update([FromBody] AuditLogModel model)
		{
			var result = await _auditLogService.UpdateAuditLogAsync(model);
			return StatusCode(result.Code, result);
		}
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(Guid id)
		{
			var result = await _auditLogService.DeleteAuditLogAsync(id);
			return StatusCode(result.Code, result);
		}
		#endregion
	}
}

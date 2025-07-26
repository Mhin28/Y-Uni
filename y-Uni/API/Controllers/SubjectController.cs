using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories.ViewModels.SubjectModel;
using Services.Services.SubjectService;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class SubjectController : ControllerBase
	{
		private readonly ISubjectService _subjectService;

		public SubjectController(ISubjectService subjectService)
		{
			_subjectService = subjectService;
		}

		#region CRUD Operations

		// GET: api/Subject
		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var result = await _subjectService.GetAllAsync();
			return StatusCode(result.Code, result);
		}

		// GET: api/Subject/{id}
		[HttpGet("{id:guid}")]
		public async Task<IActionResult> GetById(Guid id)
		{
			var result = await _subjectService.GetByIdAsync(id);
			return StatusCode(result.Code, result);
		}

		// GET: api/Subject/user/{userId}
		[HttpGet("user/{userId:guid}")]
		public async Task<IActionResult> GetByUserId(Guid userId)
		{
			var result = await _subjectService.GetByUserIdAsync(userId);
			return StatusCode(result.Code, result);
		}

		// GET: api/Subject/my
		[HttpGet("my")]
		public async Task<IActionResult> GetMySubjects()
		{
			var result = await _subjectService.GetMySubjects();
			return StatusCode(result.Code, result);
		}

		// POST: api/Subject
		[HttpPost]
		public async Task<IActionResult> Create([FromBody] PostSubjectModel model)
		{
			var result = await _subjectService.AddAsync(model);
			return StatusCode(result.Code, result);
		}

		// PUT: api/Subject
		[HttpPut]
		public async Task<IActionResult> Update([FromBody] SubjectModel model)
		{
			var result = await _subjectService.UpdateAsync(model);
			return StatusCode(result.Code, result);
		}

		// DELETE: api/Subject/{id}
		[HttpDelete("{id:guid}")]
		public async Task<IActionResult> Delete(Guid id)
		{
			var result = await _subjectService.DeleteAsync(id);
			return StatusCode(result.Code, result);
		}

		#endregion
	}
} 
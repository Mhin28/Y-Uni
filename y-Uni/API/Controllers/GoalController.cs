using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Services.GoalService;
using Repositories.ViewModels.GoalModel;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GoalController : ControllerBase
    {
        private readonly IGoalService _goalService;
        public GoalController(IGoalService goalService)
        {
            _goalService = goalService;
        }
        [HttpGet("my-goals")]
        public async Task<IActionResult> GetMyGoals([FromQuery] string? status, [FromQuery] DateOnly? from, [FromQuery] DateOnly? to)
        {
            string token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var result = await _goalService.GetGoalsByUserAsync(token, status, from, to);
            return StatusCode(result.Code, result);
        }
        [HttpPost]
        public async Task<IActionResult> CreateGoal([FromBody] PostGoalModel model)
        {
            string token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var result = await _goalService.CreateGoalAsync(token, model);
            return StatusCode(result.Code, result);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGoal(Guid id, [FromBody] UpdateGoalModel model)
        {
            string token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var result = await _goalService.UpdateGoalAsync(token, id, model);
            return StatusCode(result.Code, result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGoal(Guid id)
        {
            string token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var result = await _goalService.DeleteGoalAsync(token, id);
            return StatusCode(result.Code, result);
        }
    }
} 
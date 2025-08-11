using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Services.ReviewServices;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] 
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _reviewService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(Guid userId)
        {
            var result = await _reviewService.GetByUserIdAsync(userId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdate([FromBody] ReviewRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _reviewService.CreateOrUpdateAsync(request.UserId, request.Rating, request.Comment);
            return Ok(result);
        }

        [HttpDelete("{reviewId}")]
        public async Task<IActionResult> Delete(Guid reviewId)
        {
            var result = await _reviewService.DeleteAsync(reviewId);
            return Ok(result);
        }
    }

    public class ReviewRequest
    {
        public Guid UserId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}

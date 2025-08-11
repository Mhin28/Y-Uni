using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Services.ReviewServices;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var res = await _reviewService.GetAllReviewsAsync();
            return StatusCode(res.Code, res);
        }

        [HttpGet("user/{userId:guid}")]
        public async Task<IActionResult> GetByUserId(Guid userId)
        {
            var res = await _reviewService.GetReviewsByUserIdAsync(userId);
            return StatusCode(res.Code, res);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateReviewRequest request)
        {
            string token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var res = await _reviewService.CreateReviewAsync(token, request.Rating, request.Comment);
            return StatusCode(res.Code, res);
        }

        [HttpPut("update/{reviewId:guid}")]
        public async Task<IActionResult> Update(Guid reviewId, [FromBody] UpdateReviewRequest request)
        {
            var res = await _reviewService.UpdateReviewAsync(request.UserId, reviewId, request.Rating, request.Comment);
            return StatusCode(res.Code, res);
        }
    }

    public class CreateReviewRequest
    {
        public int Rating { get; set; }
        public string Comment { get; set; }
    }

    public class UpdateReviewRequest
    {
        public Guid UserId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}

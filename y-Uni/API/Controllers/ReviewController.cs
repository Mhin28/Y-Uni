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

        [HttpGet("user")]
        public async Task<IActionResult> GetByUserId()
        {
            string token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var res = await _reviewService.GetReviewsByTokenAsync(token);
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
            string token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var res = await _reviewService.UpdateReviewAsync(token, reviewId, request.Rating, request.Comment);
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
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}

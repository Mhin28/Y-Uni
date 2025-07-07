using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories.ViewModels.UserModel;
using Services.Services.UserService;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [Route("create-user")]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserModel userModel)
        {
            string token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var res = await _userService.CreateUser(token, userModel);
            return StatusCode(res.Code, res);
        }
        
        // GET: api/User/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _userService.GetUserByIdAsync(id);
            return StatusCode(result.Code, result);
        }
        
        // PUT: api/User/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserModel model)
        {
            // Ensure the ID in the route matches the ID in the model
            if (model.UserId != id)
            {
                model.UserId = id;
            }
            
            var result = await _userService.UpdateUserAsync(id, model);
            return StatusCode(result.Code, result);
        }
        [HttpPost("verify")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailModel model)
        {
            var result = await _userService.VerifyEmailAsync(model.Email, model.Code);
            return StatusCode(result.Code, result);
        }
        [HttpPost("resend-verification")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendVerificationCode([FromBody] ResendVerificationModel model)
        {
            var result = await _userService.ResendVerificationCodeAsync(model.Email);
            return StatusCode(result.Code, result);
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories.ViewModels.UserModel;
using Services.Services.CloudinaryService;
using Services.Services.UserService;
using System.Net;
using System.Net.Mail;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly ICloudinaryService _cloudinaryService;

        public UserController(IUserService userService, ICloudinaryService cloudinaryService)
        {
            _userService = userService;
            _cloudinaryService = cloudinaryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _userService.GetAllUser();
            return StatusCode(result.Code, result);
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

        [HttpPut("update-account")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateAccount([FromForm] UpdateUserModel model)
        {
            string token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var res = await _userService.UpdateAccountLogin(token, model);
            return StatusCode(res.Code, res);
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
        [HttpGet("logged-in-user")]
        public async Task<IActionResult> GetLoggedInUser()
        {
            string token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var res = await _userService.GetLoggedInUser(token);
            return StatusCode(res.Code, res);
        }
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            string token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var res = await _userService.ChangePassword(token, model);
            return StatusCode(res.Code, res);
        }

        [HttpPost("forgot-password/send-code")]
        [AllowAnonymous]
        public async Task<IActionResult> SendForgotPasswordCode([FromBody] ResendVerificationModel model)
        {
            var result = await _userService.SendForgotPasswordCodeAsync(model.Email);
            return StatusCode(result.Code, result);
        }

        [HttpPost("forgot-password/reset")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPasswordWithCode([FromBody] ChangePasswordModel model)
        {
            var result = await _userService.ResetPasswordWithCodeAsync(model);
            return StatusCode(result.Code, result);
        }

        [HttpPost("login-google")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithGoogle([FromBody] GoogleLoginModel model)
        {
            var result = await _userService.LoginWithGoogleAsync(model.IdToken);
            return StatusCode(result.Code, result);
        }


        [HttpPost("upload-avatar")]
        public async Task<IActionResult> UploadAvatar(IFormFile file)
        {
            string token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var result = await _userService.UpdateAvatarAsync(token, file);
            return StatusCode(result.Code, result);
        }

        [HttpPost("test-upload")]
        [AllowAnonymous]
        public async Task<IActionResult> TestUpload(IFormFile file)
        {
            var result = await _cloudinaryService.TestUploadAsync(file);
            return StatusCode(result.Code, result);
        }

        [HttpPost("upload-image")]
        [AllowAnonymous]
        public async Task<IActionResult> UploadImage(IFormFile file, [FromQuery] string folder = "general")
        {
            var result = await _cloudinaryService.UploadImageAsync(file, folder);
            return StatusCode(result.Code, result);
        }

        [HttpDelete("delete-image")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteImage([FromQuery] string publicId)
        {
            var result = await _cloudinaryService.DeleteImageAsync(publicId);
            return StatusCode(result.Code, result);
        }

        [HttpPost("test-smtp")]
        [AllowAnonymous]
        public async Task<IActionResult> TestSMTP()
        {
            try
            {
                Console.WriteLine("🧪 Starting SMTP test...");

                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new System.Net.NetworkCredential("yunibuddy18@gmail.com", "pjue wfbe qsfe mwhp"),
                    EnableSsl = true,
                    Timeout = 30000,
                    UseDefaultCredentials = false
                };

                Console.WriteLine("📧 SMTP client configured");

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("yunibuddy18@gmail.com", "Y-Uni Test"),
                    Subject = "SMTP Test - " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Body = @"
                <h2>SMTP Test Email</h2>
                <p>This is a direct SMTP test from Y-Uni API.</p>
                <p><strong>Sent at:</strong> " + DateTime.Now + @"</p>
                <p><strong>Server time:</strong> " + DateTime.UtcNow + @" UTC</p>
                <p>If you receive this email, the SMTP configuration is working correctly!</p>
            ",
                    IsBodyHtml = true,
                };

                mailMessage.To.Add("thiennhse184989@fpt.edu.vn");

                Console.WriteLine($"📧 Sending test email to: thiennhse184989@fpt.edu.vn");
                Console.WriteLine($"📧 Subject: {mailMessage.Subject}");

                await smtpClient.SendMailAsync(mailMessage);

                Console.WriteLine("✅ SMTP test email sent successfully!");

                return Ok(new
                {
                    success = true,
                    message = "SMTP test email sent successfully",
                    sentTo = "thiennhse184989@fpt.edu.vn",
                    sentAt = DateTime.Now,
                    serverTime = DateTime.UtcNow
                });
            }
            catch (SmtpException smtpEx)
            {
                Console.WriteLine($"❌ SMTP Exception: {smtpEx.Message}");
                Console.WriteLine($"❌ SMTP Status Code: {smtpEx.StatusCode}");
                Console.WriteLine($"❌ Inner Exception: {smtpEx.InnerException?.Message}");

                return BadRequest(new
                {
                    success = false,
                    error = "SMTP Error",
                    message = smtpEx.Message,
                    statusCode = smtpEx.StatusCode.ToString(),
                    innerError = smtpEx.InnerException?.Message,
                    type = "SmtpException"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ General Exception: {ex.Message}");
                Console.WriteLine($"❌ Exception Type: {ex.GetType().Name}");
                Console.WriteLine($"❌ Stack Trace: {ex.StackTrace}");

                return BadRequest(new
                {
                    success = false,
                    error = "General Error",
                    message = ex.Message,
                    type = ex.GetType().Name,
                    innerError = ex.InnerException?.Message
                });
            }
        }

        [HttpPost("test-smtp-alternative")]
        [AllowAnonymous]
        public async Task<IActionResult> TestSMTPAlternative()
        {
            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 465,
                    Credentials = new NetworkCredential("yunibuddy18@gmail.com", "pjue wfbe qsfe mwhp"),
                    EnableSsl = true,
                    Timeout = 30000,
                    UseDefaultCredentials = false
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("yunibuddy18@gmail.com"),
                    Subject = "Alternative SMTP Test - " + DateTime.Now,
                    Body = "Alternative SMTP test using port 465",
                    IsBodyHtml = false,
                };

                mailMessage.To.Add("thiennhse184989@fpt.edu.vn");

                await smtpClient.SendMailAsync(mailMessage);

                return Ok(new
                {
                    success = true,
                    message = "Alternative SMTP test successful (port 465)",
                    port = 465
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    error = ex.Message,
                    port = 465,
                    type = ex.GetType().Name
                });
            }
        }
    }
}
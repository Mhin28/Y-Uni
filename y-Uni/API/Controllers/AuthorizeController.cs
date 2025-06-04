using Microsoft.AspNetCore.Mvc;
using Repositories.ViewModels.AutheticateModel;
using Repositories.ViewModels.ResultModels;
using Services.Services.AccountService;

namespace API.Controllers
{
    [Route("api/controller")]
    [ApiController]
    public class AuthorizeController: Controller
    {
        private readonly IAccountService _accountService; 
        public AuthorizeController(IAccountService accountService)
        {
            _accountService = accountService;
        }
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginReqModel user)
        {
            ResultModel resultModel = await _accountService.loginService(user);
            return resultModel.IsSuccess ? Ok(resultModel) : BadRequest(resultModel);
        }
    }
}

using Repositories.Repositories;
using Repositories.ViewModels.AutheticateModel;
using Repositories.ViewModels.ResultModels;
using Services.Services.AuthenticateService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.AccountService
{
    public class AccountService : IAccountService
    {
        private readonly IUserRepo _userRepo;
        private readonly IAuthenticateService _authenticateService;
        public AccountService(IUserRepo userRepo, IAuthenticateService authenticateService)
        {
            _userRepo = userRepo;
            _authenticateService = authenticateService;
        }

        public bool IsValidRole(string userRole, List<int> validRole)
        {
            return validRole.Any(role => role.ToString() == userRole);
        }

        public async Task<ResultModel> loginService(UserLoginReqModel user)
        {
            ResultModel res = new ResultModel();
            try
            {
                var existedUser = await _userRepo.GetByUsernameAsync(user.username);
                if (existedUser == null)
                {
                    res.IsSuccess = false;
                    res.Code = 400;
                    res.Message = "User not existed";
                    return res;
                }
                if (existedUser.IsVerified.Equals(false))
                {
                    res.IsSuccess = false;
                    res.Code = 400;
                    res.Message = "You do not have access!";
                    return res;
                }
                bool isMatch = HashPass.HashPass.VerifyPassword(user.password, existedUser.PasswordHash);
                if (!isMatch)
                {
                    res.IsSuccess = false;
                    res.Code = 400;
                    res.Message = "Wrong password";
                    return res;
                }
                LoginResModel loginResModel = new LoginResModel()
                {
                    UserId = existedUser.UserId,
                    FullName = existedUser.FullName,
                    UserName = existedUser.UserName,
                    Email = existedUser.Email,
                    DoB = existedUser.DoB,
                    RoleId = existedUser.RoleId,
                    PasswordHash = existedUser.PasswordHash,
                    LastLogin = existedUser.LastLogin,
                    Img = existedUser.Img,
                    IsVerified = existedUser.IsVerified,
                    CreatedAt = existedUser.CreatedAt,
                    UpdatedAt = existedUser.UpdatedAt
                };
                var token = _authenticateService.GenerateJWT(loginResModel);
                LoginTokenModel loginTokenModel = new LoginTokenModel()
                {
                    LoginResModel = loginResModel,
                    token = token
                };
                res.IsSuccess = true;
                res.Code = 200;
                res.Data = loginTokenModel;
                return res;
            }
            catch (Exception e)
            {
                res.IsSuccess = false;
                res.Code = 400;
                res.Message = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return res;
        }
    }
}

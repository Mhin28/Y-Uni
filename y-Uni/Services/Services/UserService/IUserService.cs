using Repositories.ViewModels.ResultModels;
using Repositories.ViewModels.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.UserService
{
    public interface IUserService
    {
        Task<ResultModel> CreateUser(string token, CreateUserModel model);
        Task<ResultModel> UpdateAccountLogin(string token, UpdateUserModel model);
        Task<ResultModel> GetUserByIdAsync(Guid userId);
        Task<ResultModel> VerifyEmailAsync(string email, string code);
        Task<ResultModel> ResendVerificationCodeAsync(string email);
        Task<ResultModel> ChangePassword(string Token, ChangePasswordModel model);
        Task<ResultModel> GetLoggedInUser(string token);
    }
}

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
    }
}

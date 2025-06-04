using Repositories.ViewModels.AutheticateModel;
using Repositories.ViewModels.ResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.AccountService
{
    public interface IAccountService
    {
        public Task<ResultModel> loginService(UserLoginReqModel user);
        public bool IsValidRole(string userRole, List<int> validRole);
    }
}

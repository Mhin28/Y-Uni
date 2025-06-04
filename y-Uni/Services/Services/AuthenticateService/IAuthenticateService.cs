using Repositories.ViewModels.AutheticateModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.AuthenticateService
{
    public interface IAuthenticateService
    {
        string GenerateJWT(LoginResModel User);
        string decodeToken(string jwtToken, string nameClaim);
    }
}

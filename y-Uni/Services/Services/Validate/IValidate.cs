using Microsoft.Extensions.Options;
using Repositories.ViewModels.ResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.Validate
{
    public interface IValidate
    {
        Task<ResultModel> IsUserNameUnique(string username);
        Task<ResultModel> IsEmailUnique(string Email);
    }
}

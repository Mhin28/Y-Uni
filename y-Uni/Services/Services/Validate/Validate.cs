using Repositories.Repositories;
using Repositories.ViewModels.ResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Services.Services.Validate
{
    public class Validate : IValidate
    {
        private readonly IUserRepo _userRepo;
        public Validate(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<ResultModel> IsEmailUnique(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = "Email cannot be empty."
                };
            }

            email = email.Trim().ToLower();
            var existingUser = await _userRepo.GetByEmailAsync(email);

            if (existingUser != null)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.Conflict,
                    Message = "This email is already in use."
                };
            }

            return new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Email is unique."
            };
        }

        public async Task<ResultModel> IsUserNameUnique(string username)
        {
            var result = new ResultModel();
            try
            {
                var existedUser = await _userRepo.GetByUsernameAsync(username);
                if (existedUser != null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.BadRequest;
                    result.Message = "The provided username has already existed";
                }
                return result;

            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = (int)HttpStatusCode.BadRequest;
                result.Message = ex.Message;
                return result;
            }

        }
    }
}

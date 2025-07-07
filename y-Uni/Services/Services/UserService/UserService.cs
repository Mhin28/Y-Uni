using Repositories.Models;
using Repositories.Repositories;
using Repositories.ViewModels.ResultModels;
using Repositories.ViewModels.UserModel;
using Services.Services.AccountService;
using Services.Services.AuthenticateService;
using Services.Services.TokenService;
using Services.Services.Validate;
using Services.Services.EmailService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;

namespace Services.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IUserRepo _userRepo;
        private readonly IValidate _Validate;
        private readonly IAccountService _accountService;
        private readonly IAuthenticateService _authentocateService;
        private readonly ITokenService _token;
        private readonly IEmailService _emailService;
        public UserService(IUserRepo userRepo,
            ITokenService token,
            IAuthenticateService authenticateService,
            IAccountService accountService,
            IValidate userValidate,
            IEmailService emailService
            )
        {
            _userRepo = userRepo;
            _token = token;
            _authentocateService = authenticateService;
            _accountService = accountService;
            _Validate = userValidate;
            _emailService = emailService;
        }
        public async Task<ResultModel> CreateUser(string token, CreateUserModel model)
        {
            var res = new ResultModel
            {
                IsSuccess = false,
                Code = (int)HttpStatusCode.BadRequest,
                Message = "Invalid request."
            };

            if (!string.IsNullOrEmpty(token))
            {
                var decodeModel = _token.decode(token);
                if (decodeModel == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Code = (int)HttpStatusCode.Unauthorized,
                        Message = "Invalid token."
                    };
                }
            }
            var existingUser = await _userRepo.GetByUsernameAsync(model.UserName);
            if (existingUser != null)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.Conflict,
                    Message = "This user is already registered."
                };
            }

            var existingEmail = await _userRepo.GetByEmailAsync(model.Email);
            if (existingEmail != null)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.Conflict,
                    Message = "This email is already registered."
                };
            }
            var validRole = await _userRepo.CheckRoleExists((Guid)model.RoleId);
            if (!validRole)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = "RoleId không hợp lệ."
                };
            }

            string hashedPassword = HashPass.HashPass.HashPassword(model.PasswordHash);
            var code = new Random().Next(100000, 999999).ToString();
            var user = new User
            {
                UserId = Guid.NewGuid(),
                FullName = model.FullName,
                UserName = model.UserName,
                Email = model.Email,
                DoB = model.DoB,
                PasswordHash = hashedPassword,
                LastLogin = null,
                Img = null,
                IsVerified = false,
                RoleId = model.RoleId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                VerificationCode = code,
                VerificationCodeExpiry = DateTime.UtcNow.AddMinutes(10)
            };

            await _userRepo.AddAsync(user);
            await _emailService.SendVerificationEmailAsync(user.Email, code);

            return new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.Created,
                Message = "User created successfully. Verification code sent to email.",
                Data = new
                {
                    Email = user.Email,
                    UserId = user.UserId
                }
            };
        }

        public async Task<ResultModel> UpdateUserAsync(Guid userId, UpdateUserModel model)
        {
            var result = new ResultModel
            {
                IsSuccess = false,
                Code = (int)HttpStatusCode.BadRequest,
                Message = "Update failed."
            };

            try
            {
                // Check if user exists
                var existingUser = await _userRepo.GetByIdAsync(userId);
                if (existingUser == null)
                {
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "User not found.";
                    return result;
                }

                // Check if email is already in use by another user
                if (!string.Equals(existingUser.Email, model.Email, StringComparison.OrdinalIgnoreCase))
                {
                    var emailExists = await _userRepo.GetByEmailAsync(model.Email);
                    if (emailExists != null && emailExists.UserId != userId)
                    {
                        result.Code = (int)HttpStatusCode.Conflict;
                        result.Message = "This email is already in use by another account.";
                        return result;
                    }
                }

                // Update user properties
                existingUser.FullName = model.FullName;
                existingUser.Email = model.Email;
                existingUser.DoB = model.DoB;
                existingUser.UpdatedAt = DateTime.UtcNow;

                // Save changes
                var updatedUser = await _userRepo.UpdateAsync(existingUser);

                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.OK;
                result.Message = "User updated successfully.";
                result.Data = new
                {
                    UserId = updatedUser.UserId,
                    FullName = updatedUser.FullName,
                    Email = updatedUser.Email,
                    DoB = updatedUser.DoB,
                    UpdatedAt = updatedUser.UpdatedAt
                };
            }
            catch (Exception ex)
            {
                result.Code = (int)HttpStatusCode.InternalServerError;
                result.Message = $"An error occurred: {ex.Message}";
            }

            return result;
        }

        public async Task<ResultModel> GetUserByIdAsync(Guid userId)
        {
            var result = new ResultModel
            {
                IsSuccess = false,
                Code = (int)HttpStatusCode.BadRequest,
                Message = "Invalid request."
            };

            try
            {
                var user = await _userRepo.GetByIdAsync(userId);
                if (user == null)
                {
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "User not found.";
                    return result;
                }

                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.OK;
                result.Data = new
                {
                    UserId = user.UserId,
                    FullName = user.FullName,
                    UserName = user.UserName,
                    Email = user.Email,
                    DoB = user.DoB,
                    IsVerified = user.IsVerified,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                };
            }
            catch (Exception ex)
            {
                result.Code = (int)HttpStatusCode.InternalServerError;
                result.Message = $"An error occurred: {ex.Message}";
            }

            return result;
        }

        public async Task<ResultModel> VerifyEmailAsync(string email, string code)
        {
            var result = new ResultModel
            {
                IsSuccess = false,
                Code = (int)HttpStatusCode.BadRequest,
                Message = "Invalid or expired code."
            };

            try
            {
                var user = await _userRepo.GetByEmailAsync(email);
                if (user == null)
                {
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "User not found.";
                    return result;
                }
                if (user.VerificationCode != code || user.VerificationCodeExpiry < DateTime.UtcNow)
                {
                    result.Code = (int)HttpStatusCode.BadRequest;
                    result.Message = "Invalid or expired code.";
                    return result;
                }

                user.IsVerified = true;
                user.VerificationCode = null;
                user.VerificationCodeExpiry = null;

                var updatedUser = await _userRepo.UpdateVerifyAsync(user);

                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.OK;
                result.Message = "Email verified successfully.";
                result.Data = new
                {
                    UserId = updatedUser.UserId,
                    Email = updatedUser.Email,
                    IsVerified = updatedUser.IsVerified
                };
            }
            catch (Exception ex)
            {
                result.Code = (int)HttpStatusCode.InternalServerError;
                result.Message = $"An error occurred: {ex.Message}";
            }

            return result;
        }

        public async Task<ResultModel> ResendVerificationCodeAsync(string email)
        {
            var result = new ResultModel
            {
                IsSuccess = false,
                Code = (int)HttpStatusCode.BadRequest,
                Message = "User not found."
            };

            try
            {
                // Lấy user theo email
                var user = await _userRepo.GetByEmailAsync(email);
                if (user == null)
                {
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "User not found.";
                    return result;
                }

                var code = new Random().Next(100000, 999999).ToString();
                user.VerificationCode = code;
                user.VerificationCodeExpiry = DateTime.UtcNow.AddMinutes(10);
                user.IsVerified = false;

                var updatedUser = await _userRepo.UpdateVerifyAsync(user);
                await _emailService.SendVerificationEmailAsync(user.Email, code);

                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.OK;
                result.Message = "Verification code resent successfully.";
                result.Data = new
                {
                    UserId = updatedUser.UserId,
                    Email = updatedUser.Email,
                    VerificationCode = code,
                    VerificationCodeExpiry = updatedUser.VerificationCodeExpiry
                };
            }
            catch (Exception ex)
            {
                result.Code = (int)HttpStatusCode.InternalServerError;
                result.Message = $"An error occurred: {ex.Message}";
            }

            return result;
        }
    }
}

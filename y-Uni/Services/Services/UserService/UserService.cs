using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Repositories.Models;
using Repositories.Repositories;
using Repositories.ViewModels.AutheticateModel;
using Repositories.ViewModels.ResultModels;
using Repositories.ViewModels.UserModel;
using Services.Services.AccountService;
using Services.Services.AuthenticateService;
using Services.Services.CloudinaryService;
using Services.Services.EmailService;
using Services.Services.TokenService;
using Services.Services.Validate;

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
        private readonly ICloudinaryService _cloudinaryService;
        
        public UserService(IUserRepo userRepo,
            ITokenService token,
            IAuthenticateService authenticateService,
            IAccountService accountService,
            IValidate userValidate,
            IEmailService emailService,
            ICloudinaryService cloudinaryService
            )
        {
            _userRepo = userRepo;
            _token = token;
            _authentocateService = authenticateService;
            _accountService = accountService;
            _Validate = userValidate;
            _emailService = emailService;
            _cloudinaryService = cloudinaryService;
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
            var validRole = await _userRepo.CheckRoleExists((int)model.RoleId);
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
                Data = user
            };
        }

        public async Task<ResultModel> UpdateAvatarAsync(string token, IFormFile file)
        {
            var result = new ResultModel
            {
                IsSuccess = false,
                Code = (int)HttpStatusCode.BadRequest,
                Message = "Failed to update avatar."
            };

            // Validate token
            if (string.IsNullOrEmpty(token))
            {
                result.Code = (int)HttpStatusCode.Unauthorized;
                result.Message = "Invalid token.";
                return result;
            }

            var decodedUser = _token.decode(token);
            if (decodedUser == null || string.IsNullOrEmpty(decodedUser.userid))
            {
                result.Code = (int)HttpStatusCode.Unauthorized;
                result.Message = "Invalid token.";
                return result;
            }

            if (!Guid.TryParse(decodedUser.userid, out Guid userId))
            {
                result.Code = (int)HttpStatusCode.Unauthorized;
                result.Message = "Invalid token.";
                return result;
            }

            // Check if user exists
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null)
            {
                result.Code = (int)HttpStatusCode.NotFound;
                result.Message = "User not found.";
                return result;
            }

            // Validate file
            if (file == null || file.Length == 0)
            {
                result.Message = "No file provided.";
                return result;
            }

            try
            {
                // Delete existing avatar if it exists
                if (!string.IsNullOrEmpty(user.Img))
                {
                    // Extract public ID from existing image URL
                    var existingPublicId = ExtractPublicIdFromUrl(user.Img);
                    Console.WriteLine($"🗑️ Attempting to delete existing avatar with public ID: '{existingPublicId}' from URL: '{user.Img}'");
                    
                    if (!string.IsNullOrEmpty(existingPublicId))
                    {
                        var deleteResult = await _cloudinaryService.DeleteImageAsync(existingPublicId);
                        Console.WriteLine($"🗑️ Delete result: Success={deleteResult.IsSuccess}, Message={deleteResult.Message}");
                    }
                    else
                    {
                        Console.WriteLine("⚠️ Could not extract public ID from existing avatar URL");
                    }
                }

                // Upload new avatar with user-specific naming
                Console.WriteLine($"📤 Uploading new avatar for user: {userId}");
                var uploadResult = await _cloudinaryService.UploadAvatarAsync(file, userId.ToString());
                
                if (!uploadResult.IsSuccess)
                {
                    result.Code = uploadResult.Code;
                    result.Message = uploadResult.Message;
                    return result;
                }

                // Extract secure URL from upload result
                dynamic uploadData = uploadResult.Data;
                string secureUrl = uploadData.SecureUrl;
                string publicId = uploadData.PublicId;
                
                Console.WriteLine($"📤 Upload successful - URL: {secureUrl}, PublicId: {publicId}");

                // Update user's avatar in database
                var updatedUser = await _userRepo.UpdateAvatarAsync(userId, secureUrl);
                if (updatedUser == null)
                {
                    result.Code = (int)HttpStatusCode.InternalServerError;
                    result.Message = "Failed to update user avatar in database.";
                    return result;
                }

                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.OK;
                result.Message = "Avatar updated successfully.";
                result.Data = new
                {
                    UserId = updatedUser.UserId,
                    AvatarUrl = updatedUser.Img,
                    UploadDetails = uploadResult.Data,
                    DeletedOldAvatar = !string.IsNullOrEmpty(user.Img)
                };
            }
            catch (Exception ex)
            {
                result.Code = (int)HttpStatusCode.InternalServerError;
                result.Message = $"An error occurred while updating avatar: {ex.Message}";
            }

            return result;
        }

        private string ExtractPublicIdFromUrl(string imageUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(imageUrl) || !imageUrl.Contains("cloudinary.com"))
                    return null;

                // Extract public ID from Cloudinary URL
                // Format: https://res.cloudinary.com/{cloud_name}/image/upload/{version_or_transformations}/{public_id}.{format}
                var uri = new Uri(imageUrl);
                var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
                
                // Find the upload segment
                var uploadIndex = Array.IndexOf(segments, "upload");
                if (uploadIndex == -1 || uploadIndex + 1 >= segments.Length)
                    return null;

                // Get segments after upload
                var segmentsAfterUpload = segments.Skip(uploadIndex + 1).ToArray();
                
                // Skip version number if present (starts with 'v' followed by digits)
                var startIndex = 0;
                if (segmentsAfterUpload.Length > 0 && segmentsAfterUpload[0].StartsWith("v") && 
                    segmentsAfterUpload[0].Length > 1 && segmentsAfterUpload[0].Substring(1).All(char.IsDigit))
                {
                    startIndex = 1;
                }

                // Skip transformation parameters (they contain specific patterns like w_, h_, c_, etc.)
                while (startIndex < segmentsAfterUpload.Length)
                {
                    var segment = segmentsAfterUpload[startIndex];
                    // Check if this looks like a transformation parameter
                    if (segment.Contains("_") && (segment.StartsWith("w_") || segment.StartsWith("h_") || 
                        segment.StartsWith("c_") || segment.StartsWith("q_") || segment.StartsWith("f_") ||
                        segment.Contains(",") || segment.All(c => char.IsDigit(c) || c == '_' || c == ',')))
                    {
                        startIndex++;
                    }
                    else
                    {
                        break;
                    }
                }

                if (startIndex >= segmentsAfterUpload.Length)
                    return null;

                // Join remaining segments to form public ID
                var publicIdParts = segmentsAfterUpload.Skip(startIndex).ToArray();
                var publicId = string.Join("/", publicIdParts);
                
                // Remove file extension from the last part
                var lastDotIndex = publicId.LastIndexOf('.');
                if (lastDotIndex > 0)
                {
                    publicId = publicId.Substring(0, lastDotIndex);
                }

                return publicId;
            }
            catch (Exception ex)
            {
                // Log the error for debugging
                Console.WriteLine($"Error extracting public ID from URL '{imageUrl}': {ex.Message}");
                return null;
            }
        }

        public async Task<ResultModel> UpdateAccountLogin(string token, UpdateUserModel model)
        {
            var res = new ResultModel
            {
                IsSuccess = false,
                Code = (int)HttpStatusCode.BadRequest,
                Message = "Cập nhật thất bại."
            };

            if (string.IsNullOrEmpty(token))
            {
                res.Code = (int)HttpStatusCode.Unauthorized;
                res.Message = "Token không hợp lệ.";
                return res;
            }

            var decodedUser = _token.decode(token);
            if (decodedUser == null || string.IsNullOrEmpty(decodedUser.userid))
            {
                res.Code = (int)HttpStatusCode.Unauthorized;
                res.Message = "Token không hợp lệ.";
                return res;
            }

            if (!Guid.TryParse(decodedUser.userid, out Guid userId))
            {
                res.Code = (int)HttpStatusCode.Unauthorized;
                res.Message = "Token không hợp lệ.";
                return res;
            }

            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null)
            {
                res.Code = (int)HttpStatusCode.NotFound;
                res.Message = "Không tìm thấy người dùng.";
                return res;
            }
            
            if (!string.Equals(user.Email, model.Email, StringComparison.OrdinalIgnoreCase))
            {
                var emailExists = await _userRepo.GetByEmailAsync(model.Email);
                if (emailExists != null && emailExists.UserId != userId)
                {
                    res.Code = (int)HttpStatusCode.Conflict;
                    res.Message = "Email đã được sử dụng.";
                    return res;
                }
            }
            if (!string.IsNullOrEmpty(model.Img))
                user.Img = model.Img;
            if (!string.IsNullOrEmpty(model.FullName))
                user.FullName = model.FullName;
            if (!string.IsNullOrEmpty(model.Email))
                user.Email = model.Email;
            if (model.DoB != null && model.DoB.HasValue)
                user.DoB = DateOnly.FromDateTime(model.DoB.Value);
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepo.UpdateAsync(user);

            res.IsSuccess = true;
            res.Code = (int)HttpStatusCode.OK;
            res.Message = "Cập nhật thành công.";
            res.Data = user;
            return res;
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
                if(user.IsVerified != true)
{
                user.IsVerified = true;
                user.VerificationCode = null;
                    user.VerificationCodeExpiry = null;
}

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

        public async Task<ResultModel> ChangePassword(string token, ChangePasswordModel model)
        {
            var res = new ResultModel
            {
                IsSuccess = false,
                Code = (int)HttpStatusCode.BadRequest,
                Message = "Đổi mật khẩu thất bại."
            };

            if (string.IsNullOrEmpty(token))
            {
                res.Code = (int)HttpStatusCode.Unauthorized;
                res.Message = "Token không hợp lệ.";
                return res;
            }

            var decodedUser = _token.decode(token);
            if (decodedUser == null || string.IsNullOrEmpty(decodedUser.userid))
            {
                res.Code = (int)HttpStatusCode.Unauthorized;
                res.Message = "Token không hợp lệ.";
                return res;
            }

            if (!Guid.TryParse(decodedUser.userid, out Guid userId))
            {
                res.Code = (int)HttpStatusCode.Unauthorized;
                res.Message = "Token không hợp lệ.";
                return res;
            }

            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null)
            {
                res.Code = (int)HttpStatusCode.NotFound;
                res.Message = "Không tìm thấy người dùng.";
                return res;
            }

            if (user.RoleId != 1 && user.RoleId != 2)
            {
                res.Code = (int)HttpStatusCode.Forbidden;
                res.Message = "Bạn không có quyền đổi mật khẩu.";
                return res;
            }

            if (!HashPass.HashPass.VerifyPassword(model.OldPassword, user.PasswordHash))
            {
                res.Code = (int)HttpStatusCode.BadRequest;
                res.Message = "Mật khẩu cũ không đúng.";
                return res;
            }

            user.PasswordHash = HashPass.HashPass.HashPassword(model.NewPassword);
            await _userRepo.UpdateAsync(user);

            res.IsSuccess = true;
            res.Code = (int)HttpStatusCode.OK;
            res.Message = "Đổi mật khẩu thành công.";
            return res;
        }

        public async Task<ResultModel> GetLoggedInUser(string token)
        {
            var res = new ResultModel
            {
                IsSuccess = false,
                Code = (int)HttpStatusCode.Unauthorized,
                Message = "Invalid token."
            };

            if (string.IsNullOrEmpty(token))
            {
                return res;
            }

            var decodedUser = _token.decode(token);
            if (decodedUser == null || string.IsNullOrEmpty(decodedUser.userid))
            {
                return res;
            }

            if (!Guid.TryParse(decodedUser.userid, out Guid userId))
            {
                return res;
            }
            if (userId == Guid.Empty)
            {
                return res;
            }
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.NotFound,
                    Message = "User not found."
                };
            }

            return new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "User retrieved successfully.",
                Data = new
                {
                    user.UserId,
                    user.FullName,
                    user.UserName,
                    user.Email,
                    user.DoB,
                    user.Img,
                    user.CreatedAt,
                    user.UpdatedAt,
                    user.RoleId,
                }
            };
        }

        public async Task<ResultModel> SendForgotPasswordCodeAsync(string email)
        {
            var result = new ResultModel
            {
                IsSuccess = false,
                Code = (int)HttpStatusCode.BadRequest,
                Message = "Email không tồn tại hoặc không hợp lệ."
            };
            var user = await _userRepo.GetByEmailAsync(email);
            if (user == null)
            {
                result.Code = (int)HttpStatusCode.NotFound;
                result.Message = "Không tìm thấy người dùng với email này.";
                return result;
            }
            var code = new Random().Next(100000, 999999).ToString();
            user.VerificationCode = code;
            user.VerificationCodeExpiry = DateTime.UtcNow.AddMinutes(10);
            await _userRepo.UpdateVerifyAsync(user);
            await _emailService.SendVerificationEmailAsync(user.Email, code);
            result.IsSuccess = true;
            result.Code = (int)HttpStatusCode.OK;
            result.Message = "Đã gửi mã xác nhận đến email.";
            return result;
        }

        public async Task<ResultModel> ResetPasswordWithCodeAsync(ChangePasswordModel model)
        {
            var result = new ResultModel
            {
                IsSuccess = false,
                Code = (int)HttpStatusCode.BadRequest,
                Message = "Mã xác nhận không hợp lệ hoặc đã hết hạn."
            };
            var user = await _userRepo.GetByEmailAsync(model.Email);
            if (user == null)
            {
                result.Code = (int)HttpStatusCode.NotFound;
                result.Message = "Không tìm thấy người dùng với email này.";
                return result;
            }
            if (user.VerificationCode != model.Code || user.VerificationCodeExpiry < DateTime.UtcNow)
            {
                result.Code = (int)HttpStatusCode.BadRequest;
                result.Message = "Mã xác nhận không hợp lệ hoặc đã hết hạn.";
                return result;
            }
            user.PasswordHash = HashPass.HashPass.HashPassword(model.NewPassword);
            user.VerificationCode = null;
            user.VerificationCodeExpiry = null;
            await _userRepo.UpdateVerifyAsync(user);
            result.IsSuccess = true;
            result.Code = (int)HttpStatusCode.OK;
            result.Message = "Đổi mật khẩu thành công.";
            return result;
        }

        public async Task<ResultModel> LoginWithGoogleAsync(string accessToken)
        {
            var result = new ResultModel { IsSuccess = false, Code = 400, Message = "Đăng nhập Google thất bại." };

            try
            {
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                var response = await httpClient.GetAsync("https://www.googleapis.com/oauth2/v3/userinfo");
                if (!response.IsSuccessStatusCode)
                {
                    result.Message = "Access token không hợp lệ.";
                    return result;
                }

                var payload = await response.Content.ReadAsStringAsync();
                var payloadObj = System.Text.Json.JsonDocument.Parse(payload).RootElement;

                var email = payloadObj.GetProperty("email").GetString();
                var name = payloadObj.GetProperty("name").GetString();
                var picture = payloadObj.GetProperty("picture").GetString();

                var user = await _userRepo.GetByEmailAsync(email);
                if (user == null)
                {
                    user = new User
                    {
                        UserId = Guid.NewGuid(),
                        Email = email,
                        FullName = name,
                        UserName = email,
                        PasswordHash = Guid.NewGuid().ToString("N"),
                        Img = picture,
                        IsVerified = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        RoleId = 2
                    };
                    await _userRepo.AddAsync(user);
                }

                var token = _authentocateService.GenerateJWT(new LoginResModel
                {
                    UserId = user.UserId,
                    FullName = user.FullName,
                    UserName = user.UserName,
                    Email = user.Email,
                    Img = user.Img
                });

                result.IsSuccess = true;
                result.Code = 200;
                result.Message = "Đăng nhập Google thành công.";
                result.Data = new { Token = token, User = user };
            }
            catch (Exception ex)
            {
                result.Message = $"Lỗi: {ex.Message}";
            }

            return result;
        }

    }
}

using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Repositories.ViewModels.ResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.CloudinaryService
{
    public interface ICloudinaryService
    {
        Task<ResultModel> UploadImageAsync(IFormFile file, string folder = "avatars");
        Task<ResultModel> UploadAvatarAsync(IFormFile file, string userId);
        Task<ResultModel> DeleteImageAsync(string publicId);
        Task<ResultModel> TestUploadAsync(IFormFile file);
    }

    public class CloudinaryService : ICloudinaryService
    {
        private readonly IConfiguration _configuration;
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IConfiguration configuration)
        {
            _configuration = configuration;
            var account = new Account(
                _configuration["Cloudinary:CloudName"],
                _configuration["Cloudinary:ApiKey"],
                _configuration["Cloudinary:ApiSecret"] // Fixed configuration key
            );
            _cloudinary = new Cloudinary(account);
            _cloudinary.Api.Secure = true;
        }

        public async Task<ResultModel> UploadImageAsync(IFormFile file, string folder = "avatars")
        {
            var result = new ResultModel
            {
                IsSuccess = false,
                Code = (int)HttpStatusCode.BadRequest,
                Message = "Upload image failed"
            };

            if (file == null || file.Length == 0)
            {
                result.Message = "File is empty or null";
                return result;
            }

            // Validate file type
            var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
            if (!allowedTypes.Contains(file.ContentType.ToLower()))
            {
                result.Message = "Invalid file type. Only JPEG, PNG, GIF, and WebP are allowed.";
                return result;
            }

            // Validate file size (5MB max)
            if (file.Length > 5 * 1024 * 1024)
            {
                result.Message = "File size too large. Maximum size is 5MB.";
                return result;
            }

            try
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = folder,
                    Overwrite = true,
                    Transformation = new Transformation()
                        .Width(400).Height(400)
                        .Crop("fill")
                        .Quality("auto")
                        .FetchFormat("auto")
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                {
                    result.Message = $"Cloudinary error: {uploadResult.Error.Message}";
                    return result;
                }

                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.OK;
                result.Message = "Upload successful";
                result.Data = new
                {
                    SecureUrl = uploadResult.SecureUrl.ToString(),
                    PublicId = uploadResult.PublicId,
                    Width = uploadResult.Width,
                    Height = uploadResult.Height,
                    Format = uploadResult.Format
                };
            }
            catch (Exception ex)
            {
                result.Code = (int)HttpStatusCode.InternalServerError;
                result.Message = $"Upload failed: {ex.Message}";
            }

            return result;
        }

        public async Task<ResultModel> UploadAvatarAsync(IFormFile file, string userId)
        {
            var result = new ResultModel
            {
                IsSuccess = false,
                Code = (int)HttpStatusCode.BadRequest,
                Message = "Upload avatar failed"
            };

            if (file == null || file.Length == 0)
            {
                result.Message = "File is empty or null";
                return result;
            }

            if (string.IsNullOrEmpty(userId))
            {
                result.Message = "User ID is required";
                return result;
            }

            // Validate file type
            var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
            if (!allowedTypes.Contains(file.ContentType.ToLower()))
            {
                result.Message = "Invalid file type. Only JPEG, PNG, GIF, and WebP are allowed.";
                return result;
            }

            // Validate file size (5MB max)
            if (file.Length > 5 * 1024 * 1024)
            {
                result.Message = "File size too large. Maximum size is 5MB.";
                return result;
            }

            try
            {
                using var stream = file.OpenReadStream();
                var publicId = $"avatars/user_{userId}";
                
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, stream),
                    PublicId = publicId,
                    Overwrite = true,
                    Transformation = new Transformation()
                        .Width(400).Height(400)
                        .Crop("fill")
                        .Quality("auto")
                        .FetchFormat("auto")
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                {
                    result.Message = $"Cloudinary error: {uploadResult.Error.Message}";
                    return result;
                }

                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.OK;
                result.Message = "Avatar uploaded successfully";
                result.Data = new
                {
                    SecureUrl = uploadResult.SecureUrl.ToString(),
                    PublicId = uploadResult.PublicId,
                    Width = uploadResult.Width,
                    Height = uploadResult.Height,
                    Format = uploadResult.Format
                };
            }
            catch (Exception ex)
            {
                result.Code = (int)HttpStatusCode.InternalServerError;
                result.Message = $"Upload failed: {ex.Message}";
            }

            return result;
        }

        public async Task<ResultModel> DeleteImageAsync(string publicId)
        {
            var result = new ResultModel
            {
                IsSuccess = false,
                Code = (int)HttpStatusCode.BadRequest,
                Message = "Delete image failed"
            };

            if (string.IsNullOrEmpty(publicId))
            {
                result.Message = "Public ID is required";
                return result;
            }

            try
            {
                var deletionParams = new DeletionParams(publicId);
                var deleteResult = await _cloudinary.DestroyAsync(deletionParams);

                if (deleteResult.Error != null)
                {
                    result.Message = $"Cloudinary error: {deleteResult.Error.Message}";
                    return result;
                }

                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.OK;
                result.Message = "Image deleted successfully";
                result.Data = new
                {
                    Result = deleteResult.Result,
                    PublicId = publicId
                };
            }
            catch (Exception ex)
            {
                result.Code = (int)HttpStatusCode.InternalServerError;
                result.Message = $"Delete failed: {ex.Message}";
            }

            return result;
        }

        public async Task<ResultModel> TestUploadAsync(IFormFile file)
        {
            var result = new ResultModel
            {
                IsSuccess = false,
                Code = (int)HttpStatusCode.BadRequest,
                Message = "Test upload failed"
            };

            if (file == null || file.Length == 0)
            {
                result.Message = "File is empty or null";
                return result;
            }

            try
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = "test",
                    PublicId = $"test_{Guid.NewGuid()}",
                    Overwrite = true
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                {
                    result.Message = $"Cloudinary error: {uploadResult.Error.Message}";
                    return result;
                }

                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.OK;
                result.Message = "Test upload successful";
                result.Data = new
                {
                    SecureUrl = uploadResult.SecureUrl.ToString(),
                    PublicId = uploadResult.PublicId,
                    Width = uploadResult.Width,
                    Height = uploadResult.Height,
                    Format = uploadResult.Format,
                    Bytes = uploadResult.Bytes
                };
            }
            catch (Exception ex)
            {
                result.Code = (int)HttpStatusCode.InternalServerError;
                result.Message = $"Test upload failed: {ex.Message}";
            }

            return result;
        }
    }
}

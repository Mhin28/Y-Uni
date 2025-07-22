using Repositories.Models;
using Repositories.Repositories;
using Repositories.ViewModels.EventCategoryModel;
using Repositories.ViewModels.ResultModels;
using Services.Services.UserContextService;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Services.Services.EventCategoryService
{
    public class EventCategoryService : IEventCategoryService
    {
        private readonly IEventCategoryRepo _repo;
        private readonly IUserContextService _userContext;

        public EventCategoryService(IEventCategoryRepo repo, IUserContextService userContext)
        {
            _repo = repo;
            _userContext = userContext;
        }

        public async Task<ResultModel> GetAllAsync()
        {
            var result = new ResultModel();
            try
            {
                var categories = await _repo.GetAllOrderedByNameAsync();

                var categoryModels = categories.Select(c => new EventCategoryModel
                {
                    EvCategoryId = c.EvCategoryId,
                    CategoryName = c.CategoryName,
                    Description = c.Description,
                    UserId = c.UserId
                }).ToList();

                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.OK;
                result.Data = categoryModels;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = (int)HttpStatusCode.InternalServerError;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<ResultModel> GetByIdAsync(Guid id)
        {
            var result = new ResultModel();
            try
            {
                var category = await _repo.GetByIdAsync(id);
                if (category == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "Event category not found.";
                    return result;
                }

                var categoryModel = new EventCategoryModel
                {
                    EvCategoryId = category.EvCategoryId,
                    CategoryName = category.CategoryName,
                    Description = category.Description,
                    UserId = category.UserId
                };

                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.OK;
                result.Data = categoryModel;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = (int)HttpStatusCode.InternalServerError;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<ResultModel> AddAsync(PostEventCategoryModel model)
        {
            var result = new ResultModel
            {
                IsSuccess = false,
                Code = (int)HttpStatusCode.BadRequest,
                Message = "Invalid request"
            };

            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(model.CategoryName))
                {
                    result.Message = "Category name is required";
                    return result;
                }

                // Get userId from token automatically
                var currentUserId = _userContext.GetCurrentUserId();

                // Check if category name already exists for this user
                var nameExists = await _repo.CategoryNameExistsForUserAsync(model.CategoryName, currentUserId);
                if (nameExists)
                {
                    result.Message = "Category name already exists for this user";
                    return result;
                }

                var eventCategory = new EventCategory
                {
                    EvCategoryId = Guid.NewGuid(),
                    CategoryName = model.CategoryName,
                    Description = model.Description,
                    UserId = currentUserId
                };

                await _repo.CreateAsync(eventCategory);

                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.Created;
                result.Message = "Event category created successfully";
                result.Data = new EventCategoryModel
                {
                    EvCategoryId = eventCategory.EvCategoryId,
                    CategoryName = eventCategory.CategoryName,
                    Description = eventCategory.Description,
                    UserId = eventCategory.UserId
                };
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }

        public async Task<ResultModel> GetByUserIdAsync(Guid userId)
        {
            var result = new ResultModel();
            try
            {
                var categories = await _repo.GetCategoriesByUserIdAsync(userId);

                var categoryModels = categories.Select(c => new EventCategoryModel
                {
                    EvCategoryId = c.EvCategoryId,
                    CategoryName = c.CategoryName,
                    Description = c.Description,
                    UserId = c.UserId
                }).ToList();

                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.OK;
                result.Data = categoryModels;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = (int)HttpStatusCode.InternalServerError;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<ResultModel> GetMyCategories()
        {
            var result = new ResultModel();
            try
            {
                var currentUserId = _userContext.GetCurrentUserId();
                var categories = await _repo.GetCategoriesByUserIdAsync(currentUserId);
                
                var categoryModels = categories.Select(c => new EventCategoryModel
                {
                    EvCategoryId = c.EvCategoryId,
                    CategoryName = c.CategoryName,
                    Description = c.Description,
                    UserId = c.UserId
                }).ToList();
                
                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.OK;
                result.Data = categoryModels;
            }
            catch (UnauthorizedAccessException ex)
            {
                result.IsSuccess = false;
                result.Code = (int)HttpStatusCode.Unauthorized;
                result.Message = ex.Message;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = (int)HttpStatusCode.InternalServerError;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<ResultModel> UpdateAsync(EventCategoryModel model)
        {
            var result = new ResultModel
            {
                IsSuccess = false,
                Code = (int)HttpStatusCode.BadRequest,
                Message = "Update failed"
            };

            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(model.CategoryName))
                {
                    result.Message = "Category name is required";
                    return result;
                }

                var category = await _repo.GetByIdAsync(model.EvCategoryId);
                if (category == null)
                {
                    result.Message = "Event category not found";
                    return result;
                }

                category.CategoryName = model.CategoryName;
                category.Description = model.Description;

                await _repo.UpdateAsync(category);

                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.OK;
                result.Message = "Event category updated successfully";
                result.Data = model;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }

        public async Task<ResultModel> DeleteAsync(Guid id)
        {
            var result = new ResultModel
            {
                IsSuccess = false,
                Code = (int)HttpStatusCode.BadRequest,
                Message = "Delete failed"
            };

            try
            {
                var category = await _repo.GetByIdAsync(id);
                if (category == null)
                {
                    result.Message = "Event category not found";
                    return result;
                }

                // Check if this category is used by any events
                var isInUse = await _repo.IsCategoryInUseAsync(id);
                if (isInUse)
                {
                    result.Message = "Cannot delete category that is in use by events";
                    return result;
                }

                await _repo.RemoveAsync(category);

                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.OK;
                result.Message = "Deleted successfully";
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }
    }
}
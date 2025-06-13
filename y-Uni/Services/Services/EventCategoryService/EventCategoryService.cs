using Repositories.Models;
using Repositories.Repositories;
using Repositories.ViewModels.EventCategoryModel;
using Repositories.ViewModels.ResultModels;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Services.Services.EventCategoryService
{
    public class EventCategoryService : IEventCategoryService
    {
        private readonly IEventCategoryRepo _repo;

        public EventCategoryService(IEventCategoryRepo repo)
        {
            _repo = repo;
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
                    Description = c.Description
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
                    Description = category.Description
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

                var eventCategory = new EventCategory
                {
                    EvCategoryId = Guid.NewGuid(),
                    CategoryName = model.CategoryName,
                    Description = model.Description
                };
                
                await _repo.CreateAsync(eventCategory);
                
                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.Created;
                result.Data = new EventCategoryModel
                {
                    EvCategoryId = eventCategory.EvCategoryId,
                    CategoryName = eventCategory.CategoryName,
                    Description = eventCategory.Description
                };
            }
            catch (Exception ex)
            {
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
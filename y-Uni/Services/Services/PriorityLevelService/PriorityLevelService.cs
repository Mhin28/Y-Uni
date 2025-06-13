using Repositories.Models;
using Repositories.Repositories;
using Repositories.ViewModels.PriorityLevelModel;
using Repositories.ViewModels.ResultModels;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Services.Services.PriorityLevelService
{
    public class PriorityLevelService : IPriorityLevelService
    {
        private readonly IPriorityLevelRepo _repo;

        public PriorityLevelService(IPriorityLevelRepo repo)
        {
            _repo = repo;
        }

        public async Task<ResultModel> GetAllAsync()
        {
            var result = new ResultModel();
            try
            {
                var priorityLevels = await _repo.GetAllOrderedByPriorityAsync();
                
                var priorityLevelModels = priorityLevels.Select(p => new PriorityLevelModel
                {
                    PriorityId = p.PriorityId,
                    LevelName = p.LevelName,
                    ColorCode = p.ColorCode
                }).ToList();
                
                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.OK;
                result.Data = priorityLevelModels;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = (int)HttpStatusCode.InternalServerError;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<ResultModel> GetByIdAsync(byte id)
        {
            var result = new ResultModel();
            try
            {
                var priorityLevel = await _repo.GetByPriorityIdAsync(id);
                if (priorityLevel == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "Priority level not found.";
                    return result;
                }

                var priorityLevelModel = new PriorityLevelModel
                {
                    PriorityId = priorityLevel.PriorityId,
                    LevelName = priorityLevel.LevelName,
                    ColorCode = priorityLevel.ColorCode
                };

                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.OK;
                result.Data = priorityLevelModel;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = (int)HttpStatusCode.InternalServerError;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<ResultModel> AddAsync(PostPriorityLevelModel model)
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
                if (string.IsNullOrWhiteSpace(model.LevelName))
                {
                    result.Message = "Level name is required";
                    return result;
                }

                if (string.IsNullOrWhiteSpace(model.ColorCode))
                {
                    result.Message = "Color code is required";
                    return result;
                }

                // Find the next available priority ID
                var allPriorities = await _repo.GetAllAsync();
                byte nextId = 1;
                if (allPriorities.Any())
                {
                    nextId = (byte)(allPriorities.Max(p => p.PriorityId) + 1);
                }

                var priorityLevel = new PriorityLevel
                {
                    PriorityId = nextId,
                    LevelName = model.LevelName,
                    ColorCode = model.ColorCode
                };
                
                await _repo.CreateAsync(priorityLevel);
                
                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.Created;
                result.Data = new PriorityLevelModel
                {
                    PriorityId = priorityLevel.PriorityId,
                    LevelName = priorityLevel.LevelName,
                    ColorCode = priorityLevel.ColorCode
                };
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }

        public async Task<ResultModel> UpdateAsync(PriorityLevelModel model)
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
                if (string.IsNullOrWhiteSpace(model.LevelName))
                {
                    result.Message = "Level name is required";
                    return result;
                }

                if (string.IsNullOrWhiteSpace(model.ColorCode))
                {
                    result.Message = "Color code is required";
                    return result;
                }

                var priorityLevel = await _repo.GetByPriorityIdAsync(model.PriorityId);
                if (priorityLevel == null)
                {
                    result.Message = "Priority level not found";
                    return result;
                }

                priorityLevel.LevelName = model.LevelName;
                priorityLevel.ColorCode = model.ColorCode;

                await _repo.UpdateAsync(priorityLevel);

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

        public async Task<ResultModel> DeleteAsync(byte id)
        {
            var result = new ResultModel
            {
                IsSuccess = false,
                Code = (int)HttpStatusCode.BadRequest,
                Message = "Delete failed"
            };

            try
            {
                var priorityLevel = await _repo.GetByPriorityIdAsync(id);
                if (priorityLevel == null)
                {
                    result.Message = "Priority level not found";
                    return result;
                }

                // Check if this priority level is used by any assignments
                if (priorityLevel.Assignments.Any())
                {
                    result.Message = "Cannot delete priority level that is in use by assignments";
                    return result;
                }

                await _repo.RemoveAsync(priorityLevel);

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
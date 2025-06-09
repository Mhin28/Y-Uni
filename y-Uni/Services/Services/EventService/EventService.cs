using Repositories.Models;
using Repositories.Repositories;
using Repositories.ViewModels.EventModel;
using Repositories.ViewModels.ResultModels;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Services.Services.EventService
{
    public class EventService : IEventService
    {
        private readonly IEventRepo _repo;

        public EventService(IEventRepo repo)
        {
            _repo = repo;
        }

        public async Task<ResultModel> GetAllAsync()
        {
            var result = new ResultModel();
            try
            {
                var events = await _repo.GetAllAsync();
                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.OK;
                result.Data = events;
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
                var eventEntity = await _repo.GetByIdAsync(id);
                if (eventEntity == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "Event not found.";
                    return result;
                }
                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.OK;
                result.Data = eventEntity;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = (int)HttpStatusCode.InternalServerError;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<ResultModel> GetByUserIdAsync(Guid userId)
        {
            var result = new ResultModel();
            try
            {
                var events = await _repo.GetEventsByUserIdAsync(userId);
                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.OK;
                result.Data = events;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = (int)HttpStatusCode.InternalServerError;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<ResultModel> GetUpcomingByUserIdAsync(Guid userId, DateTime startDate, DateTime endDate)
        {
            var result = new ResultModel();
            try
            {
                var events = await _repo.GetUpcomingEventsByUserIdAsync(userId, startDate, endDate);
                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.OK;
                result.Data = events;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = (int)HttpStatusCode.InternalServerError;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<ResultModel> GetByCategoryAsync(Guid categoryId)
        {
            var result = new ResultModel();
            try
            {
                var events = await _repo.GetEventsByCategoryAsync(categoryId);
                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.OK;
                result.Data = events;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = (int)HttpStatusCode.InternalServerError;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<ResultModel> AddAsync(PostEventModel model)
        {
            var result = new ResultModel
            {
                IsSuccess = false,
                Code = (int)HttpStatusCode.BadRequest,
                Message = "Invalid request"
            };

            try
            {
                // Validate end time is after start time
                if (model.EndDateTime <= model.StartDateTime)
                {
                    result.Message = "End time must be after start time";
                    return result;
                }

                var eventEntity = new Event
                {
                    EventId = Guid.NewGuid(),
                    Title = model.Title,
                    Description = model.Description,
                    StartDateTime = model.StartDateTime,
                    EndDateTime = model.EndDateTime,
                    RecurrencePattern = model.RecurrencePattern ?? "none",
                    RecurrenceEndDate = model.RecurrenceEndDate,
                    EvCategoryId = model.EvCategoryId,
                    UserId = model.UserId
                };
                
                await _repo.CreateAsync(eventEntity);
                
                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.Created;
                result.Data = eventEntity;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }

        public async Task<ResultModel> UpdateAsync(EventModel model)
        {
            var result = new ResultModel
            {
                IsSuccess = false,
                Code = (int)HttpStatusCode.BadRequest,
                Message = "Update failed"
            };

            try
            {
                // Validate end time is after start time
                if (model.EndDateTime <= model.StartDateTime)
                {
                    result.Message = "End time must be after start time";
                    return result;
                }

                var eventEntity = await _repo.GetByIdAsync(model.EventId);
                if (eventEntity == null)
                {
                    result.Message = "Event not found";
                    return result;
                }

                eventEntity.Title = model.Title;
                eventEntity.Description = model.Description;
                eventEntity.StartDateTime = model.StartDateTime;
                eventEntity.EndDateTime = model.EndDateTime;
                eventEntity.RecurrencePattern = model.RecurrencePattern;
                eventEntity.RecurrenceEndDate = model.RecurrenceEndDate;
                eventEntity.EvCategoryId = model.EvCategoryId;

                await _repo.UpdateAsync(eventEntity);

                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.OK;
                result.Data = eventEntity;
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
                var eventEntity = await _repo.GetByIdAsync(id);
                if (eventEntity == null)
                {
                    result.Message = "Event not found";
                    return result;
                }

                await _repo.RemoveAsync(eventEntity);

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
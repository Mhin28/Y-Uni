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
        private readonly IReminderRepo _reminderRepo;
        private readonly IReminderTemplateRepo _reminderTemplateRepo;

        public EventService(IEventRepo repo, IReminderRepo reminderRepo, IReminderTemplateRepo reminderTemplateRepo)
        {
            _repo = repo;
            _reminderRepo = reminderRepo;
            _reminderTemplateRepo = reminderTemplateRepo;
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
                
                // Generate virtual occurrences for recurring events
                var expandedEvents = new List<object>();
                foreach (var eventItem in events)
                {
                    if (eventItem.RecurrencePattern != null && eventItem.RecurrencePattern != "none")
                    {
                        // Generate occurrences for the next 3 months
                        var occurrences = GenerateEventOccurrences(eventItem, DateTime.Now, DateTime.Now.AddMonths(3));
                        expandedEvents.AddRange(occurrences);
                    }
                    else
                    {
                        // Single occurrence event
                        expandedEvents.Add(eventItem);
                    }
                }
                
                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.OK;
                result.Data = expandedEvents;
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

        public async Task<ResultModel> AddAsync(PostEventModel model, Guid userId)
        {
            var result = new ResultModel
            {
                IsSuccess = false,
                Code = (int)HttpStatusCode.BadRequest,
                Message = "Invalid request"
            };

            try
            {
                // Validate start time is not in the past
                if (model.StartDateTime <= DateTime.Now)
                {
                    result.Message = "Start time cannot be in the past";
                    return result;
                }

                // Validate end time is after start time
                if (model.EndDateTime <= model.StartDateTime)
                {
                    result.Message = "End time must be after start time";
                    return result;
                }

                // Validate event category is required and exists
                if (!model.EvCategoryId.HasValue)
                {
                    result.Message = "Event category ID is required";
                    return result;
                }

                var categoryExists = await _repo.CheckCategoryExistsAsync(model.EvCategoryId.Value);
                if (!categoryExists)
                {
                    result.Message = "Event category not found";
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
                    UserId = userId // Use userId from JWT token
                };
                
                await _repo.CreateAsync(eventEntity);
                
                // Auto-create email reminder based on default template
                await CreateDefaultReminderForEvent(eventEntity);
                
                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.Created;
                result.Message = "Event created successfully with reminder";
                result.Data = eventEntity;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }

        public async Task<ResultModel> UpdateAsync(UpdateEventModel model)
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
                result.Message = "Event updated successfully";
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
                    result.Code = (int)HttpStatusCode.NotFound;
                    return result;
                }

                // Delete related reminders first to avoid foreign key constraint violations
                var relatedReminders = await _reminderRepo.GetRemindersByEventIdAsync(id);
                foreach (var reminder in relatedReminders)
                {
                    await _reminderRepo.RemoveAsync(reminder);
                }

                // Now delete the event
                await _repo.RemoveAsync(eventEntity);

                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.OK;
                result.Message = $"Event and {relatedReminders.Count()} related reminder(s) deleted successfully";
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }

        private async Task CreateDefaultReminderForEvent(Event eventEntity)
        {
            try
            {
                // Get customizable template or use default logic
                var defaultTemplate = await _reminderTemplateRepo.GetDefaultEventTemplateAsync();
                
                // Default: 30 minutes before event start time
                int minutesBeforeStart = 30;
                if (defaultTemplate != null && defaultTemplate.TriggerValue.HasValue)
                {
                    minutesBeforeStart = defaultTemplate.TriggerValue.Value;
                }

                var reminderTime = eventEntity.StartDateTime.AddMinutes(-minutesBeforeStart);
                
                // Only create reminder if it's in the future
                if (reminderTime > DateTime.Now)
                {
                    var reminder = new Reminder
                    {
                        ReminderId = Guid.NewGuid(),
                        EventId = eventEntity.EventId,
                        UserId = eventEntity.UserId,
                        TemplateId = defaultTemplate?.TemplateId,
                        ReminderTime = reminderTime,
                        Status = "pending",
                        NotificationChannel = "email"
                    };

                    await _reminderRepo.CreateAsync(reminder);
                    
                    System.Console.WriteLine($"SUCCESS: Event reminder created for {reminderTime} ({minutesBeforeStart} minutes before start time)");
                }
                else
                {
                    System.Console.WriteLine($"SKIPPED: Event reminder time {reminderTime} is in the past (would be {minutesBeforeStart} minutes before start time)");
                }
            }
            catch (Exception ex)
            {
                // Log error but don't fail event creation
                System.Console.WriteLine($"ERROR creating event reminder: {ex.Message}");
                System.Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        private List<object> GenerateEventOccurrences(Event baseEvent, DateTime startDate, DateTime endDate)
        {
            var occurrences = new List<object>();
            
            if (baseEvent.RecurrencePattern == "none" || string.IsNullOrEmpty(baseEvent.RecurrencePattern))
            {
                // Single occurrence
                if (baseEvent.StartDateTime >= startDate && baseEvent.StartDateTime <= endDate)
                {
                    occurrences.Add(new
                    {
                        EventId = baseEvent.EventId,
                        Title = baseEvent.Title,
                        Description = baseEvent.Description,
                        StartDateTime = baseEvent.StartDateTime,
                        EndDateTime = baseEvent.EndDateTime,
                        RecurrencePattern = baseEvent.RecurrencePattern,
                        EvCategoryId = baseEvent.EvCategoryId,
                        UserId = baseEvent.UserId,
                        IsRecurring = false,
                        OriginalEventId = baseEvent.EventId,
                        OccurrenceDate = baseEvent.StartDateTime.Date
                    });
                }
                return occurrences;
            }
            
            // Calculate recurring occurrences
            var current = baseEvent.StartDateTime;
            var duration = baseEvent.EndDateTime - baseEvent.StartDateTime;
            int occurrenceCount = 0;
            const int maxOccurrences = 100; // Safety limit
            
            while (current <= endDate && occurrenceCount < maxOccurrences)
            {
                // Check if we've reached the recurrence end date
                if (baseEvent.RecurrenceEndDate.HasValue && current.Date > baseEvent.RecurrenceEndDate.Value.ToDateTime(TimeOnly.MinValue))
                {
                    break;
                }
                
                if (current >= startDate)
                {
                    occurrences.Add(new
                    {
                        EventId = Guid.NewGuid(), // Virtual occurrence ID
                        Title = baseEvent.Title,
                        Description = baseEvent.Description,
                        StartDateTime = current,
                        EndDateTime = current + duration,
                        RecurrencePattern = baseEvent.RecurrencePattern,
                        EvCategoryId = baseEvent.EvCategoryId,
                        UserId = baseEvent.UserId,
                        IsRecurring = true,
                        OriginalEventId = baseEvent.EventId,
                        OccurrenceDate = current.Date
                    });
                }
                
                // Move to next occurrence based on pattern
                current = baseEvent.RecurrencePattern.ToLower() switch
                {
                    "daily" => current.AddDays(1),
                    "weekly" => current.AddDays(7),
                    "monthly" => current.AddMonths(1),
                    "yearly" => current.AddYears(1),
                    _ => current.AddYears(100) // Break the loop for unknown patterns
                };
                
                occurrenceCount++;
            }
            
            return occurrences;
        }
    }
} 
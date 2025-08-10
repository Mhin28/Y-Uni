using Repositories.Models;
using Repositories.Repositories;
using Repositories.ViewModels.AssignmentModel;
using Repositories.ViewModels.ResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.AssignmentService
{
    public class AssignmentService : IAssignmentService
    {
        private readonly IAssignmentRepo _repo;
        private readonly IReminderRepo _reminderRepo;
        private readonly IReminderTemplateRepo _reminderTemplateRepo;

        public AssignmentService(IAssignmentRepo repo, IReminderRepo reminderRepo, IReminderTemplateRepo reminderTemplateRepo)
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
                var assignments = await _repo.GetAllAsync();
                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.OK;
                result.Data = assignments;
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
                var assignment = await _repo.GetByIdAsync(id);
                if (assignment == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "Assignment not found.";
                    return result;
                }
                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.OK;
                result.Data = assignment;
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
                var assignments = await _repo.GetAssignmentsByUserIdAsync(userId);
                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.OK;
                result.Data = assignments;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = (int)HttpStatusCode.InternalServerError;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<ResultModel> GetBySubjectAsync(Guid subjectId)
        {
            var result = new ResultModel();
            try
            {
                var assignments = await _repo.GetAssignmentsBySubjectAsync(subjectId);
                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.OK;
                result.Data = assignments;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = (int)HttpStatusCode.InternalServerError;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<ResultModel> GetUpcomingByUserIdAsync(Guid userId, DateTime? dueDate = null)
        {
            var result = new ResultModel();
            try
            {
                var targetDate = dueDate ?? DateTime.Now.AddDays(7);
                var assignments = await _repo.GetUpcomingAssignmentsByUserIdAsync(userId, targetDate);
                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.OK;
                result.Data = assignments;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = (int)HttpStatusCode.InternalServerError;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<ResultModel> GetByStatusAsync(Guid userId, string status)
        {
            var result = new ResultModel();
            try
            {
                var assignments = await _repo.GetAssignmentsByStatusAsync(userId, status);
                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.OK;
                result.Data = assignments;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = (int)HttpStatusCode.InternalServerError;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<ResultModel> AddAsync(PostAssignmentModel model, Guid userId)
        {
            var result = new ResultModel
            {
                IsSuccess = false,
                Code = (int)HttpStatusCode.BadRequest,
                Message = "Invalid request"
            };

            try
            {
                // Validate due date is not in the past
                if (model.DueDate <= DateTime.Now)
                {
                    result.Message = "Due date cannot be in the past";
                    return result;
                }


                var assignment = new Assignment
                {
                    AssignmentId = Guid.NewGuid(),
                    Title = model.Title,
                    Description = model.Description,
                    DueDate = model.DueDate,
                    Status = "not_started",
                    PriorityId = model.PriorityId ?? 3,
                    EstimatedTime = model.EstimatedTime,
                    SubjectId = model.SubjectId,
                    UserId = userId
                };
                
                await _repo.CreateAsync(assignment);
                
                // Auto-create email reminder
                await CreateDefaultReminderForAssignment(assignment);
                
                // Reload with navigation properties
                var createdAssignment = await _repo.GetByIdWithIncludesAsync(assignment.AssignmentId);
                
                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.Created;
                result.Message = "Assignment created successfully with reminder";
                result.Data = createdAssignment ?? assignment;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }

        public async Task<ResultModel> UpdateAsync(UpdateAssignmentModel model)
        {
            var result = new ResultModel
            {
                IsSuccess = false,
                Code = (int)HttpStatusCode.BadRequest,
                Message = "Update failed"
            };

            try
            {
                var assignment = await _repo.GetByIdAsync(model.AssignmentId);
                if (assignment == null)
                {
                    result.Message = "Assignment not found";
                    return result;
                }

                assignment.Title = model.Title;
                assignment.Description = model.Description;
                assignment.DueDate = model.DueDate;
                assignment.Status = model.Status;
                assignment.PriorityId = model.PriorityId;
                assignment.EstimatedTime = model.EstimatedTime;
                assignment.SubjectId = model.SubjectId;
                assignment.CompletedDate = model.CompletedDate;

                await _repo.UpdateAsync(assignment);

                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.OK;
                result.Message = "Assignment updated successfully";
                result.Data = assignment;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }

        public async Task<ResultModel> UpdateStatusAsync(Guid id, string status)
        {
            var result = new ResultModel
            {
                IsSuccess = false,
                Code = (int)HttpStatusCode.BadRequest,
                Message = "Status update failed"
            };

            try
            {
                var assignment = await _repo.GetByIdAsync(id);
                if (assignment == null)
                {
                    result.Message = "Assignment not found";
                    return result;
                }

                assignment.Status = status;
                if (status == "completed")
                {
                    assignment.CompletedDate = DateTime.Now;
                }

                await _repo.UpdateAsync(assignment);

                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.OK;
                result.Message = "Status updated successfully";
                result.Data = assignment;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }

        public async Task<ResultModel> CompleteAssignmentAsync(Guid id)
        {
            return await UpdateStatusAsync(id, "completed");
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
                var assignment = await _repo.GetByIdAsync(id);
                if (assignment == null)
                {
                    result.Message = "Assignment not found";
                    result.Code = (int)HttpStatusCode.NotFound;
                    return result;
                }

                // Delete related reminders first to avoid foreign key constraint violations
                var relatedReminders = await _reminderRepo.GetRemindersByAssignmentIdAsync(id);
                foreach (var reminder in relatedReminders)
                {
                    await _reminderRepo.RemoveAsync(reminder);
                }

                // Now delete the assignment
                await _repo.RemoveAsync(assignment);

                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.OK;
                result.Message = $"Assignment and {relatedReminders.Count()} related reminder(s) deleted successfully";
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }

        private async Task CreateDefaultReminderForAssignment(Assignment assignment)
        {
            try
            {
                // Get customizable template or use default logic
                var defaultTemplate = await _reminderTemplateRepo.GetDefaultAssignmentTemplateAsync();
                
                // Default: 2 days (2880 minutes) before due date for assignments
                int minutesBeforeDue = 2880; // 48 hours = 2880 minutes
                if (defaultTemplate != null && defaultTemplate.TriggerValue.HasValue)
                {
                    minutesBeforeDue = defaultTemplate.TriggerValue.Value;
                }

                var reminderTime = assignment.DueDate.AddMinutes(-minutesBeforeDue);
                
                // Only create reminder if it's in the future
                if (reminderTime > DateTime.Now)
                {
                    var reminder = new Reminder
                    {
                        ReminderId = Guid.NewGuid(),
                        AssignmentId = assignment.AssignmentId,
                        UserId = assignment.UserId,
                        TemplateId = defaultTemplate?.TemplateId,
                        ReminderTime = reminderTime,
                        Status = "pending",
                        NotificationChannel = "email"
                    };

                    await _reminderRepo.CreateAsync(reminder);
                    
                    // Convert minutes to human-readable format for logging
                    var hours = minutesBeforeDue / 60;
                    var remainingMinutes = minutesBeforeDue % 60;
                    var timeDescription = hours > 0 ? 
                        (remainingMinutes > 0 ? $"{hours}h {remainingMinutes}m" : $"{hours}h") : 
                        $"{minutesBeforeDue}m";
                    
                    System.Console.WriteLine($"SUCCESS: Assignment reminder created for {reminderTime} ({timeDescription} before due date)");
                }
                else
                {
                    var hours = minutesBeforeDue / 60;
                    var remainingMinutes = minutesBeforeDue % 60;
                    var timeDescription = hours > 0 ? 
                        (remainingMinutes > 0 ? $"{hours}h {remainingMinutes}m" : $"{hours}h") : 
                        $"{minutesBeforeDue}m";
                    
                    System.Console.WriteLine($"SKIPPED: Assignment reminder time {reminderTime} is in the past (would be {timeDescription} before due date)");
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"ERROR creating assignment reminder: {ex.Message}");
            }
        }
    }
}
using Repositories.Models;
using Repositories.Repositories;
using Repositories.ViewModels.AssignmentModel;
using Repositories.ViewModels.ResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Services.Services.AssignmentService
{
    public class AssignmentService : IAssignmentService
    {
        private readonly IAssignmentRepo _repo;

        public AssignmentService(IAssignmentRepo repo)
        {
            _repo = repo;
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
                var endDate = dueDate ?? DateTime.Now.AddDays(7);
                var assignments = await _repo.GetUpcomingAssignmentsByUserIdAsync(userId, endDate);
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

        public async Task<ResultModel> AddAsync(PostAssignmentModel model)
        {
            var result = new ResultModel
            {
                IsSuccess = false,
                Code = (int)HttpStatusCode.BadRequest,
                Message = "Invalid request"
            };

            try
            {
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
                    UserId = model.UserId
                };
                
                await _repo.CreateAsync(assignment);
                
                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.Created;
                result.Data = assignment;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }

        public async Task<ResultModel> UpdateAsync(AssignmentModel model)
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
                assignment.PriorityId = model.PriorityId;
                assignment.EstimatedTime = model.EstimatedTime;
                assignment.SubjectId = model.SubjectId;

                await _repo.UpdateAsync(assignment);

                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.OK;
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
                Message = "Update failed"
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
                
                if (status == "completed" && assignment.CompletedDate == null)
                {
                    assignment.CompletedDate = DateTime.Now;
                }
                else if (status != "completed")
                {
                    assignment.CompletedDate = null;
                }

                await _repo.UpdateAsync(assignment);

                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.OK;
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
            var result = new ResultModel
            {
                IsSuccess = false,
                Code = (int)HttpStatusCode.BadRequest,
                Message = "Update failed"
            };

            try
            {
                var assignment = await _repo.GetByIdAsync(id);
                if (assignment == null)
                {
                    result.Message = "Assignment not found";
                    return result;
                }

                assignment.Status = "completed";
                assignment.CompletedDate = DateTime.Now;

                await _repo.UpdateAsync(assignment);

                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.OK;
                result.Data = assignment;
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
                var assignment = await _repo.GetByIdAsync(id);
                if (assignment == null)
                {
                    result.Message = "Assignment not found";
                    return result;
                }

                await _repo.RemoveAsync(assignment);

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
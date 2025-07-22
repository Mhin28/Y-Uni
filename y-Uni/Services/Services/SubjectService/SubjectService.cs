using Repositories.Models;
using Repositories.Repositories;
using Repositories.ViewModels.ResultModels;
using Repositories.ViewModels.SubjectModel;
using Services.Services.UserContextService;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Services.Services.SubjectService
{
    public class SubjectService : ISubjectService
    {
        private readonly ISubjectRepo _repo;
        private readonly IUserContextService _userContext;

        public SubjectService(ISubjectRepo repo, IUserContextService userContext)
        {
            _repo = repo;
            _userContext = userContext;
        }

        public async Task<ResultModel> GetAllAsync()
        {
            var result = new ResultModel();
            try
            {
                var subjects = await _repo.GetAllOrderedByNameAsync();

                var subjectModels = subjects.Select(s => new SubjectModel
                {
                    SubjectId = s.SubjectId,
                    SubjectName = s.SubjectName,
                    Description = s.Description,
                    UserId = s.UserId
                }).ToList();

                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.OK;
                result.Data = subjectModels;
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
                var subject = await _repo.GetByIdAsync(id);
                if (subject == null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.NotFound;
                    result.Message = "Subject not found.";
                    return result;
                }

                var subjectModel = new SubjectModel
                {
                    SubjectId = subject.SubjectId,
                    SubjectName = subject.SubjectName,
                    Description = subject.Description,
                    UserId = subject.UserId
                };

                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.OK;
                result.Data = subjectModel;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = (int)HttpStatusCode.InternalServerError;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<ResultModel> AddAsync(PostSubjectModel model)
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
                if (string.IsNullOrWhiteSpace(model.SubjectName))
                {
                    result.Message = "Subject name is required";
                    return result;
                }

                // Get userId from token automatically
                var currentUserId = _userContext.GetCurrentUserId();

                // Check if subject name already exists for this user
                var nameExists = await _repo.SubjectNameExistsForUserAsync(model.SubjectName, currentUserId);
                if (nameExists)
                {
                    result.Message = "Subject name already exists for this user";
                    return result;
                }

                var subject = new Subject
                {
                    SubjectId = Guid.NewGuid(),
                    SubjectName = model.SubjectName,
                    Description = model.Description,
                    UserId = currentUserId
                };

                await _repo.CreateAsync(subject);

                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.Created;
                result.Data = new SubjectModel
                {
                    SubjectId = subject.SubjectId,
                    SubjectName = subject.SubjectName,
                    Description = subject.Description,
                    UserId = subject.UserId
                };
                result.Message = "Subject created successfully";
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
                var subjects = await _repo.GetSubjectsByUserIdAsync(userId);

                var subjectModels = subjects.Select(s => new SubjectModel
                {
                    SubjectId = s.SubjectId,
                    SubjectName = s.SubjectName,
                    Description = s.Description,
                    UserId = s.UserId
                }).ToList();

                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.OK;
                result.Data = subjectModels;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = (int)HttpStatusCode.InternalServerError;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<ResultModel> GetMySubjects()
        {
            var result = new ResultModel();
            try
            {
                var currentUserId = _userContext.GetCurrentUserId();
                var subjects = await _repo.GetSubjectsByUserIdAsync(currentUserId);
                
                var subjectModels = subjects.Select(s => new SubjectModel
                {
                    SubjectId = s.SubjectId,
                    SubjectName = s.SubjectName,
                    Description = s.Description,
                    UserId = s.UserId
                }).ToList();
                
                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.OK;
                result.Data = subjectModels;
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

        public async Task<ResultModel> UpdateAsync(SubjectModel model)
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
                if (string.IsNullOrWhiteSpace(model.SubjectName))
                {
                    result.Message = "Subject name is required";
                    return result;
                }

                var subject = await _repo.GetByIdAsync(model.SubjectId);
                if (subject == null)
                {
                    result.Message = "Subject not found";
                    return result;
                }

                subject.SubjectName = model.SubjectName;
                subject.Description = model.Description;

                await _repo.UpdateAsync(subject);

                result.IsSuccess = true;
                result.Code = (int)HttpStatusCode.OK;
                result.Message = "Subject updated successfully";
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
                var subject = await _repo.GetByIdAsync(id);
                if (subject == null)
                {
                    result.Message = "Subject not found";
                    return result;
                }

                // Check if this subject is used by any assignments
                var isInUse = await _repo.IsSubjectInUseAsync(id);
                if (isInUse)
                {
                    result.Message = "Cannot delete subject that is in use by assignments";
                    return result;
                }

                await _repo.RemoveAsync(subject);

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
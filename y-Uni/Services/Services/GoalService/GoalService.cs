using Repositories.Models;
using Repositories.Repositories;
using Repositories.ViewModels.GoalModel;
using Repositories.ViewModels.ResultModels;
using Services.Services.TokenService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Services.Services.GoalService
{
    public class GoalService : IGoalService
    {
        private readonly IGoalRepo _goalRepo;
        private readonly ITokenService _tokenService;
        public GoalService(IGoalRepo goalRepo, ITokenService tokenService)
        {
            _goalRepo = goalRepo;
            _tokenService = tokenService;
        }
        public async Task<ResultModel> GetGoalsByUserAsync(string token, string status = null, DateOnly? from = null, DateOnly? to = null)
        {
            var decoded = _tokenService.decode(token);
            if (decoded == null || string.IsNullOrEmpty(decoded.userid) || decoded.role != "2")
                return new ResultModel { IsSuccess = false, Code = (int)HttpStatusCode.Forbidden, Message = "Không có quyền truy cập" };
            Guid userId = Guid.Parse(decoded.userid);
            var goals = await _goalRepo.GetGoalsByUserIdAsync(userId, status, from, to);
            var data = goals.Select(g => new GoalModel
            {
                GoalId = g.GoalId,
                GoalName = g.GoalName,
                Description = g.Description,
                TargetDate = g.TargetDate,
                Status = g.Status
            }).ToList();
            return new ResultModel { IsSuccess = true, Code = (int)HttpStatusCode.OK, Data = data, Message = "Lấy danh sách goal thành công" };
        }
        public async Task<ResultModel> CreateGoalAsync(string token, PostGoalModel model)
        {
            var decoded = _tokenService.decode(token);
            if (decoded == null || string.IsNullOrEmpty(decoded.userid) || decoded.role != "2")
                return new ResultModel { IsSuccess = false, Code = (int)HttpStatusCode.Forbidden, Message = "Không có quyền truy cập" };
            Guid userId = Guid.Parse(decoded.userid);
            var goal = new Goal
            {
                GoalId = Guid.NewGuid(),
                GoalName = model.GoalName,
                Description = model.Description,
                TargetDate = model.TargetDate,
                Status = model.Status,
                UserId = userId
            };
            var created = await _goalRepo.AddGoalAsync(goal);
            if (created == null)
                return new ResultModel { IsSuccess = false, Code = (int)HttpStatusCode.InternalServerError, Message = "Tạo goal thất bại" };
            return new ResultModel { IsSuccess = true, Code = (int)HttpStatusCode.Created, Message = "Tạo goal thành công", Data = new GoalModel {
                GoalId = created.GoalId,
                GoalName = created.GoalName,
                Description = created.Description,
                TargetDate = created.TargetDate,
                Status = created.Status
            }};
        }
        public async Task<ResultModel> UpdateGoalAsync(string token, Guid goalId, UpdateGoalModel model)
        {
            var decoded = _tokenService.decode(token);
            if (decoded == null || string.IsNullOrEmpty(decoded.userid) || decoded.role != "2")
                return new ResultModel { IsSuccess = false, Code = (int)HttpStatusCode.Forbidden, Message = "Không có quyền truy cập" };
            Guid userId = Guid.Parse(decoded.userid);
            var goal = await _goalRepo.GetGoalByIdAsync(goalId);
            if (goal == null || goal.UserId != userId)
                return new ResultModel { IsSuccess = false, Code = (int)HttpStatusCode.NotFound, Message = "Goal không tồn tại hoặc không thuộc quyền sở hữu" };
            goal.GoalName = model.GoalName;
            goal.Description = model.Description;
            goal.TargetDate = model.TargetDate;
            goal.Status = model.Status;
            var success = await _goalRepo.UpdateGoalAsync(goal);
            if (!success)
                return new ResultModel { IsSuccess = false, Code = (int)HttpStatusCode.InternalServerError, Message = "Cập nhật goal thất bại" };
            return new ResultModel { IsSuccess = true, Code = (int)HttpStatusCode.OK, Message = "Cập nhật goal thành công" };
        }
        public async Task<ResultModel> DeleteGoalAsync(string token, Guid goalId)
        {
            var decoded = _tokenService.decode(token);
            if (decoded == null || string.IsNullOrEmpty(decoded.userid) || decoded.role != "2")
                return new ResultModel { IsSuccess = false, Code = (int)HttpStatusCode.Forbidden, Message = "Không có quyền truy cập" };
            Guid userId = Guid.Parse(decoded.userid);
            var goal = await _goalRepo.GetGoalByIdAsync(goalId);
            if (goal == null || goal.UserId != userId)
                return new ResultModel { IsSuccess = false, Code = (int)HttpStatusCode.NotFound, Message = "Goal không tồn tại hoặc không thuộc quyền sở hữu" };
            var success = await _goalRepo.DeleteGoalAsync(goal);
            if (!success)
                return new ResultModel { IsSuccess = false, Code = (int)HttpStatusCode.InternalServerError, Message = "Xóa goal thất bại" };
            return new ResultModel { IsSuccess = true, Code = (int)HttpStatusCode.OK, Message = "Xóa goal thành công" };
        }
    }
} 
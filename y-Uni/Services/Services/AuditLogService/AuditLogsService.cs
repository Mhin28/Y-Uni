using Repositories.Models;
using Repositories.Repositories;
using Repositories.ViewModels.AuditLogsModel;
using Repositories.ViewModels.ResultModels;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Services.Services.AuditLogService
{
	public class AuditLogsService : IAuditLogsService
	{
		private readonly IAuditLogsRepo _auditLogRepo;

		public AuditLogsService(IAuditLogsRepo auditLogRepo)
		{
			_auditLogRepo = auditLogRepo;
		}

		public async Task<ResultModel> GetByUserIdAsync(Guid userId)
		{
			var result = new ResultModel();
			try
			{
				var logs = await _auditLogRepo.GetByUserIdAsync(userId);
				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.OK;
				result.Data = logs;
			}
			catch (Exception ex)
			{
				result.IsSuccess = false;
				result.Code = (int)HttpStatusCode.InternalServerError;
				result.Message = ex.Message;
			}
			return result;
		}

		public async Task<ResultModel> AddAuditLogAsync(PostAuditLogModel model)
		{
			var result = new ResultModel
			{
				IsSuccess = false,
				Code = (int)HttpStatusCode.BadRequest,
				Message = "Invalid request."
			};
			try
			{
				var auditLog = new AuditLog
				{
					LogId = Guid.NewGuid(),
					UserId = model.UserId,
					ActionTimestamp = model.ActionTimestamp,
					ActionType = model.ActionType,
					NewValues = model.NewValues,
					OldValues = model.OldValues,
					RecordId = model.RecordId,
					TableName = model.TableName
				};
				await _auditLogRepo.CreateAsync(auditLog);
				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.Created;
				result.Data = auditLog;
				result.Message = "Audit log created successfully.";
			}
			catch (Exception ex)
			{
				result.Message = ex.Message;
			}
			return result;
		}

		public async Task<ResultModel> DeleteAuditLogAsync(Guid logId)
		{
			var result = new ResultModel();
			try
			{
				var auditLog = await _auditLogRepo.GetByIdAsync(logId);
				if (auditLog == null)
				{
					result.IsSuccess = false;
					result.Code = (int)HttpStatusCode.NotFound;
					result.Message = "Audit log not found.";
					return result;
				}
				await _auditLogRepo.RemoveAsync(auditLog);
				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.OK;
				result.Message = "Deleted successfully.";
			}
			catch (Exception ex)
			{
				result.IsSuccess = false;
				result.Code = (int)HttpStatusCode.InternalServerError;
				result.Message = ex.Message;
			}
			return result;
		}

		public async Task<ResultModel> UpdateAuditLogAsync(AuditLogModel model)
		{
			var result = new ResultModel();

			if (model == null)
			{
				result.IsSuccess = false;
				result.Code = (int)HttpStatusCode.BadRequest;
				result.Message = "Invalid audit log model.";
				return result;
			}

			try
			{
				var existingLog = await _auditLogRepo.GetByIdAsync(model.LogId);
				if (existingLog == null)
				{
					result.IsSuccess = false;
					result.Code = (int)HttpStatusCode.NotFound;
					result.Message = "Audit log not found.";
					return result;
				}

				existingLog.ActionTimestamp = model.ActionTimestamp;
				existingLog.ActionType = model.ActionType;
				existingLog.NewValues = model.NewValues;
				existingLog.OldValues = model.OldValues;
				existingLog.RecordId = model.RecordId;
				existingLog.TableName = model.TableName;
				existingLog.UserId = model.UserId;

				await _auditLogRepo.UpdateAsync(existingLog);

				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.OK;
				result.Data = existingLog;
				result.Message = "Updated successfully.";
			}
			catch (Exception ex)
			{

				result.IsSuccess = false;
				result.Code = (int)HttpStatusCode.InternalServerError;
				result.Message = "An error occurred while updating the audit log.";
			}
			return result;
		}


		public async Task<ResultModel> GetAllAsync()
		{
			var result = new ResultModel();
			try
			{
				var logs = await _auditLogRepo.GetAllAsync(al => al.User);
				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.OK;
				result.Data = logs;
			}
			catch (Exception ex)
			{
				result.IsSuccess = false;
				result.Code = (int)HttpStatusCode.InternalServerError;
				result.Message = ex.Message;
			}
			return result;
		}

		public async Task<ResultModel> GetByIdAsync(Guid logId)
		{
			var result = new ResultModel();
			try
			{
				var auditLog = await _auditLogRepo.GetByIdAsync(logId);
				if (auditLog == null)
				{
					result.IsSuccess = false;
					result.Code = (int)HttpStatusCode.NotFound;
					result.Message = "Audit log not found.";
					return result;
				}
				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.OK;
				result.Data = auditLog;
			}
			catch (Exception ex)
			{
				result.IsSuccess = false;
				result.Code = (int)HttpStatusCode.InternalServerError;
				result.Message = ex.Message;
			}
			return result;
		}
	}
}

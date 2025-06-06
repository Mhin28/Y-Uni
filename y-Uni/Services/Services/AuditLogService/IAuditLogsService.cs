using Repositories.Models;
using Repositories.ViewModels.AuditLogsModel;
using Repositories.ViewModels.ResultModels;
using System;
using System.Threading.Tasks;

namespace Services.Services.AuditLogService
{
	public interface IAuditLogsService
	{
		Task<ResultModel> GetByUserIdAsync(Guid userId);
		Task<ResultModel> AddAuditLogAsync(PostAuditLogModel model);
		Task<ResultModel> DeleteAuditLogAsync(Guid logId);
		Task<ResultModel> UpdateAuditLogAsync(AuditLogModel model);
		Task<ResultModel> GetAllAsync();
		Task<ResultModel> GetByIdAsync(Guid logId);
	}
}

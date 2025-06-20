using Repositories.ViewModels.ReminderModel;
using Repositories.ViewModels.ResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.ReminderService
{
	public interface IReminderService 
	{
		// CRUD operations for reminders
		Task<ResultModel> GetAllAsync();
		Task<ResultModel> GetByIdAsync(Guid id);
		Task<ResultModel> CreateAsync(PostReminderModel model);
		Task<ResultModel> UpdateAsync(ReminderModel model);
		Task<ResultModel> DeleteAsync(Guid id);
		// Additional methods for specific queries
		Task<ResultModel> GetRemindersByUserIdAsync(Guid userId);
		Task<ResultModel> GetRemindersByEventIdAsync(Guid eventId);
		Task<ResultModel> GetRemindersByAssignmentIdAsync(Guid assignmentId);
		Task<ResultModel> GetRemindersByTemplateIdAsync(Guid templateId);
		Task<ResultModel> GetRemindersByStatusAsync(string status);
		Task<ResultModel> GetRemindersByNotificationChannelAsync(string notificationChannel);
	}
}

using Repositories.Base;
using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
	public interface IReminderRepo : IGenericRepository<Reminder>
	{
		Task<IEnumerable<Reminder>> GetRemindersByUserIdAsync(Guid userId);
		Task<IEnumerable<Reminder>> GetRemindersByEventIdAsync(Guid eventId);
		Task<IEnumerable<Reminder>> GetRemindersByAssignmentIdAsync(Guid assignmentId);
		Task<IEnumerable<Reminder>> GetRemindersByTemplateIdAsync(Guid templateId);
		Task<IEnumerable<Reminder>> GetRemindersByStatusAsync(string status);
		Task<IEnumerable<Reminder>> GetRemindersByNotificationChannelAsync(string notificationChannel);
	}
}

using Microsoft.EntityFrameworkCore;
using Repositories.Base;
using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
	public class ReminderRepo : GenericRepository<Reminder>, IReminderRepo
	{
		public ReminderRepo(YUniContext context) : base(context)
		{
		}
		public async Task<IEnumerable<Reminder>> GetRemindersByUserIdAsync(Guid userId)
		{
			return await _context.Reminders.Where(r => r.UserId == userId).ToListAsync();
		}
		public async Task<IEnumerable<Reminder>> GetRemindersByEventIdAsync(Guid eventId)
		{
			return await _context.Reminders.Where(r => r.EventId == eventId).ToListAsync();
		}
		public async Task<IEnumerable<Reminder>> GetRemindersByAssignmentIdAsync(Guid assignmentId)
		{
			return await _context.Reminders.Where(r => r.AssignmentId == assignmentId).ToListAsync();
		}
		public async Task<IEnumerable<Reminder>> GetRemindersByTemplateIdAsync(Guid templateId)
		{
			return await _context.Reminders.Where(r => r.TemplateId == templateId).ToListAsync();
		}
		public async Task<IEnumerable<Reminder>> GetRemindersByStatusAsync(string status)
		{
			return await _context.Reminders.Where(r => r.Status == status).ToListAsync();
		}
		public async Task<IEnumerable<Reminder>> GetRemindersByNotificationChannelAsync(string notificationChannel)
		{
			return await _context.Reminders.Where(r => r.NotificationChannel == notificationChannel).ToListAsync();
		}
	}
}

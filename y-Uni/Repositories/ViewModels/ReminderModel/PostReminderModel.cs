using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.ViewModels.ReminderModel
{
	public class PostReminderModel
	{

		public DateTime ReminderTime { get; set; }

		public string Status { get; set; }

		public string NotificationChannel { get; set; }

		public Guid? EventId { get; set; }

		public Guid? AssignmentId { get; set; }

		public Guid? UserId { get; set; }

		public Guid? TemplateId { get; set; }
	}
}

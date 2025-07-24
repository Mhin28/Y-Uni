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
	public class ReminderTemplateRepo : GenericRepository<ReminderTemplate>, IReminderTemplateRepo
	{
		public ReminderTemplateRepo(YUniContext context) : base(context)
		{
		}

		public async Task<ReminderTemplate> GetDefaultAssignmentTemplateAsync()
		{
			// Look for default assignment template using 'before_start' trigger type
			var template = await _context.ReminderTemplates
				.FirstOrDefaultAsync(t => t.TemplateName == "Default Assignment Reminder" && t.TriggerType == "before_start");
			
			if (template == null)
			{
				// Return a default template with 48 hours (2880 minutes) before due date
				return new ReminderTemplate
				{
					TemplateId = Guid.NewGuid(),
					TemplateName = "Default Assignment Reminder",
					TriggerType = "before_start", // Use correct constraint value
					TriggerValue = 2880 // 48 hours = 2880 minutes
				};
			}
			
			return template;
		}

		public async Task<ReminderTemplate> GetDefaultEventTemplateAsync()
		{
			// Look for default event template using 'before_start' trigger type
			var template = await _context.ReminderTemplates
				.FirstOrDefaultAsync(t => t.TemplateName == "Default Event Reminder" && t.TriggerType == "before_start");
			
			if (template == null)
			{
				// Return a default template with 30 minutes before start time
				return new ReminderTemplate
				{
					TemplateId = Guid.NewGuid(),
					TemplateName = "Default Event Reminder",
					TriggerType = "before_start", // Use correct constraint value
					TriggerValue = 30 // 30 minutes before
				};
			}
			
			return template;
		}
	}
}

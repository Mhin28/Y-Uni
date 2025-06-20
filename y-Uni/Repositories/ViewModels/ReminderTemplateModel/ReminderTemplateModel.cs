using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.ViewModels.ReminderTemplateModel
{
	public class ReminderTemplateModel
	{
		public Guid TemplateId { get; set; }

		public string TemplateName { get; set; }

		public string TriggerType { get; set; }

		public int? TriggerValue { get; set; }
	}
}

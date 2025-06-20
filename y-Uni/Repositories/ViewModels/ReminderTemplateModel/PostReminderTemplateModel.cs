using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.ViewModels.ReminderTemplateModel
{
	public class PostReminderTemplateModel
	{

		public string TemplateName { get; set; }

		public string TriggerType { get; set; }

		public int? TriggerValue { get; set; }
	}
}

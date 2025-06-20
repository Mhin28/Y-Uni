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
		// Additional methods specific to ReminderTemplate can be added here
	}
}

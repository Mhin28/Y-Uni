using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.ViewModels.AuditLogsModel
{
	public class PostAuditLogModel
	{
		public string ActionType { get; set; }

		public string TableName { get; set; }

		public Guid RecordId { get; set; }

		public Guid? UserId { get; set; }

		public DateTime? ActionTimestamp { get; set; }

		public string OldValues { get; set; }

		public string NewValues { get; set; }
	}
}

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
	public class AuditLogsRepo : GenericRepository<AuditLog>, IAuditLogsRepo
	{

		public AuditLogsRepo(YUniContext context) : base(context)
		{ }

		public async Task<List<AuditLog>> GetByUserIdAsync(Guid userId)
		{
			var lists = await _context.AuditLogs.Where(al => al.UserId == userId)
				.OrderByDescending(al => al.ActionTimestamp)
				.ToListAsync();
			return lists;
		}
	}
}

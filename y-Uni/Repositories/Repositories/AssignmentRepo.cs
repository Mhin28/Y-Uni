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
    public class AssignmentRepo : GenericRepository<Assignment>, IAssignmentRepo
    {
        public AssignmentRepo(YUniContext context): base(context) { }

        public async Task<List<Assignment>> GetAssignmentsByUserIdAsync(Guid userId)
        {
            return await _context.Assignments
                .Where(a => a.UserId == userId)
                .Include(a => a.Subject)
                .Include(a => a.Priority)
                .OrderBy(a => a.DueDate)
                .ToListAsync();
        }

        public async Task<List<Assignment>> GetAssignmentsBySubjectAsync(Guid subjectId)
        {
            return await _context.Assignments
                .Where(a => a.SubjectId == subjectId)
                .Include(a => a.Priority)
                .OrderBy(a => a.DueDate)
                .ToListAsync();
        }

        public async Task<List<Assignment>> GetUpcomingAssignmentsByUserIdAsync(Guid userId, DateTime dueDate)
        {
            return await _context.Assignments
                .Where(a => a.UserId == userId && 
                          a.DueDate <= dueDate && 
                          (a.Status == "not_started" || a.Status == "in_progress"))
                .Include(a => a.Subject)
                .Include(a => a.Priority)
                .OrderBy(a => a.DueDate)
                .ToListAsync();
        }

        public async Task<List<Assignment>> GetAssignmentsByStatusAsync(Guid userId, string status)
        {
            return await _context.Assignments
                .Where(a => a.UserId == userId && a.Status == status)
                .Include(a => a.Subject)
                .Include(a => a.Priority)
                .OrderBy(a => a.DueDate)
                .ToListAsync();
        }
    }
}

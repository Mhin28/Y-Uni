using Microsoft.EntityFrameworkCore;
using Repositories.Base;
using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public class SubjectRepo : GenericRepository<Subject>, ISubjectRepo
    {
        public SubjectRepo(YuniBuddyContext context) : base(context) { }

        public async Task<Subject> GetByIdAsync(Guid subjectId)
        {
            return await _context.Subjects
                .FirstOrDefaultAsync(s => s.SubjectId == subjectId);
        }

        public async Task<List<Subject>> GetAllOrderedByNameAsync()
        {
            return await _context.Subjects
                .OrderBy(s => s.SubjectName)
                .ToListAsync();
        }

        public async Task<bool> IsSubjectInUseAsync(Guid subjectId)
        {
            return await _context.Assignments
                .AnyAsync(a => a.SubjectId == subjectId);
        }

        public async Task<List<Subject>> GetSubjectsByUserIdAsync(Guid userId)
        {
            return await _context.Subjects
                .Where(s => s.UserId == userId)
                .OrderBy(s => s.SubjectName)
                .ToListAsync();
        }

        public async Task<bool> SubjectNameExistsForUserAsync(string subjectName, Guid userId)
        {
            return await _context.Subjects
                .AnyAsync(s => s.SubjectName.ToLower() == subjectName.ToLower() && s.UserId == userId);
        }
    }
}
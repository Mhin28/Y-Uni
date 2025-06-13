using Microsoft.EntityFrameworkCore;
using Repositories.Base;
using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public class PriorityLevelRepo : GenericRepository<PriorityLevel>, IPriorityLevelRepo
    {
        public PriorityLevelRepo(YUniContext context) : base(context) { }

        public async Task<PriorityLevel> GetByPriorityIdAsync(byte priorityId)
        {
            return await _context.PriorityLevels
                .FirstOrDefaultAsync(p => p.PriorityId == priorityId);
        }

        public async Task<List<PriorityLevel>> GetAllOrderedByPriorityAsync()
        {
            return await _context.PriorityLevels
                .OrderBy(p => p.PriorityId)
                .ToListAsync();
        }
    }
} 
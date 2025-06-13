using Repositories.Base;
using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public interface IPriorityLevelRepo : IGenericRepository<PriorityLevel>
    {
        Task<PriorityLevel> GetByPriorityIdAsync(byte priorityId);
        Task<List<PriorityLevel>> GetAllOrderedByPriorityAsync();
    }
} 
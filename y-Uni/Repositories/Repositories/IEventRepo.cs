using Repositories.Base;
using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public interface IEventRepo : IGenericRepository<Event>
    {
        Task<List<Event>> GetEventsByUserIdAsync(Guid userId);
        Task<List<Event>> GetUpcomingEventsByUserIdAsync(Guid userId, DateTime startDate, DateTime endDate);
        Task<List<Event>> GetEventsByCategoryAsync(Guid categoryId);
    }
} 
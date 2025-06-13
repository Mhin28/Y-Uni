using Repositories.Base;
using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public interface IEventCategoryRepo : IGenericRepository<EventCategory>
    {
        Task<EventCategory> GetByIdAsync(Guid categoryId);
        Task<List<EventCategory>> GetAllOrderedByNameAsync();
        Task<bool> IsCategoryInUseAsync(Guid categoryId);
    }
} 
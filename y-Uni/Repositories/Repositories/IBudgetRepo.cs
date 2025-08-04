using Repositories.Base;
using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public interface IBudgetRepo : IGenericRepository<Budget>
    {
        Task<List<Budget>> GetUserBudgetsAsync(Guid userId);
        Task<List<Budget>> GetByUserIdAsync(Guid userId);
        Task<List<Budget>> GetActiveBudgetsForUserAsync(Guid userId, DateTime date);
        Task<List<Budget>> GetUserBudgetsForMonthAsync(Guid userId, int year, int month);
        Task<Budget?> GetBudgetByCategoryAsync(Guid userId, Guid categoryId);
    }
} 
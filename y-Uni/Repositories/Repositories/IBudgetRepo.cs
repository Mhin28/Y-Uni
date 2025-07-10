using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public interface IBudgetRepo
    {
        Task<List<Budget>> GetBudgetsByUserIdAsync(Guid userId, Guid? categoryId = null, DateOnly? from = null, DateOnly? to = null);
        Task<Budget> GetBudgetByIdAsync(Guid budgetId);
        Task<Budget> AddBudgetAsync(Budget budget);
        Task<bool> UpdateBudgetAsync(Budget budget);
        Task<bool> DeleteBudgetAsync(Budget budget);
    }
} 
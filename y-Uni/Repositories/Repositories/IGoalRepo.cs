using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public interface IGoalRepo
    {
        Task<List<Goal>> GetGoalsByUserIdAsync(Guid userId, string status = null, DateOnly? from = null, DateOnly? to = null);
        Task<Goal> GetGoalByIdAsync(Guid goalId);
        Task<bool> UpdateGoalAsync(Goal goal);
        Task<bool> DeleteGoalAsync(Goal goal);
        Task<Goal> AddGoalAsync(Goal goal);
    }
} 
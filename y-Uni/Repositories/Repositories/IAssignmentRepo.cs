using Repositories.Base;
using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public interface IAssignmentRepo : IGenericRepository<Assignment>
    {
        Task<List<Assignment>> GetAssignmentsByUserIdAsync(Guid userId);
        Task<List<Assignment>> GetAssignmentsBySubjectAsync(Guid subjectId);
        Task<List<Assignment>> GetUpcomingAssignmentsByUserIdAsync(Guid userId, DateTime dueDate);
        Task<List<Assignment>> GetAssignmentsByStatusAsync(Guid userId, string status);
    }
}

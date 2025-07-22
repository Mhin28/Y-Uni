using Repositories.Base;
using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public interface ISubjectRepo : IGenericRepository<Subject>
    {
        Task<Subject> GetByIdAsync(Guid subjectId);
        Task<List<Subject>> GetAllOrderedByNameAsync();
        Task<List<Subject>> GetSubjectsByUserIdAsync(Guid userId);
        Task<bool> IsSubjectInUseAsync(Guid subjectId);
        Task<bool> SubjectNameExistsForUserAsync(string subjectName, Guid userId);
    }
} 
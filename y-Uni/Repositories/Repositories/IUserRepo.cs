using Microsoft.Extensions.Options;
using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public interface IUserRepo
    {
        public Task<User?> GetByUsernameAsync(string username);
        public Task<List<User>> GetAllUser();
        Task<User> GetByEmailAsync(string email);
        Task<User> AddAsync(User user);
        Task<User> UpdateAsync(User user);
        Task<User?> GetByIdAsync(Guid userId);
        Task<bool> CheckRoleExists(Guid roleId);
        Task<User> UpdateVerifyAsync(User model);
    }
}

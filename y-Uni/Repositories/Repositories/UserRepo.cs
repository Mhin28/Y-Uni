using Microsoft.EntityFrameworkCore;
using Repositories.Base;
using Repositories.Models;
using Repositories.ViewModels.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public class UserRepo : GenericRepository<User>, IUserRepo
    {
        private readonly YUniContext _context;
        public UserRepo(YUniContext context) : base(context)
        {
            _context = context;
        }
        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserName == username);
        }
        public async Task<List<User>> GetAllUser()
        {
            return await _context.Users.ToListAsync();
        }
        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
        
        public async Task<User> UpdateAsync(User user)
        {
            var existingUser = await _context.Users.FindAsync(user.UserId);
            if (existingUser == null)
                return null;
                
            existingUser.FullName = user.FullName;
            existingUser.Email = user.Email;
            existingUser.DoB = user.DoB;
            existingUser.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return existingUser;
        }
        
        public async Task<User?> GetByIdAsync(Guid userId)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }
        public async Task<bool> CheckRoleExists(int roleId)
        {
            return await _context.Roles.AnyAsync(r => r.RoleId == roleId);
        }
        public async Task<User> UpdateVerifyAsync(User model)
        {
            var existingUser = await _context.Users.FindAsync(model.UserId);
            if (existingUser == null)
                return null;

            existingUser.VerificationCode = model.VerificationCode;
            existingUser.VerificationCodeExpiry = model.VerificationCodeExpiry;
            existingUser.IsVerified = model.IsVerified;
            _context.Entry(existingUser).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return existingUser;
        }
    }
}

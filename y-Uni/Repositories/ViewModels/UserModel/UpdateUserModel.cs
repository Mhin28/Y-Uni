using System;
using Microsoft.EntityFrameworkCore;

namespace Repositories.ViewModels.UserModel
{
    public class UpdateUserModel
    {
        public Guid UserId { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public DateOnly? DoB { get; set; }

        // Password is handled separately for security reasons

    }
} 
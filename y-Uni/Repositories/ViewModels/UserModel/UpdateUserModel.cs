using System;
using Microsoft.EntityFrameworkCore;

namespace Repositories.ViewModels.UserModel
{
    public class UpdateUserModel
    {
        public string? Img { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public DateTime? DoB { get; set; }


    }
} 
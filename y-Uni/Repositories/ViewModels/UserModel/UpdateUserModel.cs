using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace Repositories.ViewModels.UserModel
{
    public class UpdateUserModel
    {
        public IFormFile? Img { get; set; }

        public string? FullName { get; set; }

        public string? Email { get; set; }

        public DateTime? DoB { get; set; }


    }
} 
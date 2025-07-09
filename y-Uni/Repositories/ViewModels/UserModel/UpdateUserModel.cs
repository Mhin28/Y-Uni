using System;
using Microsoft.EntityFrameworkCore;

namespace Repositories.ViewModels.UserModel
{
    public class UpdateUserModel
    {

        public string FullName { get; set; }

        public string Email { get; set; }

        public DateOnly? DoB { get; set; }


    }
} 
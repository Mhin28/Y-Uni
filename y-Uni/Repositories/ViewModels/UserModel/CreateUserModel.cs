﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.ViewModels.UserModel
{
    public class CreateUserModel
    {
        public Guid UserId { get; set; }

        public string FullName { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public DateOnly? DoB { get; set; }

        public string PasswordHash { get; set; }

        public DateTime? LastLogin { get; set; }

        public bool? IsVerified { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}

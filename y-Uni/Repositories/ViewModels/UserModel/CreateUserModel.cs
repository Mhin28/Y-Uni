using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.ViewModels.UserModel
{
    public class CreateUserModel
    {

        public string FullName { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public DateOnly? DoB { get; set; }

        public string PasswordHash { get; set; }

        public int? RoleId { get; set; }
    }
}

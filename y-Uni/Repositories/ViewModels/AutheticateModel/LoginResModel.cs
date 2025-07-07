using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.ViewModels.AutheticateModel
{
    public class LoginResModel
    {
        public Guid UserId { get; set; }

        public string FullName { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public DateOnly? DoB { get; set; }

        public string PasswordHash { get; set; }

        public DateTime? LastLogin { get; set; }
        public string Img { get; set; }

        public Guid? RoleId { get; set; }

        public bool? IsVerified { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}

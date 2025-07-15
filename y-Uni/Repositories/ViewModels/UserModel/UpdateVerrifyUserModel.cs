using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.ViewModels.UserModel
{
    public class UpdateVerrifyUserModel
    {
        public Guid UserId { get; set; }
        public string VerificationCode { get; set; }

        public DateTime? VerificationCodeExpiry { get; set; }
        public bool? IsVerified { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.ViewModels.TokenModel
{
    public class TokenModel
    {
        public string userid { get; set; }
        public string role { get; set; }
        public TokenModel(string userid, string role)
        {
            this.userid = userid;
            this.role = role;
        }
    }
}

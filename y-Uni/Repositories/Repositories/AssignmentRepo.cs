using Microsoft.EntityFrameworkCore;
using Repositories.Base;
using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public class AssignmentRepo : GenericRepository<Assignment>, IAssignmentRepo
    {
        public AssignmentRepo(YUniContext context): base(context) { }
    }
}

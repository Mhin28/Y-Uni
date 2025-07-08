using Repositories.Base;
using Repositories.Models;

namespace Repositories.Repositories
{
	public class BudgetRepo : GenericRepository<Budget>, IBudgetRepo
	{
		public BudgetRepo(YUniContext context) : base(context) { }
	}
}
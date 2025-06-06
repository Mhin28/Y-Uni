using Repositories.Base;
using Repositories.Models;

namespace Repositories.Repositories
{
	public class ExpenseRepo : GenericRepository<Expense>, IExpenseRepo
	{
		public ExpenseRepo(YUniContext context) : base(context) { }
	}
}

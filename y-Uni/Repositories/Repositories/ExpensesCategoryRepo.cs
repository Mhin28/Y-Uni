using Repositories.Base;
using Repositories.Models;

namespace Repositories.Repositories
{
	public class ExpensesCategoryRepo : GenericRepository<ExpensesCategory>, IExpensesCategoryRepo
	{
		public ExpensesCategoryRepo(YuniBuddyContext context) : base(context) { }
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.ViewModels.BudgetModel
{
    public class PostBudgetModel
    {
		public Guid? CategoryId { get; set; }

		public Guid? AccountId { get; set; }

        public decimal BudgetAmount { get; set; }

		public DateOnly? StartDate { get; set; }

		public DateOnly? EndDate { get; set; }
    }
} 
using System;

namespace Repositories.ViewModels.BudgetModel
{
    public class PostBudgetModel
    {
        public Guid CategoryId { get; set; }
        public Guid AccountId { get; set; }
        public decimal BudgetAmount { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }
} 
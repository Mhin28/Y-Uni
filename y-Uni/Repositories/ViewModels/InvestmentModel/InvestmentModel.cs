using System;

namespace Repositories.ViewModels.InvestmentModel
{
    public class InvestmentModel
    {
        public Guid InvestmentId { get; set; }
        public string InvestmentName { get; set; }
        public decimal Amount { get; set; }
        public DateOnly InvestmentDate { get; set; }
        public DateOnly MaturityDate { get; set; }
        public double InterestRate { get; set; }
        public Guid UserId { get; set; }
    }
} 
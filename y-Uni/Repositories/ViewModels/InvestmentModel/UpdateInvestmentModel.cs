using System;

namespace Repositories.ViewModels.InvestmentModel
{
    public class UpdateInvestmentModel
    {
        public string InvestmentName { get; set; }
        public decimal Amount { get; set; }
        public DateOnly InvestmentDate { get; set; }
        public DateOnly? MaturityDate { get; set; }
        public decimal? InterestRate { get; set; }
    }
} 
using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public interface IInvestmentRepo
    {
        Task<List<Investment>> GetInvestmentsByUserIdAsync(
            Guid userId,
            string investmentName = null,
            DateOnly? from = null,
            DateOnly? to = null,
            decimal? minAmount = null,
            decimal? maxAmount = null,
            DateOnly? maturityFrom = null,
            DateOnly? maturityTo = null,
            decimal? interestRate = null);
        Task<Investment> GetInvestmentByIdAsync(Guid investmentId);
        Task<Investment> AddInvestmentAsync(Investment investment);
        Task<bool> UpdateInvestmentAsync(Investment investment);
        Task<bool> DeleteInvestmentAsync(Investment investment);
    }
} 
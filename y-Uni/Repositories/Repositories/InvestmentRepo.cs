using Microsoft.EntityFrameworkCore;
using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public class InvestmentRepo : IInvestmentRepo
    {
        private readonly YUniContext _context;
        public InvestmentRepo(YUniContext context)
        {
            _context = context;
        }
        public async Task<List<Investment>> GetInvestmentsByUserIdAsync(
            Guid userId,
            string investmentName = null,
            DateOnly? from = null,
            DateOnly? to = null,
            decimal? minAmount = null,
            decimal? maxAmount = null,
            DateOnly? maturityFrom = null,
            DateOnly? maturityTo = null,
            decimal? interestRate = null)
        {
            var query = _context.Investments.Where(i => i.UserId == userId);
            if (!string.IsNullOrEmpty(investmentName))
                query = query.Where(i => i.InvestmentName.Contains(investmentName));
            if (from.HasValue)
                query = query.Where(i => i.InvestmentDate >= from.Value);
            if (to.HasValue)
                query = query.Where(i => i.InvestmentDate <= to.Value);
            if (minAmount.HasValue)
                query = query.Where(i => i.Amount >= minAmount.Value);
            if (maxAmount.HasValue)
                query = query.Where(i => i.Amount <= maxAmount.Value);
            if (maturityFrom.HasValue)
                query = query.Where(i => i.MaturityDate >= maturityFrom.Value);
            if (maturityTo.HasValue)
                query = query.Where(i => i.MaturityDate <= maturityTo.Value);
            if (interestRate.HasValue)
                query = query.Where(i => i.InterestRate == interestRate.Value);
            return await query.ToListAsync();
        }
        public async Task<Investment> GetInvestmentByIdAsync(Guid investmentId)
        {
            return await _context.Investments.FirstOrDefaultAsync(i => i.InvestmentId == investmentId);
        }
        public async Task<Investment> AddInvestmentAsync(Investment investment)
        {
            _context.Investments.Add(investment);
            await _context.SaveChangesAsync();
            return investment;
        }
        public async Task<bool> UpdateInvestmentAsync(Investment investment)
        {
            _context.Investments.Update(investment);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> DeleteInvestmentAsync(Investment investment)
        {
            _context.Investments.Remove(investment);
            return await _context.SaveChangesAsync() > 0;
        }
    }
} 
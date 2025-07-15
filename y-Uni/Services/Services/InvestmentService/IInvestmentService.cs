using Repositories.ViewModels.InvestmentModel;
using Repositories.ViewModels.ResultModels;
using System;
using System.Threading.Tasks;

namespace Services.Services.InvestmentService
{
    public interface IInvestmentService
    {
        Task<ResultModel> GetInvestmentsByUserAsync(
            string token,
            string investmentName = null,
            DateOnly? from = null,
            DateOnly? to = null,
            decimal? minAmount = null,
            decimal? maxAmount = null,
            DateOnly? maturityFrom = null,
            DateOnly? maturityTo = null,
            decimal? interestRate = null);
        Task<ResultModel> CreateInvestmentAsync(string token, PostInvestmentModel model);
        Task<ResultModel> UpdateInvestmentAsync(string token, Guid investmentId, UpdateInvestmentModel model);
        Task<ResultModel> DeleteInvestmentAsync(string token, Guid investmentId);
    }
} 
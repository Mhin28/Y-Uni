using Repositories.Models;
using Repositories.Repositories;
using Repositories.ViewModels.InvestmentModel;
using Repositories.ViewModels.ResultModels;
using Services.Services.TokenService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Services.Services.InvestmentService
{
    public class InvestmentService : IInvestmentService
    {
        private readonly IInvestmentRepo _investmentRepo;
        private readonly ITokenService _tokenService;
        public InvestmentService(IInvestmentRepo investmentRepo, ITokenService tokenService)
        {
            _investmentRepo = investmentRepo;
            _tokenService = tokenService;
        }
        public async Task<ResultModel> GetInvestmentsByUserAsync(
            string token,
            string investmentName = null,
            DateOnly? from = null,
            DateOnly? to = null,
            decimal? minAmount = null,
            decimal? maxAmount = null,
            DateOnly? maturityFrom = null,
            DateOnly? maturityTo = null,
            decimal? interestRate = null)
        {
            var decoded = _tokenService.decode(token);
            if (decoded == null || string.IsNullOrEmpty(decoded.userid) || decoded.role != "2")
                return new ResultModel { IsSuccess = false, Code = (int)HttpStatusCode.Forbidden, Message = "Không có quyền truy cập" };
            Guid userId = Guid.Parse(decoded.userid);
            var investments = await _investmentRepo.GetInvestmentsByUserIdAsync(userId, investmentName, from, to, minAmount, maxAmount, maturityFrom, maturityTo, interestRate);
            var data = investments.Select(i => new InvestmentModel
            {
                InvestmentId = i.InvestmentId,
                InvestmentName = i.InvestmentName,
                Amount = i.Amount,
                InvestmentDate = i.InvestmentDate,
                MaturityDate = i.MaturityDate,
                InterestRate = i.InterestRate,
                UserId = i.UserId
            }).ToList();
            return new ResultModel { IsSuccess = true, Code = (int)HttpStatusCode.OK, Data = data, Message = "Lấy danh sách đầu tư thành công" };
        }
        public async Task<ResultModel> CreateInvestmentAsync(string token, PostInvestmentModel model)
        {
            var decoded = _tokenService.decode(token);
            if (decoded == null || string.IsNullOrEmpty(decoded.userid) || decoded.role != "2")
                return new ResultModel { IsSuccess = false, Code = (int)HttpStatusCode.Forbidden, Message = "Không có quyền truy cập" };
            Guid userId = Guid.Parse(decoded.userid);
            var investment = new Investment
            {
                InvestmentId = Guid.NewGuid(),
                InvestmentName = model.InvestmentName,
                Amount = model.Amount,
                InvestmentDate = model.InvestmentDate,
                MaturityDate = model.MaturityDate,
                InterestRate = model.InterestRate,
                UserId = userId
            };
            var created = await _investmentRepo.AddInvestmentAsync(investment);
            if (created == null)
                return new ResultModel { IsSuccess = false, Code = (int)HttpStatusCode.InternalServerError, Message = "Tạo đầu tư thất bại" };
            return new ResultModel { IsSuccess = true, Code = (int)HttpStatusCode.Created, Message = "Tạo đầu tư thành công", Data = new InvestmentModel {
                InvestmentId = created.InvestmentId,
                InvestmentName = created.InvestmentName,
                Amount = created.Amount,
                InvestmentDate = created.InvestmentDate,
                MaturityDate = created.MaturityDate,
                InterestRate = created.InterestRate,
                UserId = created.UserId
            }};
        }
        public async Task<ResultModel> UpdateInvestmentAsync(string token, Guid investmentId, UpdateInvestmentModel model)
        {
            var decoded = _tokenService.decode(token);
            if (decoded == null || string.IsNullOrEmpty(decoded.userid) || decoded.role != "2")
                return new ResultModel { IsSuccess = false, Code = (int)HttpStatusCode.Forbidden, Message = "Không có quyền truy cập" };
            Guid userId = Guid.Parse(decoded.userid);
            var investment = await _investmentRepo.GetInvestmentByIdAsync(investmentId);
            if (investment == null || investment.UserId != userId)
                return new ResultModel { IsSuccess = false, Code = (int)HttpStatusCode.NotFound, Message = "Đầu tư không tồn tại hoặc không thuộc quyền sở hữu" };
            investment.InvestmentName = model.InvestmentName;
            investment.Amount = model.Amount;
            investment.InvestmentDate = model.InvestmentDate;
            investment.MaturityDate = model.MaturityDate;
            investment.InterestRate = model.InterestRate;
            var success = await _investmentRepo.UpdateInvestmentAsync(investment);
            if (!success)
                return new ResultModel { IsSuccess = false, Code = (int)HttpStatusCode.InternalServerError, Message = "Cập nhật đầu tư thất bại" };
            return new ResultModel { IsSuccess = true, Code = (int)HttpStatusCode.OK, Message = "Cập nhật đầu tư thành công" };
        }
        public async Task<ResultModel> DeleteInvestmentAsync(string token, Guid investmentId)
        {
            var decoded = _tokenService.decode(token);
            if (decoded == null || string.IsNullOrEmpty(decoded.userid) || decoded.role != "2")
                return new ResultModel { IsSuccess = false, Code = (int)HttpStatusCode.Forbidden, Message = "Không có quyền truy cập" };
            Guid userId = Guid.Parse(decoded.userid);
            var investment = await _investmentRepo.GetInvestmentByIdAsync(investmentId);
            if (investment == null || investment.UserId != userId)
                return new ResultModel { IsSuccess = false, Code = (int)HttpStatusCode.NotFound, Message = "Đầu tư không tồn tại hoặc không thuộc quyền sở hữu" };
            var success = await _investmentRepo.DeleteInvestmentAsync(investment);
            if (!success)
                return new ResultModel { IsSuccess = false, Code = (int)HttpStatusCode.InternalServerError, Message = "Xóa đầu tư thất bại" };
            return new ResultModel { IsSuccess = true, Code = (int)HttpStatusCode.OK, Message = "Xóa đầu tư thành công" };
        }
    }
} 
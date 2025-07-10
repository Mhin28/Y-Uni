using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Services.InvestmentService;
using Repositories.ViewModels.InvestmentModel;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InvestmentController : ControllerBase
    {
        private readonly IInvestmentService _investmentService;
        public InvestmentController(IInvestmentService investmentService)
        {
            _investmentService = investmentService;
        }
        [HttpGet("my-investments")]
        public async Task<IActionResult> GetMyInvestments(
            [FromQuery] string? investmentName,
            [FromQuery] DateOnly? from,
            [FromQuery] DateOnly? to,
            [FromQuery] decimal? minAmount,
            [FromQuery] decimal? maxAmount,
            [FromQuery] DateOnly? maturityFrom,
            [FromQuery] DateOnly? maturityTo,
            [FromQuery] double? interestRate)
        {
            string token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var result = await _investmentService.GetInvestmentsByUserAsync(token, investmentName, from, to, minAmount, maxAmount, maturityFrom, maturityTo, interestRate);
            return StatusCode(result.Code, result);
        }
        [HttpPost]
        public async Task<IActionResult> CreateInvestment([FromBody] PostInvestmentModel model)
        {
            string token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var result = await _investmentService.CreateInvestmentAsync(token, model);
            return StatusCode(result.Code, result);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInvestment(Guid id, [FromBody] UpdateInvestmentModel model)
        {
            string token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var result = await _investmentService.UpdateInvestmentAsync(token, id, model);
            return StatusCode(result.Code, result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvestment(Guid id)
        {
            string token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var result = await _investmentService.DeleteInvestmentAsync(token, id);
            return StatusCode(result.Code, result);
        }
    }
} 
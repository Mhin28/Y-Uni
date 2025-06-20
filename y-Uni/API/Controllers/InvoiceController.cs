using Microsoft.AspNetCore.Mvc;
using Repositories.ViewModels.InvoiceModel;
using Services.Services.InvoiceService;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class InvoiceController : ControllerBase
	{
		private readonly IInvoiceService _invoiceService;

		public InvoiceController(IInvoiceService invoiceService)
		{
			_invoiceService = invoiceService;
		}

		#region CRUD

		// GET: api/Invoice
		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var result = await _invoiceService.GetAllAsync();
			return StatusCode(result.Code, result);
		}

		// GET: api/Invoice/{id}
		[HttpGet("{id:guid}")]
		public async Task<IActionResult> GetById(Guid id)
		{
			var result = await _invoiceService.GetByIdAsync(id);
			return StatusCode(result.Code, result);
		}

		// POST: api/Invoice
		[HttpPost]
		public async Task<IActionResult> Create([FromBody] PostInvoiceModel model)
		{
			var result = await _invoiceService.AddAsync(model);
			return StatusCode(result.Code, result);
		}

		// PUT: api/Invoice
		[HttpPut]
		public async Task<IActionResult> Update([FromBody] InvoiceModel model)
		{
			var result = await _invoiceService.UpdateAsync(model);
			return StatusCode(result.Code, result);
		}

		// DELETE: api/Invoice/{id}
		[HttpDelete("{id:guid}")]
		public async Task<IActionResult> Delete(Guid id)
		{
			var result = await _invoiceService.DeleteAsync(id);
			return StatusCode(result.Code, result);
		}

		#endregion

		#region Filtering

		// GET: api/Invoice/user/{userId}
		[HttpGet("user/{userId:guid}")]
		public async Task<IActionResult> GetByUserId(Guid userId)
		{
			var result = await _invoiceService.GetInvoicesByUserIdAsync(userId);
			return StatusCode(result.Code, result);
		}

		// GET: api/Invoice/payment-method/{paymentMethodId}
		[HttpGet("payment-method/{paymentMethodId:guid}")]
		public async Task<IActionResult> GetByPaymentMethodId(Guid paymentMethodId)
		{
			var result = await _invoiceService.GetInvoicesByPaymentMethodAsync(paymentMethodId);
			return StatusCode(result.Code, result);
		}

		// GET: api/Invoice/membership-plan/{membershipPlanId}
		[HttpGet("membership-plan/{membershipPlanId:guid}")]
		public async Task<IActionResult> GetByMembershipPlanId(Guid membershipPlanId)
		{
			var result = await _invoiceService.GetInvoicesByMembershipPlanAsync(membershipPlanId);
			return StatusCode(result.Code, result);
		}

		// GET: api/Invoice/discount/{discountId}
		[HttpGet("discount/{discountId:guid}")]
		public async Task<IActionResult> GetByDiscountId(Guid discountId)
		{
			var result = await _invoiceService.GetInvoicesByDiscountAsync(discountId);
			return StatusCode(result.Code, result);
		}

		#endregion
	}
}

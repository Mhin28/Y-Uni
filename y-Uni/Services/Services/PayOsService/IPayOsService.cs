using Net.payOS.Types;
using Repositories.ViewModels.PayOsModel;
using Repositories.ViewModels.ResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.PayOsService
{
	public interface IPayOsService
	{
		Task<ResultModel> CreatePaymentLinkAsync(CreatePaymentLinkRequestModel model);
		Task<ResultModel> GetPaymentInfoAsync(Guid invoiceId);
		Task<ResultModel> CancelPaymentAsync(Guid invoiceId);
		Task<ResultModel> HandleWebhookAsync(WebhookType body);
	}
}

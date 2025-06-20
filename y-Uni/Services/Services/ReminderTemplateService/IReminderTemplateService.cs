using Repositories.ViewModels.ReminderTemplateModel;
using Repositories.ViewModels.ResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.ReminderTemplateService
{
	public interface IReminderTemplateService
	{
		//use ResultModel for all methods to handle success and error responses
		Task<ResultModel> GetAllAsync();
		Task<ResultModel> GetByIdAsync(Guid id);
		Task<ResultModel> CreateAsync(PostReminderTemplateModel model);
		Task<ResultModel> UpdateAsync(ReminderTemplateModel model);
		Task<ResultModel> DeleteAsync(Guid id);
	}
}

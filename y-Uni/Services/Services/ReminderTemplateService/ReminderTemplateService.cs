using Repositories.Models;
using Repositories.Repositories;
using Repositories.ViewModels.ReminderTemplateModel;
using Repositories.ViewModels.ResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.ReminderTemplateService
{
	public class ReminderTemplateService : IReminderTemplateService
	{
		private readonly IReminderTemplateRepo _repo;

		public ReminderTemplateService(IReminderTemplateRepo repo)
		{
			_repo = repo;
		}

		public async Task<ResultModel> GetAllAsync()
		{
			var result = new ResultModel();
			try
			{
				var templates = await _repo.GetAllAsync();
				result.IsSuccess = true;
				result.Code = (int)System.Net.HttpStatusCode.OK;
				result.Data = templates;
			}
			catch (Exception ex)
			{
				result.IsSuccess = false;
				result.Code = (int)System.Net.HttpStatusCode.InternalServerError;
				result.Message = ex.Message;
			}
			return result;
		}
		public async Task<ResultModel> GetByIdAsync(Guid id)
		{
			var result = new ResultModel();
			try
			{
				var template = await _repo.GetByIdAsync(id);
				if (template == null)
				{
					result.IsSuccess = false;
					result.Code = (int)System.Net.HttpStatusCode.NotFound;
					result.Message = "Reminder template not found.";
					return result;
				}
				result.IsSuccess = true;
				result.Code = (int)System.Net.HttpStatusCode.OK;
				result.Data = template;
			}
			catch (Exception ex)
			{
				result.IsSuccess = false;
				result.Code = (int)System.Net.HttpStatusCode.InternalServerError;
				result.Message = ex.Message;
			}
			return result;
		}

		public async Task<ResultModel> CreateAsync(PostReminderTemplateModel model)
		{
			var result = new ResultModel()
			{
				IsSuccess = false,
				Code = (int)System.Net.HttpStatusCode.BadRequest,
				Message = "Invalid reminder template data."
			};
			try
			{
				var template = new ReminderTemplate
				{
					TemplateId = Guid.NewGuid(),
					TemplateName = model.TemplateName,
					TriggerType = model.TriggerType,
					TriggerValue = model.TriggerValue
				};
				await _repo.CreateAsync(template);

				result.IsSuccess = true;
				result.Code = (int)System.Net.HttpStatusCode.Created;
				result.Data = template;
			}
			catch (Exception ex)
			{
				result.IsSuccess = false;
				result.Code = (int)System.Net.HttpStatusCode.InternalServerError;
				result.Message = ex.Message;
			}
			return result;
		}

		public async Task<ResultModel> UpdateAsync(ReminderTemplateModel model)
		{
			var result = new ResultModel()
			{
				IsSuccess = false,
				Code = (int)System.Net.HttpStatusCode.BadRequest,
				Message = "Invalid reminder template data."
			};
			try
			{
				var existingTemplate = await _repo.GetByIdAsync(model.TemplateId);
				if (existingTemplate == null)
				{
					result.IsSuccess = false;
					result.Code = (int)System.Net.HttpStatusCode.NotFound;
					result.Message = "Reminder template not found.";
					return result;
				}

				if (!string.IsNullOrEmpty(model.TemplateName))
					existingTemplate.TemplateName = model.TemplateName;

				if (!string.IsNullOrEmpty(model.TriggerType))
					existingTemplate.TriggerType = model.TriggerType;

				if (model.TriggerValue.HasValue)
					existingTemplate.TriggerValue = model.TriggerValue.Value;

				await _repo.UpdateAsync(existingTemplate);
				result.IsSuccess = true;
				result.Code = (int)System.Net.HttpStatusCode.OK;
				result.Data = existingTemplate;
				result.Message = "Reminder template updated successfully.";
			}
			catch (Exception ex)
			{
				result.IsSuccess = false;
				result.Code = (int)System.Net.HttpStatusCode.InternalServerError;
				result.Message = ex.Message;
			}
			return result;
		}

		public async Task<ResultModel> DeleteAsync(Guid id)
		{
			var result = new ResultModel()
			{
				IsSuccess = false,
				Code = (int)System.Net.HttpStatusCode.BadRequest,
				Message = "Invalid reminder template ID."
			};
			try
			{
				var template = await _repo.GetByIdAsync(id);
				if (template == null)
				{
					result.IsSuccess = false;
					result.Code = (int)System.Net.HttpStatusCode.NotFound;
					result.Message = "Reminder template not found.";
					return result;
				}
				await _repo.RemoveAsync(template);
				result.IsSuccess = true;
				result.Code = (int)System.Net.HttpStatusCode.OK;
				result.Message = "Reminder template deleted successfully.";
			}
			catch (Exception ex)
			{
				result.IsSuccess = false;
				result.Code = (int)System.Net.HttpStatusCode.InternalServerError;
				result.Message = ex.Message;
			}
			return result;
		}
	}
}

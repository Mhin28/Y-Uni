using Repositories.Models;
using Repositories.Repositories;
using Repositories.ViewModels.ReminderModel;
using Repositories.ViewModels.ResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.ReminderService
{
	public class ReminderService : IReminderService
	{
		private readonly IReminderRepo _repo;
		public ReminderService(IReminderRepo repo)
		{
			_repo = repo;
		}

		public async Task<ResultModel> GetAllAsync()
		{
			var result = new ResultModel();
			try
			{
				var reminders = await _repo.GetAllAsync();
				result.IsSuccess = true;
				result.Code = (int)System.Net.HttpStatusCode.OK;
				result.Data = reminders;
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
				var reminder = await _repo.GetByIdAsync(id);
				if (reminder == null)
				{
					result.IsSuccess = false;
					result.Code = (int)System.Net.HttpStatusCode.NotFound;
					result.Message = "Reminder not found.";
					return result;
				}
				result.IsSuccess = true;
				result.Code = (int)System.Net.HttpStatusCode.OK;
				result.Data = reminder;
			}
			catch (Exception ex)
			{
				result.IsSuccess = false;
				result.Code = (int)System.Net.HttpStatusCode.InternalServerError;
				result.Message = ex.Message;
			}
			return result;
		}
		public async Task<ResultModel> CreateAsync(PostReminderModel model)
		{
			var result = new ResultModel()
			{
				IsSuccess = false,
				Code = (int)System.Net.HttpStatusCode.BadRequest,
				Message = "Invalid reminder data."
			};
			try
			{
				
				var reminder = new Reminder
				{
					ReminderId = Guid.NewGuid(),
					UserId = model.UserId,
					EventId = model.EventId,
					AssignmentId = model.AssignmentId,
					TemplateId = model.TemplateId,
					Status = model.Status,
					NotificationChannel = model.NotificationChannel,
					ReminderTime = model.ReminderTime
				};
				await _repo.CreateAsync(reminder);
				
				result.IsSuccess = true;
				result.Code = (int)System.Net.HttpStatusCode.Created;
				result.Data = reminder;
			}
			catch (Exception ex)
			{
				result.IsSuccess = false;
				result.Code = (int)System.Net.HttpStatusCode.InternalServerError;
				result.Message = ex.Message;
			}
			return result;
		}

		public async Task<ResultModel> UpdateAsync(ReminderModel model)
		{
			var result = new ResultModel()
			{
				IsSuccess = false,
				Code = (int)System.Net.HttpStatusCode.BadRequest,
				Message = "Invalid reminder data."
			};
			try
			{
				var reminder = await _repo.GetByIdAsync(model.ReminderId);
				if (reminder == null)
				{
					result.IsSuccess = false;
					result.Code = (int)System.Net.HttpStatusCode.NotFound;
					result.Message = "Reminder not found.";
					return result;
				}
				if (model.UserId.HasValue)
					reminder.UserId = model.UserId.Value;

				if (model.EventId.HasValue)
					reminder.EventId = model.EventId.Value;

				if (model.AssignmentId.HasValue)
					reminder.AssignmentId = model.AssignmentId.Value;

				if (model.TemplateId.HasValue)
					reminder.TemplateId = model.TemplateId.Value;

				if (!string.IsNullOrEmpty(model.Status))
					reminder.Status = model.Status;

				if (!string.IsNullOrEmpty(model.NotificationChannel))
					reminder.NotificationChannel = model.NotificationChannel;

				if (model.ReminderTime.HasValue)
					reminder.ReminderTime = model.ReminderTime.Value;

				await _repo.UpdateAsync(reminder);

				result.IsSuccess = true;
				result.Code = (int)System.Net.HttpStatusCode.OK;
				result.Data = reminder;
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
			var result = new ResultModel();
			try
			{
				var reminder = await _repo.GetByIdAsync(id);
				if (reminder == null)
				{
					result.IsSuccess = false;
					result.Code = (int)System.Net.HttpStatusCode.NotFound;
					result.Message = "Reminder not found.";
					return result;
				}
				await _repo.RemoveAsync(reminder);
				result.IsSuccess = true;
				result.Code = (int)System.Net.HttpStatusCode.NoContent;
			}
			catch (Exception ex)
			{
				result.IsSuccess = false;
				result.Code = (int)System.Net.HttpStatusCode.InternalServerError;
				result.Message = ex.Message;
			}
			return result;
		}

		public async Task<ResultModel> GetRemindersByUserIdAsync(Guid userId)
		{
			var result = new ResultModel();
			try
			{
				var reminders = await _repo.GetRemindersByUserIdAsync(userId);
				result.IsSuccess = true;
				result.Code = (int)System.Net.HttpStatusCode.OK;
				result.Data = reminders;
			}
			catch (Exception ex)
			{
				result.IsSuccess = false;
				result.Code = (int)System.Net.HttpStatusCode.InternalServerError;
				result.Message = ex.Message;
			}
			return result;
		}

		public async Task<ResultModel> GetRemindersByEventIdAsync(Guid eventId)
		{
			var result = new ResultModel();
			try
			{
				var reminders = await _repo.GetRemindersByEventIdAsync(eventId);
				result.IsSuccess = true;
				result.Code = (int)System.Net.HttpStatusCode.OK;
				result.Data = reminders;
			}
			catch (Exception ex)
			{
				result.IsSuccess = false;
				result.Code = (int)System.Net.HttpStatusCode.InternalServerError;
				result.Message = ex.Message;
			}
			return result;
		}

		public async Task<ResultModel> GetRemindersByAssignmentIdAsync(Guid assignmentId)
		{
			var result = new ResultModel();
			try
			{
				var reminders = await _repo.GetRemindersByAssignmentIdAsync(assignmentId);
				result.IsSuccess = true;
				result.Code = (int)System.Net.HttpStatusCode.OK;
				result.Data = reminders;
			}
			catch (Exception ex)
			{
				result.IsSuccess = false;
				result.Code = (int)System.Net.HttpStatusCode.InternalServerError;
				result.Message = ex.Message;
			}
			return result;
		}

		public async Task<ResultModel> GetRemindersByTemplateIdAsync(Guid templateId)
		{
			var result = new ResultModel();
			try
			{
				var reminders = await _repo.GetRemindersByTemplateIdAsync(templateId);
				result.IsSuccess = true;
				result.Code = (int)System.Net.HttpStatusCode.OK;
				result.Data = reminders;
			}
			catch (Exception ex)
			{
				result.IsSuccess = false;
				result.Code = (int)System.Net.HttpStatusCode.InternalServerError;
				result.Message = ex.Message;
			}
			return result;
		}

		public async Task<ResultModel> GetRemindersByStatusAsync(string status)
		{
			var result = new ResultModel();
			try
			{
				var reminders = await _repo.GetRemindersByStatusAsync(status);
				result.IsSuccess = true;
				result.Code = (int)System.Net.HttpStatusCode.OK;
				result.Data = reminders;
			}
			catch (Exception ex)
			{
				result.IsSuccess = false;
				result.Code = (int)System.Net.HttpStatusCode.InternalServerError;
				result.Message = ex.Message;
			}
			return result;
		}

		public async Task<ResultModel> GetRemindersByNotificationChannelAsync(string notificationChannel)
		{
			var result = new ResultModel();
			try
			{
				var reminders = await _repo.GetRemindersByNotificationChannelAsync(notificationChannel);
				result.IsSuccess = true;
				result.Code = (int)System.Net.HttpStatusCode.OK;
				result.Data = reminders;
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

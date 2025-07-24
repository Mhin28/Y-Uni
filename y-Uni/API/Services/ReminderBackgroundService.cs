using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Repositories.Repositories;
using Services.Services.EmailService;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace API.Services
{
    public class ReminderBackgroundService : BackgroundService
    {
        private readonly ILogger<ReminderBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _period = TimeSpan.FromMinutes(1); // Check every 1 minute

        public ReminderBackgroundService(
            ILogger<ReminderBackgroundService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Reminder Background Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessPendingReminders();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing reminders");
                }

                await Task.Delay(_period, stoppingToken);
            }
        }

        private async Task ProcessPendingReminders()
        {
            using var scope = _serviceProvider.CreateScope();
            var reminderRepo = scope.ServiceProvider.GetRequiredService<IReminderRepo>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            // Get all pending reminders that are due
            var dueReminders = await reminderRepo.GetDueRemindersAsync(DateTime.Now);

            _logger.LogInformation($"Found {dueReminders.Count} due reminders to process");

            foreach (var reminder in dueReminders)
            {
                try
                {
                    await ProcessSingleReminder(reminder, emailService, reminderRepo);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to process reminder {reminder.ReminderId}");
                    
                    // Mark as failed
                    reminder.Status = "failed";
                    
                    // Clear navigation properties to avoid tracking conflicts
                    reminder.User = null;
                    reminder.Assignment = null;
                    reminder.Event = null;
                    reminder.Template = null;
                    
                    await reminderRepo.UpdateAsync(reminder);
                }
            }
        }

        private async Task ProcessSingleReminder(
            Repositories.Models.Reminder reminder, 
            IEmailService emailService, 
            IReminderRepo reminderRepo)
        {
            _logger.LogInformation($"Processing reminder {reminder.ReminderId} for channel {reminder.NotificationChannel}");

            bool success = false;

            switch (reminder.NotificationChannel?.ToLower())
            {
                case "email":
                    success = await SendEmailNotification(reminder, emailService);
                    break;
                case "push":
                    success = await SendPushNotification(reminder);
                    break;
                case "sms":
                    success = await SendSMSNotification(reminder);
                    break;
                default:
                    _logger.LogWarning($"Unknown notification channel: {reminder.NotificationChannel}");
                    break;
            }

            // Update reminder status
            reminder.Status = success ? "sent" : "failed";
            
            // Clear navigation properties to avoid tracking conflicts
            reminder.User = null;
            reminder.Assignment = null;
            reminder.Event = null;
            reminder.Template = null;
            
            await reminderRepo.UpdateAsync(reminder);

            _logger.LogInformation($"Reminder {reminder.ReminderId} marked as {reminder.Status}");
        }

        private async Task<bool> SendEmailNotification(Repositories.Models.Reminder reminder, IEmailService emailService)
        {
            try
            {
                string subject = "Reminder Notification";
                string body = "";

                if (reminder.AssignmentId.HasValue)
                {
                    // Get assignment details
                    using var scope = _serviceProvider.CreateScope();
                    var assignmentRepo = scope.ServiceProvider.GetRequiredService<IAssignmentRepo>();
                    var assignment = await assignmentRepo.GetByIdAsync(reminder.AssignmentId.Value);
                    
                    if (assignment != null)
                    {
                        subject = $"Assignment Reminder: {assignment.Title}";
                        body = $@"
                            <h2>Assignment Reminder</h2>
                            <p><strong>Title:</strong> {assignment.Title}</p>
                            <p><strong>Description:</strong> {assignment.Description}</p>
                            <p><strong>Due Date:</strong> {assignment.DueDate:yyyy-MM-dd HH:mm}</p>
                            <p><strong>Status:</strong> {assignment.Status}</p>
                            <p>Don't forget to complete your assignment!</p>
                        ";
                    }
                }
                else if (reminder.EventId.HasValue)
                {
                    // Get event details
                    using var scope = _serviceProvider.CreateScope();
                    var eventRepo = scope.ServiceProvider.GetRequiredService<IEventRepo>();
                    var eventItem = await eventRepo.GetByIdAsync(reminder.EventId.Value);
                    
                    if (eventItem != null)
                    {
                        subject = $"Event Reminder: {eventItem.Title}";
                        body = $@"
                            <h2>Event Reminder</h2>
                            <p><strong>Title:</strong> {eventItem.Title}</p>
                            <p><strong>Description:</strong> {eventItem.Description}</p>
                            <p><strong>Start Time:</strong> {eventItem.StartDateTime:yyyy-MM-dd HH:mm}</p>
                            <p><strong>End Time:</strong> {eventItem.EndDateTime:yyyy-MM-dd HH:mm}</p>
                            <p>Your event is coming up!</p>
                        ";
                    }
                }

                // Get user email
                using var userScope = _serviceProvider.CreateScope();
                var userRepo = userScope.ServiceProvider.GetRequiredService<IUserRepo>();
                var user = await userRepo.GetByIdAsync(reminder.UserId.Value);

                if (user != null && !string.IsNullOrEmpty(user.Email))
                {
                    // Send proper reminder email
                    await emailService.SendReminderEmailAsync(user.Email, subject, body);
                    _logger.LogInformation($"Email sent successfully to {user.Email}");
                    return true;
                }
                else
                {
                    _logger.LogWarning($"User not found or email missing for reminder {reminder.ReminderId}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email for reminder {reminder.ReminderId}");
                return false;
            }
        }

        private async Task<bool> SendPushNotification(Repositories.Models.Reminder reminder)
        {
            // TODO: Implement push notification logic
            // This would integrate with your mobile app's push notification service
            _logger.LogInformation($"Push notification would be sent for reminder {reminder.ReminderId}");
            
            // For now, just return true to simulate success
            await Task.Delay(100); // Simulate async operation
            return true;
        }

        private async Task<bool> SendSMSNotification(Repositories.Models.Reminder reminder)
        {
            // TODO: Implement SMS logic (Twilio, etc.)
            _logger.LogInformation($"SMS would be sent for reminder {reminder.ReminderId}");
            
            // For now, just return true to simulate success
            await Task.Delay(100); // Simulate async operation
            return true;
        }
    }
}
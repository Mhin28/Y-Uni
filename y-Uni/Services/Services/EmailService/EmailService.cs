using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.EmailService
{
    public class EmailService : IEmailService
    {
        public async Task SendVerificationEmailAsync(string toEmail, string code)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("yunibuddy18@gmail.com", "pjue wfbe qsfe mwhp"),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("yunibuddy18@gmail.com"),
                Subject = "Mã xác thực tài khoản Y-Uni",
                Body = $"Mã xác thực của bạn là: {code}",
                IsBodyHtml = false,
            };
            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}

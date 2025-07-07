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
                Body = $@"
        <div style='font-family: Segoe UI, Arial, sans-serif; max-width: 500px; margin: 40px auto; border-radius: 12px; box-shadow: 0 4px 16px rgba(0,0,0,0.1); background: #ffffff; overflow: hidden;'>
            <div style='background: linear-gradient(135deg, #2d8cf0, #69b7ff); padding: 24px; text-align: center;'>
                <h1 style='color: white; margin: 0; font-size: 24px;'>Chào mừng đến với <span style='color: #ffd700;'>Y-Uni</span>!</h1>
            </div>
            <div style='padding: 32px 24px;'>
                <p style='font-size: 16px; color: #333; margin-bottom: 20px;'>
                    Cảm ơn bạn đã đăng ký tài khoản. Để xác thực tài khoản, vui lòng sử dụng mã dưới đây:
                </p>
                <div style='text-align: center; margin: 24px 0;'>
                    <div style='display: inline-block; font-size: 36px; letter-spacing: 6px; color: #ffffff; background: #2d8cf0; padding: 16px 32px; border-radius: 10px; font-weight: bold; box-shadow: 0 4px 12px rgba(0,0,0,0.1);'>
                        {code}
                    </div>
                </div>
                <p style='font-size: 14px; color: #555; text-align: center;'>
                    Mã này có hiệu lực trong <strong>10 phút</strong>.<br>
                    Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email này.
                </p>
            </div>
            <div style='background: #f8f8f8; padding: 16px; text-align: center; font-size: 12px; color: #888;'>
                Y-Uni Team &copy; 2024
            </div>
        </div>
    ",
                IsBodyHtml = true,
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}

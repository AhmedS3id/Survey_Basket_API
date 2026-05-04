using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;
using Survey_Basket_API.Settings;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace Survey_Basket_API.Services
{
    public class EmailServices(IOptions<MailSettings> mailSetting,ILogger<EmailServices> logger) : IEmailSender
    {
        private readonly MailSettings _mailSetting = mailSetting.Value;
        private readonly ILogger<EmailServices> _logger = logger;

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var message = new MimeMessage
            {
                Sender = MailboxAddress.Parse(_mailSetting.Mail),
                Subject = subject
            };
            message.To.Add(MailboxAddress.Parse(email));
            var builder = new BodyBuilder
            {
                HtmlBody = htmlMessage
            };

            message.Body=builder.ToMessageBody();

            //using packag mailkit
            using var smtp = new SmtpClient();


            // ✅ تعديل 1 — تجاهل الـ SSL Certificate
            smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;

            // ✅ تعديل 2 — تغيير StartTls لـ StartTlsWhenAvailable
            await smtp.ConnectAsync(_mailSetting.Host, _mailSetting.Port, SecureSocketOptions.StartTlsWhenAvailable);
            _logger.LogInformation("Sending email to :{email}", email);
            //  دى لازم على ال production
            //   smtp.Connect(_mailSetting.Host, _mailSetting.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSetting.Mail,_mailSetting.Password);
            await smtp.SendAsync(message);
            smtp.Disconnect(true);
        }
    }
}

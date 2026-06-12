using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Survey_Basket_API.Settings;

namespace Survey_Basket_API.Health
{
    public class MailProviderHealthCheck(IOptions<MailSettings> mailSetting) : IHealthCheck
    {
        private readonly MailSettings _mailSetting = mailSetting.Value;

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                using var smtp = new SmtpClient();


                // ✅ تعديل 1 — تجاهل الـ SSL Certificate
                smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;

                // ✅ تعديل 2 — تغيير StartTls لـ StartTlsWhenAvailable
                await smtp.ConnectAsync(_mailSetting.Host, _mailSetting.Port, SecureSocketOptions.StartTlsWhenAvailable);
                //  دى لازم على ال production
                //   smtp.Connect(_mailSetting.Host, _mailSetting.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(_mailSetting.Mail, _mailSetting.Password);

                return await Task.FromResult(HealthCheckResult.Healthy());
            }
            catch (Exception ex)
            {
                return await Task.FromResult(HealthCheckResult.Unhealthy(exception: ex));
            }
        }
    }
}

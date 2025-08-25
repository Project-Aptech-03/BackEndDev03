using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Services.Interface;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ProjectDemoWebApi.Services
{
    public class EmailService : IEmailSender, IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            await SendEmailInternalAsync(email, subject, htmlMessage);
        }

        async Task IEmailService.SendEmailAsync(string toEmail, string subject, string body)
        {
            await SendEmailInternalAsync(toEmail, subject, body);
        }

        private async Task SendEmailInternalAsync(string toEmail, string subject, string body)
        {
            var mail = new MailMessage
            {
                From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mail.To.Add(toEmail);

            using var smtp = new SmtpClient(_settings.SmtpServer, _settings.SmtpPort)
            {
                Credentials = new NetworkCredential(_settings.SenderEmail, _settings.SenderPassword),
                EnableSsl = true
            };

            await smtp.SendMailAsync(mail);
        }
    }
}

using ClothingShop.Infrastructure.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace ClothingShop.Infrastructure.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SmtpEmailService> _logger;

        public SmtpEmailService(IConfiguration configuration, ILogger<SmtpEmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(MailboxAddress.Parse(_configuration["Smtp:From"]));
                message.To.Add(MailboxAddress.Parse(toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = htmlBody
                };
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();

                var host = _configuration["Smtp:Host"];
                var port = int.Parse(_configuration["Smtp:Port"] ?? "587");

                await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);

                var username = _configuration["Smtp:Username"];
                var password = _configuration["Smtp:Password"];

                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                {
                    await client.AuthenticateAsync(username, password);
                }

                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("Email sent successfully to {ToEmail}", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {ToEmail}. Error: {ErrorMessage}", toEmail, ex.Message);
                throw new InvalidOperationException($"Failed to send email: {ex.Message}", ex);
            }
        }

        public async Task SendOtpEmailAsync(string toEmail, string fullName, string otp, int expiryMinutes)
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "Email", "OtpTemplate.html");

            if (!File.Exists(filePath))
            {
                _logger.LogError("Email template not found at: {FilePath}", filePath);
                throw new FileNotFoundException("Không tìm thấy file template email tại: " + filePath);
            }

            var template = await File.ReadAllTextAsync(filePath);

            var emailBody = template
                .Replace("{{FullName}}", fullName)
                .Replace("{{Otp}}", otp)
                .Replace("{{Expiry}}", expiryMinutes.ToString());


            await SendEmailAsync(toEmail, "Mã OTP Xác Minh - DARREN SHOP", emailBody);
        }
    }
}
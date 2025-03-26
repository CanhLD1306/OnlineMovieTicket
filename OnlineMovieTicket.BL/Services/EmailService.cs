using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using OnlineMovieTicket.DAL.Configurations;
using OnlineMovieTicket.BL.Interfaces;
using System.Reflection;

namespace OnlineMovieTicket.BL.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly string _resourcePath;
        
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            _resourcePath = _resourcePath = Path.Combine(Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.Parent?.FullName ?? "", "Resources");

        }

        public async Task SendEmailAsync(string email, string subject, string templateName, Dictionary<string, string> placeholders)
        {
            var emailSettings = _configuration.GetSection("EmailSettings").Get<EmailSettings>();
            if (emailSettings == null ||
                string.IsNullOrEmpty(emailSettings.SenderEmail) ||
                string.IsNullOrEmpty(emailSettings.SenderPassword) ||
                string.IsNullOrEmpty(emailSettings.SMTPServer))
            {
                throw new Exception("Thiếu thông tin cấu hình email.");
            }
            string body = await LoadTemplateAsync(templateName, placeholders);
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(emailSettings.SenderName, emailSettings.SenderEmail));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = body };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();

            try
            {
                await client.ConnectAsync(emailSettings.SMTPServer, emailSettings.SMTPPort,
                    emailSettings.SMTPPort == 465 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(emailSettings.SenderEmail, emailSettings.SenderPassword);
                await client.SendAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi gửi email: {ex.Message}");
                throw;
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }

        private async Task<string> LoadTemplateAsync(string templateName, Dictionary<string, string> placeholders)
        {
            var filePath = Path.Combine(_resourcePath, $"{templateName}.html");

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Template '{templateName}' not found at {filePath}");


            string content =  await File.ReadAllTextAsync(filePath);

            foreach (var placeholder in placeholders)
            {
                content = content.Replace($"{{{{{placeholder.Key}}}}}", placeholder.Value);
            }

            return content;
        }
    }
}

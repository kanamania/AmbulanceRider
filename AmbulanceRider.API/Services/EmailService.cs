using System.Net;
using System.Net.Mail;

namespace AmbulanceRider.API.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly IConfiguration _configuration;

    public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        try
        {
            var smtpHost = _configuration["Email:SmtpHost"];
            var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
            var smtpUsername = _configuration["Email:SmtpUsername"];
            var smtpPassword = _configuration["Email:SmtpPassword"];
            var fromEmail = _configuration["Email:FromEmail"];
            var fromName = _configuration["Email:FromName"] ?? "AmbulanceRider";
            var enableSsl = bool.Parse(_configuration["Email:EnableSsl"] ?? "true");

            // If SMTP is not configured, just log the email
            if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpUsername))
            {
                _logger.LogInformation($"Email would be sent to {to} with subject: {subject}");
                _logger.LogInformation($"Email body: {body}");
                return;
            }

            using var smtpClient = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                EnableSsl = enableSsl
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail ?? smtpUsername, fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(to);

            await smtpClient.SendMailAsync(mailMessage);
            _logger.LogInformation($"Email sent successfully to {to} with subject: {subject}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error sending email to {to}");
            throw;
        }
    }
}

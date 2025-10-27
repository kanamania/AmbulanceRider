namespace AmbulanceRider.API.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(string to, string subject, string body)
    {
        // TODO: Implement actual email sending logic using SMTP or a service like SendGrid
        _logger.LogInformation($"Email would be sent to {to} with subject: {subject}");
        return Task.CompletedTask;
    }
}

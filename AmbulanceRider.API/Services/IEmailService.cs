namespace AmbulanceRider.API.Services;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
    Task SendEmailWithAttachmentsAsync(List<string> to, string subject, string body,
        Dictionary<string, byte[]> attachments);
}

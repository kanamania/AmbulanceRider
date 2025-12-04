namespace AmbulanceRider.API.DTOs;

public class SendInvoiceEmailDto
{
    public List<string> RecipientEmails { get; set; } = new();
    public string? Subject { get; set; }
    public string? Body { get; set; }
}

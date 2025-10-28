using System.ComponentModel.DataAnnotations;

namespace AmbulanceRider.API.Models;

/// <summary>
/// Stores performance metrics for monitoring
/// </summary>
public class PerformanceLog
{
    [Key]
    public int Id { get; set; }
    
    public string Endpoint { get; set; } = string.Empty;
    public string HttpMethod { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public double ResponseTimeMs { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? UserId { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public long? RequestSize { get; set; }
    public long? ResponseSize { get; set; }
    public string? ErrorMessage { get; set; }
}

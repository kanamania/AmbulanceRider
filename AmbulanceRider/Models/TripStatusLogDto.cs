namespace AmbulanceRider.Models;

public class TripStatusLogDto
{
    public int Id { get; set; }
    public int TripId { get; set; }
    public string FromStatus { get; set; } = string.Empty;
    public string ToStatus { get; set; } = string.Empty;
    public Guid ChangedBy { get; set; }
    public DateTime ChangedAt { get; set; }
    public string? Notes { get; set; }
    public string? RejectionReason { get; set; }
    public bool IsForceComplete { get; set; }
    public string? UserRole { get; set; }
    public string? UserName { get; set; }
    
    // Navigation properties
    public UserDto? User { get; set; }
}

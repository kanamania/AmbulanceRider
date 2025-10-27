using System.ComponentModel.DataAnnotations;

namespace AmbulanceRider.API.Models;

/// <summary>
/// Tracks all status changes for trips with full audit trail
/// </summary>
public class TripStatusLog : BaseModel
{
    [Required]
    public int TripId { get; set; }
    
    [Required]
    public TripStatus FromStatus { get; set; }
    
    [Required]
    public TripStatus ToStatus { get; set; }
    
    [Required]
    public Guid ChangedBy { get; set; }
    
    [Required]
    public DateTime ChangedAt { get; set; }
    
    [StringLength(1000)]
    public string? Notes { get; set; }
    
    [StringLength(500)]
    public string? RejectionReason { get; set; }
    
    public bool IsForceComplete { get; set; }
    
    [StringLength(100)]
    public string? UserRole { get; set; }
    
    [StringLength(255)]
    public string? UserName { get; set; }
    
    // Navigation properties
    public virtual Trip? Trip { get; set; }
    public virtual User? User { get; set; }
}

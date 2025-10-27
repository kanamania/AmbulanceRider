using System.ComponentModel.DataAnnotations;

namespace AmbulanceRider.API.Models;

public class Trip : BaseModel
{
    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    public DateTime ScheduledStartTime { get; set; }
    public DateTime? ActualStartTime { get; set; }
    public DateTime? ActualEndTime { get; set; }
    
    public TripStatus Status { get; set; } = TripStatus.Pending;
    
    [StringLength(500)]
    public string? RejectionReason { get; set; }
    
    // Foreign keys
    public int RouteId { get; set; }
    public int? VehicleId { get; set; }
    public Guid? DriverId { get; set; }
    public Guid? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    
    // Navigation properties
    public virtual Route? Route { get; set; }
    public virtual Vehicle? Vehicle { get; set; }
    public virtual User? Driver { get; set; }
    public virtual User? Approver { get; set; }
}

public enum TripStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2,
    InProgress = 3,
    Completed = 4,
    Cancelled = 5
}

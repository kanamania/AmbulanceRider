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
    
    // Location coordinates
    [Required]
    public double FromLatitude { get; set; }
    
    [Required]
    public double FromLongitude { get; set; }
    
    [Required]
    public double ToLatitude { get; set; }
    
    [Required]
    public double ToLongitude { get; set; }
    
    [StringLength(200)]
    public string? FromLocationName { get; set; }
    
    [StringLength(200)]
    public string? ToLocationName { get; set; }
    
    // Foreign keys
    public int? VehicleId { get; set; }
    public Guid? DriverId { get; set; }
    public Guid? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public int? TripTypeId { get; set; }
    
    // Trip management properties
    public bool RequiresGpsVerification { get; set; }
    public double? CompletionLatitude { get; set; }
    public double? CompletionLongitude { get; set; }
    public double? CompletionAccuracy { get; set; }
    public string? PhotoUrl { get; set; }
    public string? SignatureUrl { get; set; }
    public bool AutoApproved { get; set; }
    public bool AutoStarted { get; set; }
    public string? OptimizedRoute { get; set; }
    public string? RoutePolyline { get; set; }
    public double? EstimatedDistance { get; set; } // in meters
    public int? EstimatedDuration { get; set; } // in seconds
    
    // Dimensions
    public decimal? Weight { get; set; }
    public decimal? Height { get; set; }
    public decimal? Length { get; set; }
    public decimal? Width { get; set; }
    
    // Pricing
    public decimal? BasePrice { get; set; }
    public decimal? TaxAmount { get; set; }
    public decimal? TotalPrice { get; set; }
    
    // Pricing Matrix reference
    public int? PricingMatrixId { get; set; }
    public virtual PricingMatrix? PricingMatrix { get; set; }
    
    // Navigation properties
    public virtual Vehicle? Vehicle { get; set; }
    public virtual User? Driver { get; set; }
    public virtual User? Approver { get; set; }
    public new virtual User? Creator { get; set; }
    public virtual TripType? TripType { get; set; }
    public virtual ICollection<TripAttributeValue> AttributeValues { get; set; } = new List<TripAttributeValue>();
    public bool IsPaid { get; set; } = false;
    public DateTime? PaidDate { get; set; } = null;
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

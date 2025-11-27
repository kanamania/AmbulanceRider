namespace AmbulanceRider.Models;

public class TripDto
{
    public int Id { get; set; }
    public int? TripTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime ScheduledStartTime { get; set; }
    public DateTime? ActualStartTime { get; set; }
    public DateTime? ActualEndTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? RejectionReason { get; set; }
    
    // Location coordinates
    public double FromLatitude { get; set; }
    public double FromLongitude { get; set; }
    public double ToLatitude { get; set; }
    public double ToLongitude { get; set; }
    public string? FromLocationName { get; set; }
    public string? ToLocationName { get; set; }
    
    public int? VehicleId { get; set; }
    public VehicleDto? Vehicle { get; set; }
    public Guid? DriverId { get; set; }
    public UserDto? Driver { get; set; }
    public Guid? ApprovedBy { get; set; }
    public UserDto? Approver { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public decimal BasePrice { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalPrice { get; set; }
    public int PricingMatrixId { get; set; }
}

public class CreateTripDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime ScheduledStartTime { get; set; } = DateTime.Now.AddHours(1);
    
    // Coordinates are required
    public double FromLatitude { get; set; }
    public double FromLongitude { get; set; }
    public double ToLatitude { get; set; }
    public double ToLongitude { get; set; }
    
    // Optional location names
    public string? FromLocationName { get; set; }
    public string? ToLocationName { get; set; }
    
    public int? VehicleId { get; set; }
    public Guid? DriverId { get; set; }
    public TelemetryDto? Telemetry { get; set; }
    public decimal Weight { get; set; }
    public decimal Height { get; set; }
    public decimal Length { get; set; }
    public decimal Width { get; set; }
    public decimal BasePrice { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalPrice { get; set; }
}

public class UpdateTripDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime? ScheduledStartTime { get; set; }
    
    // Optional coordinate updates
    public double? FromLatitude { get; set; }
    public double? FromLongitude { get; set; }
    public double? ToLatitude { get; set; }
    public double? ToLongitude { get; set; }
    public string? FromLocationName { get; set; }
    public string? ToLocationName { get; set; }
    
    public int? VehicleId { get; set; }
    public Guid? DriverId { get; set; }
    public DateTime? ActualStartTime { get; set; }
    public DateTime? ActualEndTime { get; set; }
    
    // Telemetry
    public TelemetryDto? Telemetry { get; set; }
    public decimal Weight { get; set; }
    public decimal Height { get; set; }
    public decimal Length { get; set; }
    public decimal Width { get; set; }
    public decimal BasePrice { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalPrice { get; set; }
}

public class ApproveTripDto
{
    public bool Approve { get; set; }
    public string? RejectionReason { get; set; }
}

public class StartTripDto
{
    public DateTime ActualStartTime { get; set; } = DateTime.Now;
}

public class CompleteTripDto
{
    public DateTime ActualEndTime { get; set; } = DateTime.Now;
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

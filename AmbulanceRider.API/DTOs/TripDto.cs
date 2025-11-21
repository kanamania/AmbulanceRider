using AmbulanceRider.API.Models;

namespace AmbulanceRider.API.DTOs;

public class TripDto
{
    public int Id { get; set; }
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
    
    // Trip Type
    public int? TripTypeId { get; set; }
    public TripTypeDto? TripType { get; set; }
    public List<TripAttributeValueDto> AttributeValues { get; set; } = new();

    public string? OptimizedRoute { get; set; }
    public string? RoutePolyline { get; set; }
    public double? EstimatedDistance { get; set; }
    public int? EstimatedDuration { get; set; }
}

public class CreateTripDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required DateTime ScheduledStartTime { get; set; } = DateTime.Now;
    
    // Coordinates are required
    public required double FromLatitude { get; set; }
    public required double FromLongitude { get; set; }
    public required double ToLatitude { get; set; }
    public required double ToLongitude { get; set; }
    
    // Optional location names
    public string? FromLocationName { get; set; }
    public string? ToLocationName { get; set; }
    
    public int? VehicleId { get; set; }
    public Guid? DriverId { get; set; }
    
    // Trip Type
    public int? TripTypeId { get; set; }
    public List<CreateTripAttributeValueDto>? AttributeValues { get; set; }
    
    // Telemetry
    public TelemetryDto? Telemetry { get; set; }
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
    
    // Trip Type
    public int? TripTypeId { get; set; }
    public List<UpdateTripAttributeValueDto>? AttributeValues { get; set; }
    
    // Telemetry
    public TelemetryDto? Telemetry { get; set; }
}

public class ApproveTripDto
{
    public required bool Approve { get; set; }
    public string? RejectionReason { get; set; }
    
    // Required when approving a trip
    public int? VehicleId { get; set; }
    
    // Telemetry
    public TelemetryDto? Telemetry { get; set; }
}

public class StartTripDto
{
    public required DateTime ActualStartTime { get; set; }
    
    // Telemetry
    public TelemetryDto? Telemetry { get; set; }
}

public class CompleteTripDto
{
    public required DateTime ActualEndTime { get; set; }
    
    // Telemetry
    public TelemetryDto? Telemetry { get; set; }
}

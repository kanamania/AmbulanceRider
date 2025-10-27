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
}

public class CreateTripDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required DateTime ScheduledStartTime { get; set; }
    
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
}

public class ApproveTripDto
{
    public required bool Approve { get; set; }
    public string? RejectionReason { get; set; }
}

public class StartTripDto
{
    public required DateTime ActualStartTime { get; set; }
}

public class CompleteTripDto
{
    public required DateTime ActualEndTime { get; set; }
}

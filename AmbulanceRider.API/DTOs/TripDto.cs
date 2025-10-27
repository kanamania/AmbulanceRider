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
    public int RouteId { get; set; }
    public RouteDto? Route { get; set; }
    public int? VehicleId { get; set; }
    public VehicleDto? Vehicle { get; set; }
    public int? DriverId { get; set; }
    public UserDto? Driver { get; set; }
    public int? ApprovedBy { get; set; }
    public UserDto? Approver { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateTripDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required DateTime ScheduledStartTime { get; set; }
    public required int RouteId { get; set; }
    public int? VehicleId { get; set; }
    public int? DriverId { get; set; }
}

public class UpdateTripDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime? ScheduledStartTime { get; set; }
    public int? RouteId { get; set; }
    public int? VehicleId { get; set; }
    public int? DriverId { get; set; }
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

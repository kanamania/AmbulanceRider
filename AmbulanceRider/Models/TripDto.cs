namespace AmbulanceRider.Models;

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
    public Guid? DriverId { get; set; }
    public UserDto? Driver { get; set; }
    public Guid? ApprovedBy { get; set; }
    public UserDto? Approver { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateTripDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime ScheduledStartTime { get; set; } = DateTime.Now.AddHours(1);
    public int RouteId { get; set; }
    public int? VehicleId { get; set; }
    public Guid? DriverId { get; set; }
}

public class UpdateTripDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime? ScheduledStartTime { get; set; }
    public int? RouteId { get; set; }
    public int? VehicleId { get; set; }
    public Guid? DriverId { get; set; }
    public DateTime? ActualStartTime { get; set; }
    public DateTime? ActualEndTime { get; set; }
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

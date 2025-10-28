namespace AmbulanceRider.API.DTOs;

public class DashboardStatsDto
{
    public int TotalTrips { get; set; }
    public int PendingTrips { get; set; }
    public int ApprovedTrips { get; set; }
    public int InProgressTrips { get; set; }
    public int CompletedTrips { get; set; }
    public int CancelledTrips { get; set; }
    public int RejectedTrips { get; set; }
    public int TotalVehicles { get; set; }
    public int ActiveVehicles { get; set; }
    public int TotalDrivers { get; set; }
    public int ActiveDrivers { get; set; }
    public int TotalUsers { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public class TripStatusStatsDto
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
}

public class TripDateStatsDto
{
    public DateTime Date { get; set; }
    public int Count { get; set; }
    public string Label { get; set; } = string.Empty;
}

public class VehicleUtilizationDto
{
    public int VehicleId { get; set; }
    public string VehicleName { get; set; } = string.Empty;
    public string PlateNumber { get; set; } = string.Empty;
    public int TotalTrips { get; set; }
    public int CompletedTrips { get; set; }
    public int InProgressTrips { get; set; }
    public double UtilizationRate { get; set; }
}

public class DriverPerformanceDto
{
    public Guid DriverId { get; set; }
    public string DriverName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int TotalTrips { get; set; }
    public int CompletedTrips { get; set; }
    public int InProgressTrips { get; set; }
    public int CancelledTrips { get; set; }
    public double CompletionRate { get; set; }
}

public class TelemetryStatsDto
{
    public int TotalEvents { get; set; }
    public int UniqueUsers { get; set; }
    public Dictionary<string, int> EventTypeCounts { get; set; } = new();
    public Dictionary<string, int> DeviceTypeCounts { get; set; } = new();
    public Dictionary<string, int> BrowserCounts { get; set; } = new();
    public Dictionary<string, int> OsCounts { get; set; } = new();
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public class TelemetryAnalyticsEventDto
{
    public int Id { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string? EventDetails { get; set; }
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public string? DeviceType { get; set; }
    public string? Browser { get; set; }
    public string? OperatingSystem { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? IpAddress { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class PerformanceMetricsDto
{
    public double AverageResponseTime { get; set; }
    public int TotalRequests { get; set; }
    public int FailedRequests { get; set; }
    public double ErrorRate { get; set; }
    public Dictionary<string, int> EndpointCounts { get; set; } = new();
    public Dictionary<string, double> EndpointAverageResponseTimes { get; set; } = new();
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

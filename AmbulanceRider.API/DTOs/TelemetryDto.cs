namespace AmbulanceRider.API.DTOs;

/// <summary>
/// Device and telemetry information collected from client
/// </summary>
public class TelemetryDto
{
    // Device Information
    public string? DeviceType { get; set; }  // "Mobile", "Desktop", "Tablet"
    public string? DeviceModel { get; set; }
    public string? OperatingSystem { get; set; }
    public string? OsVersion { get; set; }
    public string? Browser { get; set; }
    public string? BrowserVersion { get; set; }
    public string? AppVersion { get; set; }
    
    // Account Information
    public string? GoogleAccount { get; set; }  // Google account email (Android)
    public string? AppleAccount { get; set; }   // Apple ID (iOS)
    public string? AccountType { get; set; }    // "Google", "Apple", "None"
    
    // Installed Apps (JSON array of app names/packages)
    public string? InstalledApps { get; set; }
    public int? InstalledAppsCount { get; set; }
    
    // GPS/Location Information
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public double? Accuracy { get; set; }  // in meters
    public double? Altitude { get; set; }
    public double? Speed { get; set; }  // in m/s
    public double? Heading { get; set; }  // in degrees
    public DateTime? LocationTimestamp { get; set; }
    
    // Network Information
    public string? IpAddress { get; set; }
    public string? ConnectionType { get; set; }  // "wifi", "cellular", "ethernet"
    public bool? IsOnline { get; set; }
    
    // Screen Information
    public int? ScreenWidth { get; set; }
    public int? ScreenHeight { get; set; }
    public string? Orientation { get; set; }  // "portrait", "landscape"
    
    // Battery Information (if available)
    public double? BatteryLevel { get; set; }  // 0-1
    public bool? IsCharging { get; set; }
    
    // Timestamp
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Batch telemetry logging request
/// </summary>
public class BatchTelemetryDto
{
    public List<TelemetryEventDto> Events { get; set; } = new();
}

/// <summary>
/// Single telemetry event in a batch
/// </summary>
public class TelemetryEventDto
{
    public string EventType { get; set; } = string.Empty;
    public string? EventDetails { get; set; }
    public TelemetryDto Telemetry { get; set; } = new();
}

/// <summary>
/// Timeseries query request
/// </summary>
public class TimeseriesQueryDto
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? EventType { get; set; }
    public Guid? UserId { get; set; }
    public int Limit { get; set; } = 1000;
}

/// <summary>
/// Telemetry response with minimal data for timeseries
/// </summary>
public class TelemetryTimeseriesDto
{
    public int Id { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string? EventDetails { get; set; }
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public double? Speed { get; set; }
    public double? BatteryLevel { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? EventTimestamp { get; set; }
}

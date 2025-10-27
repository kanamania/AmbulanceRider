using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AmbulanceRider.API.Models;

/// <summary>
/// Stores telemetry data for analytics and debugging
/// </summary>
public class Telemetry
{
    [Key]
    public int Id { get; set; }
    
    // Event Information
    public string EventType { get; set; } = string.Empty;  // "Login", "Register", "TripCreate", "TripStatusUpdate", etc.
    public string? EventDetails { get; set; }  // JSON with additional context
    
    // User Information (nullable for anonymous events)
    public Guid? UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }
    
    // Device Information
    public string? DeviceType { get; set; }
    public string? DeviceModel { get; set; }
    public string? OperatingSystem { get; set; }
    public string? OsVersion { get; set; }
    public string? Browser { get; set; }
    public string? BrowserVersion { get; set; }
    public string? AppVersion { get; set; }
    
    // Account Information
    public string? GoogleAccount { get; set; }
    public string? AppleAccount { get; set; }
    public string? AccountType { get; set; }
    
    // Installed Apps
    public string? InstalledApps { get; set; }
    public int? InstalledAppsCount { get; set; }
    
    // GPS/Location Information
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public double? Accuracy { get; set; }
    public double? Altitude { get; set; }
    public double? Speed { get; set; }
    public double? Heading { get; set; }
    public DateTime? LocationTimestamp { get; set; }
    
    // Network Information
    public string? IpAddress { get; set; }
    public string? ConnectionType { get; set; }
    public bool? IsOnline { get; set; }
    
    // Screen Information
    public int? ScreenWidth { get; set; }
    public int? ScreenHeight { get; set; }
    public string? Orientation { get; set; }
    
    // Battery Information
    public double? BatteryLevel { get; set; }
    public bool? IsCharging { get; set; }
    
    // Timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? EventTimestamp { get; set; }
}

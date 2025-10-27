using AmbulanceRider.API.DTOs;
using AmbulanceRider.API.Models;
using AmbulanceRider.API.Repositories;

namespace AmbulanceRider.API.Services;

public class TelemetryService : ITelemetryService
{
    private readonly ITelemetryRepository _telemetryRepository;
    private readonly ILogger<TelemetryService> _logger;

    public TelemetryService(ITelemetryRepository telemetryRepository, ILogger<TelemetryService> logger)
    {
        _telemetryRepository = telemetryRepository;
        _logger = logger;
    }

    public async Task LogTelemetryAsync(string eventType, TelemetryDto? telemetryDto, Guid? userId = null, string? eventDetails = null)
    {
        try
        {
            // Don't fail the main operation if telemetry fails
            if (telemetryDto == null)
            {
                _logger.LogDebug("No telemetry data provided for event: {EventType}", eventType);
                return;
            }

            var telemetry = new Telemetry
            {
                EventType = eventType,
                EventDetails = eventDetails,
                UserId = userId,
                
                // Device Information
                DeviceType = telemetryDto.DeviceType,
                DeviceModel = telemetryDto.DeviceModel,
                OperatingSystem = telemetryDto.OperatingSystem,
                OsVersion = telemetryDto.OsVersion,
                Browser = telemetryDto.Browser,
                BrowserVersion = telemetryDto.BrowserVersion,
                AppVersion = telemetryDto.AppVersion,
                
                // Account Information
                GoogleAccount = telemetryDto.GoogleAccount,
                AppleAccount = telemetryDto.AppleAccount,
                AccountType = telemetryDto.AccountType,
                
                // Installed Apps
                InstalledApps = telemetryDto.InstalledApps,
                InstalledAppsCount = telemetryDto.InstalledAppsCount,
                
                // GPS/Location Information
                Latitude = telemetryDto.Latitude,
                Longitude = telemetryDto.Longitude,
                Accuracy = telemetryDto.Accuracy,
                Altitude = telemetryDto.Altitude,
                Speed = telemetryDto.Speed,
                Heading = telemetryDto.Heading,
                LocationTimestamp = telemetryDto.LocationTimestamp,
                
                // Network Information
                IpAddress = telemetryDto.IpAddress,
                ConnectionType = telemetryDto.ConnectionType,
                IsOnline = telemetryDto.IsOnline,
                
                // Screen Information
                ScreenWidth = telemetryDto.ScreenWidth,
                ScreenHeight = telemetryDto.ScreenHeight,
                Orientation = telemetryDto.Orientation,
                
                // Battery Information
                BatteryLevel = telemetryDto.BatteryLevel,
                IsCharging = telemetryDto.IsCharging,
                
                // Timestamps
                EventTimestamp = telemetryDto.Timestamp,
                CreatedAt = DateTime.UtcNow
            };

            await _telemetryRepository.CreateAsync(telemetry);
            
            _logger.LogInformation("Telemetry logged for event: {EventType}, User: {UserId}, Location: {Lat},{Lon}", 
                eventType, userId, telemetry.Latitude, telemetry.Longitude);
        }
        catch (Exception ex)
        {
            // Log but don't throw - telemetry should never break the main flow
            _logger.LogError(ex, "Failed to log telemetry for event: {EventType}", eventType);
        }
    }

    public async Task<int> LogBatchTelemetryAsync(IEnumerable<(string eventType, TelemetryDto telemetryDto, Guid? userId, string? eventDetails)> telemetryBatch)
    {
        try
        {
            var telemetries = telemetryBatch.Select(item => new Telemetry
            {
                EventType = item.eventType,
                EventDetails = item.eventDetails,
                UserId = item.userId,
                
                // Device Information
                DeviceType = item.telemetryDto.DeviceType,
                DeviceModel = item.telemetryDto.DeviceModel,
                OperatingSystem = item.telemetryDto.OperatingSystem,
                OsVersion = item.telemetryDto.OsVersion,
                Browser = item.telemetryDto.Browser,
                BrowserVersion = item.telemetryDto.BrowserVersion,
                AppVersion = item.telemetryDto.AppVersion,
                
                // Account Information
                GoogleAccount = item.telemetryDto.GoogleAccount,
                AppleAccount = item.telemetryDto.AppleAccount,
                AccountType = item.telemetryDto.AccountType,
                
                // Installed Apps
                InstalledApps = item.telemetryDto.InstalledApps,
                InstalledAppsCount = item.telemetryDto.InstalledAppsCount,
                
                // GPS/Location Information
                Latitude = item.telemetryDto.Latitude,
                Longitude = item.telemetryDto.Longitude,
                Accuracy = item.telemetryDto.Accuracy,
                Altitude = item.telemetryDto.Altitude,
                Speed = item.telemetryDto.Speed,
                Heading = item.telemetryDto.Heading,
                LocationTimestamp = item.telemetryDto.LocationTimestamp,
                
                // Network Information
                IpAddress = item.telemetryDto.IpAddress,
                ConnectionType = item.telemetryDto.ConnectionType,
                IsOnline = item.telemetryDto.IsOnline,
                
                // Screen Information
                ScreenWidth = item.telemetryDto.ScreenWidth,
                ScreenHeight = item.telemetryDto.ScreenHeight,
                Orientation = item.telemetryDto.Orientation,
                
                // Battery Information
                BatteryLevel = item.telemetryDto.BatteryLevel,
                IsCharging = item.telemetryDto.IsCharging,
                
                // Timestamps
                EventTimestamp = item.telemetryDto.Timestamp,
                CreatedAt = DateTime.UtcNow
            }).ToList();

            var count = await _telemetryRepository.CreateBatchAsync(telemetries);
            
            _logger.LogInformation("Batch telemetry logged: {Count} records", count);
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log batch telemetry");
            return 0;
        }
    }

    public async Task<IEnumerable<Telemetry>> GetTimeseriesDataAsync(DateTime startTime, DateTime endTime, string? eventType = null, int limit = 1000)
    {
        return await _telemetryRepository.GetTimeseriesAsync(startTime, endTime, eventType, limit);
    }

    public async Task<IEnumerable<Telemetry>> GetUserTimeseriesAsync(Guid userId, DateTime startTime, DateTime endTime, string? eventType = null)
    {
        return await _telemetryRepository.GetByTimeRangeAsync(startTime, endTime, userId, eventType);
    }
}

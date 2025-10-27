using AmbulanceRider.Models;
using Microsoft.JSInterop;

namespace AmbulanceRider.Services;

public class TelemetryService
{
    private readonly IJSRuntime _jsRuntime;

    public TelemetryService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    /// <summary>
    /// Collects telemetry data from the browser
    /// </summary>
    public async Task<TelemetryDto> CollectTelemetryAsync()
    {
        try
        {
            var telemetry = new TelemetryDto
            {
                Timestamp = DateTime.UtcNow
            };

            // Collect device and browser information
            try
            {
                var userAgent = await _jsRuntime.InvokeAsync<string>("eval", "navigator.userAgent");
                telemetry.Browser = GetBrowserName(userAgent);
                telemetry.OperatingSystem = GetOperatingSystem(userAgent);
                telemetry.DeviceType = GetDeviceType(userAgent);
                
                // Get app version from meta tag or JavaScript
                try
                {
                    telemetry.AppVersion = await _jsRuntime.InvokeAsync<string>("getAppVersion");
                }
                catch
                {
                    telemetry.AppVersion = "1.0.0"; // Fallback version
                }
            }
            catch { /* Ignore if not available */ }
            
            // Collect account information based on OS
            try
            {
                var accountInfo = await GetAccountInfoAsync(telemetry.OperatingSystem);
                if (accountInfo != null)
                {
                    telemetry.GoogleAccount = accountInfo.GoogleAccount;
                    telemetry.AppleAccount = accountInfo.AppleAccount;
                    telemetry.AccountType = accountInfo.AccountType;
                }
            }
            catch { /* Ignore if not available */ }
            
            // Collect installed apps information
            try
            {
                var appsInfo = await GetInstalledAppsAsync();
                if (appsInfo != null)
                {
                    telemetry.InstalledApps = appsInfo.AppsJson;
                    telemetry.InstalledAppsCount = appsInfo.Count;
                }
            }
            catch { /* Ignore if not available */ }

            // Collect screen information
            try
            {
                telemetry.ScreenWidth = await _jsRuntime.InvokeAsync<int>("eval", "window.screen.width");
                telemetry.ScreenHeight = await _jsRuntime.InvokeAsync<int>("eval", "window.screen.height");
                
                var orientation = await _jsRuntime.InvokeAsync<string>("eval", 
                    "window.screen.orientation ? window.screen.orientation.type : (window.innerWidth > window.innerHeight ? 'landscape' : 'portrait')");
                telemetry.Orientation = orientation?.Contains("landscape") == true ? "landscape" : "portrait";
            }
            catch { /* Ignore if not available */ }

            // Collect network information
            try
            {
                var isOnline = await _jsRuntime.InvokeAsync<bool>("eval", "navigator.onLine");
                telemetry.IsOnline = isOnline;
                
                // Try to get connection type
                var connectionType = await _jsRuntime.InvokeAsync<string>("eval", 
                    "navigator.connection ? navigator.connection.effectiveType : null");
                telemetry.ConnectionType = connectionType;
            }
            catch { /* Ignore if not available */ }

            // Collect GPS location (if permission granted)
            try
            {
                var location = await GetLocationAsync();
                if (location != null)
                {
                    telemetry.Latitude = location.Latitude;
                    telemetry.Longitude = location.Longitude;
                    telemetry.Accuracy = location.Accuracy;
                    telemetry.Altitude = location.Altitude;
                    telemetry.Speed = location.Speed;
                    telemetry.Heading = location.Heading;
                    telemetry.LocationTimestamp = location.Timestamp;
                }
            }
            catch { /* Ignore if location not available or permission denied */ }

            // Collect battery information (if available)
            try
            {
                var batteryLevel = await _jsRuntime.InvokeAsync<double?>("eval", 
                    "navigator.getBattery ? navigator.getBattery().then(b => b.level) : null");
                if (batteryLevel.HasValue)
                {
                    telemetry.BatteryLevel = batteryLevel.Value;
                }

                var isCharging = await _jsRuntime.InvokeAsync<bool?>("eval", 
                    "navigator.getBattery ? navigator.getBattery().then(b => b.charging) : null");
                if (isCharging.HasValue)
                {
                    telemetry.IsCharging = isCharging.Value;
                }
            }
            catch { /* Ignore if battery API not available */ }

            return telemetry;
        }
        catch (Exception)
        {
            // Return minimal telemetry if collection fails
            return new TelemetryDto { Timestamp = DateTime.UtcNow };
        }
    }

    /// <summary>
    /// Gets current GPS location
    /// </summary>
    private async Task<LocationData?> GetLocationAsync()
    {
        try
        {
            var position = await _jsRuntime.InvokeAsync<GeolocationPosition?>("getLocation");
            if (position != null)
            {
                return new LocationData
                {
                    Latitude = position.Coords.Latitude,
                    Longitude = position.Coords.Longitude,
                    Accuracy = position.Coords.Accuracy,
                    Altitude = position.Coords.Altitude,
                    Speed = position.Coords.Speed,
                    Heading = position.Coords.Heading,
                    Timestamp = DateTime.UtcNow
                };
            }
        }
        catch { /* Location not available */ }

        return null;
    }

    private string GetBrowserName(string userAgent)
    {
        if (userAgent.Contains("Firefox")) return "Firefox";
        if (userAgent.Contains("Edg")) return "Edge";
        if (userAgent.Contains("Chrome")) return "Chrome";
        if (userAgent.Contains("Safari")) return "Safari";
        if (userAgent.Contains("Opera") || userAgent.Contains("OPR")) return "Opera";
        return "Unknown";
    }

    private string GetOperatingSystem(string userAgent)
    {
        if (userAgent.Contains("Windows")) return "Windows";
        if (userAgent.Contains("Mac")) return "macOS";
        if (userAgent.Contains("Linux")) return "Linux";
        if (userAgent.Contains("Android")) return "Android";
        if (userAgent.Contains("iOS") || userAgent.Contains("iPhone") || userAgent.Contains("iPad")) return "iOS";
        return "Unknown";
    }

    private string GetDeviceType(string userAgent)
    {
        if (userAgent.Contains("Mobile") || userAgent.Contains("Android") || userAgent.Contains("iPhone")) 
            return "Mobile";
        if (userAgent.Contains("Tablet") || userAgent.Contains("iPad")) 
            return "Tablet";
        return "Desktop";
    }

    /// <summary>
    /// Gets account information based on OS type
    /// </summary>
    private async Task<AccountInfo?> GetAccountInfoAsync(string? operatingSystem)
    {
        try
        {
            // Try to get account info from JavaScript
            var accountData = await _jsRuntime.InvokeAsync<AccountInfo?>("getAccountInfo");
            if (accountData != null)
            {
                return accountData;
            }

            // Fallback: determine account type based on OS
            if (!string.IsNullOrEmpty(operatingSystem))
            {
                if (operatingSystem.Contains("Android"))
                {
                    return new AccountInfo { AccountType = "Google" };
                }
                else if (operatingSystem.Contains("iOS") || operatingSystem.Contains("Mac"))
                {
                    return new AccountInfo { AccountType = "Apple" };
                }
            }
        }
        catch { /* Ignore if not available */ }

        return new AccountInfo { AccountType = "None" };
    }

    /// <summary>
    /// Gets installed apps information (primarily for mobile/PWA)
    /// </summary>
    private async Task<InstalledAppsInfo?> GetInstalledAppsAsync()
    {
        try
        {
            var appsData = await _jsRuntime.InvokeAsync<InstalledAppsInfo?>("getInstalledApps");
            return appsData;
        }
        catch { /* Ignore if not available */ }

        return null;
    }

    private class LocationData
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? Accuracy { get; set; }
        public double? Altitude { get; set; }
        public double? Speed { get; set; }
        public double? Heading { get; set; }
        public DateTime Timestamp { get; set; }
    }

    private class GeolocationPosition
    {
        public GeolocationCoordinates Coords { get; set; } = new();
        public long Timestamp { get; set; }
    }

    private class GeolocationCoordinates
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Accuracy { get; set; }
        public double? Altitude { get; set; }
        public double? AltitudeAccuracy { get; set; }
        public double? Heading { get; set; }
        public double? Speed { get; set; }
    }

    private class AccountInfo
    {
        public string? GoogleAccount { get; set; }
        public string? AppleAccount { get; set; }
        public string? AccountType { get; set; }
    }

    private class InstalledAppsInfo
    {
        public string? AppsJson { get; set; }
        public int Count { get; set; }
    }
}

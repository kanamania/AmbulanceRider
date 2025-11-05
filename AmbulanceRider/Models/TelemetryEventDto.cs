using System;

namespace AmbulanceRider.Models
{
    public class TelemetryEventDto
    {
        public string Id { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string PageUrl { get; set; } = string.Empty;
        public string DeviceType { get; set; } = string.Empty;
        public string Browser { get; set; } = string.Empty;
        public string Os { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? Accuracy { get; set; }
        public string ScreenResolution { get; set; } = string.Empty;
        public string AppVersion { get; set; } = string.Empty;
        public Dictionary<string, object> CustomProperties { get; set; } = new();
    }
}

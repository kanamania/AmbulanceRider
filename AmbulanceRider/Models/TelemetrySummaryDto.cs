using System;

namespace AmbulanceRider.Models
{
    public class TelemetrySummaryDto
    {
        public int TotalEvents { get; set; }
        public int UniqueUsers { get; set; }
        public int UniqueDevices { get; set; }
        public DateTime? FirstEventDate { get; set; }
        public DateTime? LastEventDate { get; set; }
        public Dictionary<string, int> EventsByType { get; set; } = new();
        public Dictionary<string, int> EventsByDeviceType { get; set; } = new();
        public Dictionary<string, int> EventsByBrowser { get; set; } = new();
        public Dictionary<string, int> EventsByOs { get; set; } = new();
    }
}

using AmbulanceRider.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AmbulanceRider.Services
{
    public interface ITelemetryService
    {
        Task<TelemetrySummaryDto> GetSummaryAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<List<TelemetryEventDto>> GetEventsAsync(int page = 1, int pageSize = 20, string? eventType = null, string? userId = null, DateTime? startDate = null, DateTime? endDate = null);
        Task<Dictionary<string, int>> GetEventTypeDistributionAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<Dictionary<string, int>> GetDeviceTypeDistributionAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<List<TelemetryEventDto>> GetUserActivityAsync(string userId, DateTime? startDate = null, DateTime? endDate = null);
    }
}

using AmbulanceRider.API.DTOs;
using AmbulanceRider.API.Models;

namespace AmbulanceRider.API.Services;

public interface ITelemetryService
{
    Task LogTelemetryAsync(string eventType, TelemetryDto? telemetryDto, Guid? userId = null, string? eventDetails = null);
    Task<int> LogBatchTelemetryAsync(IEnumerable<(string eventType, TelemetryDto telemetryDto, Guid? userId, string? eventDetails)> telemetryBatch);
    Task<IEnumerable<Telemetry>> GetTimeseriesDataAsync(DateTime startTime, DateTime endTime, string? eventType = null, int limit = 1000);
    Task<IEnumerable<Telemetry>> GetUserTimeseriesAsync(Guid userId, DateTime startTime, DateTime endTime, string? eventType = null);
}

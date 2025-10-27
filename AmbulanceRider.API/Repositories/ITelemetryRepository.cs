using AmbulanceRider.API.Models;

namespace AmbulanceRider.API.Repositories;

public interface ITelemetryRepository
{
    Task<Telemetry> CreateAsync(Telemetry telemetry);
    Task<int> CreateBatchAsync(IEnumerable<Telemetry> telemetries);
    Task<IEnumerable<Telemetry>> GetByUserIdAsync(Guid userId, int limit = 100);
    Task<IEnumerable<Telemetry>> GetByEventTypeAsync(string eventType, int limit = 100);
    Task<IEnumerable<Telemetry>> GetByTimeRangeAsync(DateTime startTime, DateTime endTime, Guid? userId = null, string? eventType = null);
    Task<IEnumerable<Telemetry>> GetTimeseriesAsync(DateTime startTime, DateTime endTime, string? eventType = null, int limit = 1000);
}

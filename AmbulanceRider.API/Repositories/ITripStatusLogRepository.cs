using AmbulanceRider.API.Models;

namespace AmbulanceRider.API.Repositories;

public interface ITripStatusLogRepository : IRepository<TripStatusLog>
{
    Task<IEnumerable<TripStatusLog>> GetLogsByTripIdAsync(int tripId);
    Task<IEnumerable<TripStatusLog>> GetLogsByUserIdAsync(Guid userId);
    Task<IEnumerable<TripStatusLog>> GetRecentLogsAsync(int count = 50);
}

using AmbulanceRider.API.Data;
using AmbulanceRider.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AmbulanceRider.API.Repositories;

public class TripStatusLogRepository : Repository<TripStatusLog>, ITripStatusLogRepository
{
    public TripStatusLogRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TripStatusLog>> GetLogsByTripIdAsync(int tripId)
    {
        return await _context.TripStatusLogs
            .Include(tsl => tsl.User)
            .Include(tsl => tsl.Trip)
            .Where(tsl => tsl.TripId == tripId)
            .OrderByDescending(tsl => tsl.ChangedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<TripStatusLog>> GetLogsByUserIdAsync(Guid userId)
    {
        return await _context.TripStatusLogs
            .Include(tsl => tsl.User)
            .Include(tsl => tsl.Trip)
            .Where(tsl => tsl.ChangedBy == userId)
            .OrderByDescending(tsl => tsl.ChangedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<TripStatusLog>> GetRecentLogsAsync(int count = 50)
    {
        return await _context.TripStatusLogs
            .Include(tsl => tsl.User)
            .Include(tsl => tsl.Trip)
            .OrderByDescending(tsl => tsl.ChangedAt)
            .Take(count)
            .ToListAsync();
    }
}

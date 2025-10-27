using AmbulanceRider.API.Data;
using AmbulanceRider.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AmbulanceRider.API.Repositories;

public class TelemetryRepository : ITelemetryRepository
{
    private readonly ApplicationDbContext _context;

    public TelemetryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Telemetry> CreateAsync(Telemetry telemetry)
    {
        _context.Telemetries.Add(telemetry);
        await _context.SaveChangesAsync();
        return telemetry;
    }

    public async Task<IEnumerable<Telemetry>> GetByUserIdAsync(Guid userId, int limit = 100)
    {
        return await _context.Telemetries
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<IEnumerable<Telemetry>> GetByEventTypeAsync(string eventType, int limit = 100)
    {
        return await _context.Telemetries
            .Where(t => t.EventType == eventType)
            .OrderByDescending(t => t.CreatedAt)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<int> CreateBatchAsync(IEnumerable<Telemetry> telemetries)
    {
        await _context.Telemetries.AddRangeAsync(telemetries);
        return await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Telemetry>> GetByTimeRangeAsync(DateTime startTime, DateTime endTime, Guid? userId = null, string? eventType = null)
    {
        var query = _context.Telemetries
            .Where(t => t.CreatedAt >= startTime && t.CreatedAt <= endTime);

        if (userId.HasValue)
        {
            query = query.Where(t => t.UserId == userId.Value);
        }

        if (!string.IsNullOrEmpty(eventType))
        {
            query = query.Where(t => t.EventType == eventType);
        }

        return await query
            .OrderBy(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Telemetry>> GetTimeseriesAsync(DateTime startTime, DateTime endTime, string? eventType = null, int limit = 1000)
    {
        var query = _context.Telemetries
            .Where(t => t.CreatedAt >= startTime && t.CreatedAt <= endTime);

        if (!string.IsNullOrEmpty(eventType))
        {
            query = query.Where(t => t.EventType == eventType);
        }

        return await query
            .OrderBy(t => t.CreatedAt)
            .Take(limit)
            .Include(t => t.User)
            .ToListAsync();
    }
}

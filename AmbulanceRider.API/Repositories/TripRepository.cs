using AmbulanceRider.API.Data;
using AmbulanceRider.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AmbulanceRider.API.Repositories;

public class TripRepository : Repository<Trip>, ITripRepository
{
    public TripRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<Trip?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(t => t.Route)
                .ThenInclude(r => r!.FromLocation)
            .Include(t => t.Route)
                .ThenInclude(r => r!.ToLocation)
            .Include(t => t.Vehicle)
            .Include(t => t.Driver)
            .Include(t => t.Approver)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public override async Task<IEnumerable<Trip>> GetAllAsync()
    {
        return await _dbSet
            .Include(t => t.Route)
                .ThenInclude(r => r!.FromLocation)
            .Include(t => t.Route)
                .ThenInclude(r => r!.ToLocation)
            .Include(t => t.Vehicle)
            .Include(t => t.Driver)
            .Include(t => t.Approver)
            .Where(t => !t.IsDeleted)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Trip>> GetTripsByStatusAsync(TripStatus status)
    {
        return await _dbSet
            .Include(t => t.Route)
                .ThenInclude(r => r!.FromLocation)
            .Include(t => t.Route)
                .ThenInclude(r => r!.ToLocation)
            .Include(t => t.Vehicle)
            .Include(t => t.Driver)
            .Include(t => t.Approver)
            .Where(t => !t.IsDeleted && t.Status == status)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Trip>> GetPendingTripsAsync()
    {
        return await GetTripsByStatusAsync(TripStatus.Pending);
    }

    public async Task<IEnumerable<Trip>> GetTripsByRouteAsync(int routeId)
    {
        return await _dbSet
            .Include(t => t.Route)
                .ThenInclude(r => r!.FromLocation)
            .Include(t => t.Route)
                .ThenInclude(r => r!.ToLocation)
            .Include(t => t.Vehicle)
            .Include(t => t.Driver)
            .Include(t => t.Approver)
            .Where(t => !t.IsDeleted && t.RouteId == routeId)
            .OrderByDescending(t => t.ScheduledStartTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Trip>> GetTripsByDriverAsync(int driverId)
    {
        return await _dbSet
            .Include(t => t.Route)
                .ThenInclude(r => r!.FromLocation)
            .Include(t => t.Route)
                .ThenInclude(r => r!.ToLocation)
            .Include(t => t.Vehicle)
            .Include(t => t.Driver)
            .Include(t => t.Approver)
            .Where(t => !t.IsDeleted && t.DriverId == driverId)
            .OrderByDescending(t => t.ScheduledStartTime)
            .ToListAsync();
    }
}

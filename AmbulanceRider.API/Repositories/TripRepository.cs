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
            .Include(t => t.Vehicle)
            .Include(t => t.Driver)
            .Include(t => t.Approver)
            .Include(t => t.TripType)
            .Where(t => t.DeletedAt == null)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public override async Task<IEnumerable<Trip>> GetAllAsync()
    {
        return await _dbSet
            .Include(t => t.Vehicle)
            .Include(t => t.Driver)
            .Include(t => t.Approver)
            .Include(t => t.TripType)
            .Where(t => t.DeletedAt == null)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Trip>> GetTripsByStatusAsync(TripStatus status)
    {
        return await _dbSet
            .Include(t => t.Vehicle)
            .Include(t => t.Driver)
            .Include(t => t.Approver)
            .Include(t => t.TripType)
            .Where(t => t.DeletedAt == null && t.Status == status)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Trip>> GetPendingTripsAsync()
    {
        return await GetTripsByStatusAsync(TripStatus.Pending);
    }

    public async Task<IEnumerable<Trip>> GetTripsByDriverAsync(Guid driverId)
    {
        return await _dbSet
            .Include(t => t.Vehicle)
            .Include(t => t.Driver)
            .Include(t => t.Approver)
            .Include(t => t.TripType)
            .Where(t => t.DeletedAt == null && t.DriverId == driverId)
            .OrderByDescending(t => t.ScheduledStartTime)
            .ToListAsync();
    }

    public async Task<Vehicle?> GetVehicleAsync(int vehicleId)
    {
        return await _context.Set<Vehicle>()
            .FirstOrDefaultAsync(v => v.Id == vehicleId && v.DeletedAt == null);
    }

    public async Task<IEnumerable<Trip>> GetTripsToAutoStartAsync(DateTime currentTime, TimeSpan timeWindow)
    {
        var startTime = currentTime.Subtract(timeWindow);
        var endTime = currentTime;
        
        return await _dbSet
            .Include(t => t.Vehicle)
            .Include(t => t.Driver)
            .Where(t => t.DeletedAt == null 
                && t.Status == TripStatus.Approved 
                && t.ScheduledStartTime >= startTime 
                && t.ScheduledStartTime <= endTime
                && !t.AutoStarted)
            .ToListAsync();
    }
}

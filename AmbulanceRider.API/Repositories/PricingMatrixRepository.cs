using AmbulanceRider.API.Data;
using AmbulanceRider.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AmbulanceRider.API.Repositories;

public class PricingMatrixRepository : Repository<PricingMatrix>, IPricingMatrixRepository
{
    public PricingMatrixRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<PricingMatrix?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(p => p.Region)
            .Include(p => p.Company)
            .Include(p => p.VehicleType)
            .Include(p => p.TripType)
            .FirstOrDefaultAsync(p => p.Id == id && p.DeletedAt == null);
    }

    public override async Task<IEnumerable<PricingMatrix>> GetAllAsync()
    {
        return await _dbSet
            .Include(p => p.Region)
            .Include(p => p.Company)
            .Include(p => p.VehicleType)
            .Include(p => p.TripType)
            .Where(p => p.DeletedAt == null)
            .OrderByDescending(p => p.IsDefault)
            .ThenBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<PricingMatrix>> GetByDimensionsAsync(decimal weight, decimal height, decimal length, decimal width)
    {
        return await _dbSet
            .Include(p => p.Region)
            .Include(p => p.Company)
            .Include(p => p.VehicleType)
            .Include(p => p.TripType)
            .Where(p => p.DeletedAt == null &&
                weight >= p.MinWeight && weight <= p.MaxWeight &&
                height >= p.MinHeight && height <= p.MaxHeight &&
                length >= p.MinLength && length <= p.MaxLength &&
                width >= p.MinWidth && width <= p.MaxWidth)
            .OrderByDescending(p => p.IsDefault)
            .ToListAsync();
    }

    public async Task<PricingMatrix?> GetByRegionAndDimensionsAsync(string regionName, decimal weight, decimal height, decimal length, decimal width)
    {
        return await _dbSet
            .Include(p => p.Region)
            .Include(p => p.Company)
            .Include(p => p.VehicleType)
            .Include(p => p.TripType)
            .Where(p => p.DeletedAt == null &&
                p.Region != null &&
                p.Region.IsActive &&
                p.Region.Name == regionName &&
                weight >= p.MinWeight && weight <= p.MaxWeight &&
                height >= p.MinHeight && height <= p.MaxHeight &&
                length >= p.MinLength && length <= p.MaxLength &&
                width >= p.MinWidth && width <= p.MaxWidth)
            .FirstOrDefaultAsync();
    }

    public async Task<PricingMatrix?> GetDefaultByDimensionsAsync(decimal weight, decimal height, decimal length, decimal width)
    {
        return await _dbSet
            .Include(p => p.Region)
            .Include(p => p.Company)
            .Include(p => p.VehicleType)
            .Include(p => p.TripType)
            .Where(p => p.DeletedAt == null &&
                p.IsDefault &&
                weight >= p.MinWeight && weight <= p.MaxWeight &&
                height >= p.MinHeight && height <= p.MaxHeight &&
                length >= p.MinLength && length <= p.MaxLength &&
                width >= p.MinWidth && width <= p.MaxWidth)
            .FirstOrDefaultAsync();
    }
}

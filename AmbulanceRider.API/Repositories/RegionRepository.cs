using AmbulanceRider.API.Data;
using AmbulanceRider.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AmbulanceRider.API.Repositories;

public class RegionRepository : Repository<Region>, IRegionRepository
{
    public RegionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<Region?> GetByIdAsync(int id)
    {
        return await _dbSet
            .FirstOrDefaultAsync(r => r.Id == id && r.DeletedAt == null);
    }

    public override async Task<IEnumerable<Region>> GetAllAsync()
    {
        return await _dbSet
            .Where(r => r.DeletedAt == null)
            .OrderBy(r => r.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Region>> GetActiveRegionsAsync()
    {
        return await _dbSet
            .Where(r => r.DeletedAt == null && r.IsActive)
            .OrderBy(r => r.Name)
            .ToListAsync();
    }

    public async Task<Region?> GetDefaultRegionAsync()
    {
        return await _dbSet
            .FirstOrDefaultAsync(r => r.DeletedAt == null && r.IsDefault);
    }
}

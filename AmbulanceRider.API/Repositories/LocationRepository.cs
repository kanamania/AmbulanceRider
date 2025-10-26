using AmbulanceRider.API.Data;
using AmbulanceRider.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AmbulanceRider.API.Repositories;

public class LocationRepository : BaseRepository<Location>, ILocationRepository
{
    public LocationRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Location?> GetByNameAsync(string name)
    {
        return await _context.Locations.FirstOrDefaultAsync(l => l.Name.ToLower() == name.ToLower());
    }
}

using AmbulanceRider.API.Data;
using AmbulanceRider.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AmbulanceRider.API.Repositories;

public class VehicleRepository : Repository<Vehicle>, IVehicleRepository
{
    public VehicleRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Vehicle?> GetByIdWithTypesAsync(int id)
    {
        return await _dbSet
            .Include(v => v.VehicleTypes)
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<IEnumerable<Vehicle>> GetAllWithTypesAsync()
    {
        return await _dbSet
            .Include(v => v.VehicleTypes)
            .ToListAsync();
    }
}

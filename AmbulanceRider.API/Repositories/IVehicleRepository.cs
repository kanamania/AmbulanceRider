using AmbulanceRider.API.Models;

namespace AmbulanceRider.API.Repositories;

public interface IVehicleRepository : IRepository<Vehicle>
{
    Task<Vehicle?> GetByIdWithTypesAsync(int id);
    Task<IEnumerable<Vehicle>> GetAllWithTypesAsync();
}

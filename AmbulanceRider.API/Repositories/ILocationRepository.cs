using AmbulanceRider.API.Models;

namespace AmbulanceRider.API.Repositories;

public interface ILocationRepository : IBaseRepository<Location>
{
    Task<Location?> GetByNameAsync(string name);
}

using AmbulanceRider.API.Models;

namespace AmbulanceRider.API.Repositories;

public interface IRoleRepository : IRepository<Role>
{
    Task<Role?> GetByNameAsync(string name);
}

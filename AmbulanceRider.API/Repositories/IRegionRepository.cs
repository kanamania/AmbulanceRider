using AmbulanceRider.API.Models;

namespace AmbulanceRider.API.Repositories;

public interface IRegionRepository : IRepository<Region>
{
    Task<Region?> GetByIdAsync(int id);
    Task<IEnumerable<Region>> GetAllAsync();
    Task<IEnumerable<Region>> GetActiveRegionsAsync();
    Task<Region?> GetDefaultRegionAsync();
}

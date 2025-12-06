using AmbulanceRider.API.Models;

namespace AmbulanceRider.API.Repositories;

public interface IPricingMatrixRepository : IRepository<PricingMatrix>
{
    Task<PricingMatrix?> GetByIdAsync(int id);
    Task<IEnumerable<PricingMatrix>> GetAllAsync();
    Task<IEnumerable<PricingMatrix>> GetByDimensionsAsync(decimal weight, decimal height, decimal length, decimal width);
    Task<PricingMatrix?> GetByRegionAndDimensionsAsync(string regionName, decimal weight, decimal height, decimal length, decimal width);
    Task<PricingMatrix?> GetDefaultByDimensionsAsync(decimal weight, decimal height, decimal length, decimal width);
}

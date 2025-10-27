using AmbulanceRider.API.Models;

namespace AmbulanceRider.API.Repositories;

public interface ITripTypeRepository
{
    Task<IEnumerable<TripType>> GetAllAsync();
    Task<IEnumerable<TripType>> GetActiveAsync();
    Task<TripType?> GetByIdAsync(int id);
    Task<TripType?> GetByIdWithAttributesAsync(int id);
    Task AddAsync(TripType tripType);
    Task UpdateAsync(TripType tripType);
    Task DeleteAsync(int id);
}

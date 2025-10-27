using AmbulanceRider.API.Models;

namespace AmbulanceRider.API.Repositories;

public interface ITripTypeAttributeRepository
{
    Task<IEnumerable<TripTypeAttribute>> GetByTripTypeIdAsync(int tripTypeId);
    Task<TripTypeAttribute?> GetByIdAsync(int id);
    Task AddAsync(TripTypeAttribute attribute);
    Task UpdateAsync(TripTypeAttribute attribute);
    Task DeleteAsync(int id);
}

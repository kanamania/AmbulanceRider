using AmbulanceRider.API.Models;

namespace AmbulanceRider.API.Repositories;

public interface ITripAttributeValueRepository
{
    Task<IEnumerable<TripAttributeValue>> GetByTripIdAsync(int tripId);
    Task<TripAttributeValue?> GetByIdAsync(int id);
    Task AddAsync(TripAttributeValue value);
    Task AddRangeAsync(IEnumerable<TripAttributeValue> values);
    Task UpdateAsync(TripAttributeValue value);
    Task DeleteAsync(int id);
    Task DeleteByTripIdAsync(int tripId);
}

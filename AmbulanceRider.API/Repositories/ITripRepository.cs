using AmbulanceRider.API.Models;

namespace AmbulanceRider.API.Repositories;

public interface ITripRepository : IRepository<Trip>
{
    Task<IEnumerable<Trip>> GetTripsByStatusAsync(TripStatus status);
    Task<IEnumerable<Trip>> GetPendingTripsAsync();
    Task<IEnumerable<Trip>> GetTripsByRouteAsync(int routeId);
    Task<IEnumerable<Trip>> GetTripsByDriverAsync(Guid driverId);
}

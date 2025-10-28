using AmbulanceRider.API.Models;

namespace AmbulanceRider.API.Repositories;

public interface ITripRepository : IRepository<Trip>
{
    Task<IEnumerable<Trip>> GetTripsByStatusAsync(TripStatus status);
    Task<IEnumerable<Trip>> GetPendingTripsAsync();
    Task<IEnumerable<Trip>> GetTripsByDriverAsync(Guid driverId);
    Task<Vehicle?> GetVehicleAsync(int vehicleId);
    Task<IEnumerable<Trip>> GetTripsToAutoStartAsync(DateTime currentTime, TimeSpan timeWindow);
}

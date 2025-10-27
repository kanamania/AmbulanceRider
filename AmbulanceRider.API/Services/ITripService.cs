using AmbulanceRider.API.DTOs;
using AmbulanceRider.API.Models;

namespace AmbulanceRider.API.Services;

public interface ITripService
{
    Task<IEnumerable<TripDto>> GetAllTripsAsync();
    Task<TripDto?> GetTripByIdAsync(int id);
    Task<IEnumerable<TripDto>> GetTripsByStatusAsync(TripStatus status);
    Task<IEnumerable<TripDto>> GetPendingTripsAsync();
    Task<IEnumerable<TripDto>> GetTripsByDriverAsync(Guid driverId);
    Task<TripDto> CreateTripAsync(CreateTripDto createTripDto);
    Task<TripDto> UpdateTripAsync(int id, UpdateTripDto updateTripDto);
    Task<TripDto> ApproveTripAsync(int id, ApproveTripDto approveTripDto, Guid approverId);
    Task<TripDto> StartTripAsync(int id, StartTripDto startTripDto);
    Task<TripDto> CompleteTripAsync(int id, CompleteTripDto completeTripDto);
    Task<TripDto> CancelTripAsync(int id);
    Task DeleteTripAsync(int id);
}

using AmbulanceRider.API.DTOs;
using AmbulanceRider.API.Models;

namespace AmbulanceRider.API.Services;

public interface ITripService
{
    Task<IEnumerable<TripDto>> GetAllTripsAsync();
    Task<TripDto?> GetTripByIdAsync(int id);
    Task<IEnumerable<TripDto>> GetTripsByStatusAsync(TripStatus status);
    Task<IEnumerable<TripDto>> GetPendingTripsAsync();
    Task<TripDto> CreateTripAsync(CreateTripDto createTripDto);
    Task<TripDto> UpdateTripAsync(int id, UpdateTripDto updateTripDto);
    Task<TripDto> ApproveTripAsync(int id, ApproveTripDto approveTripDto, Guid approverId);
    Task<TripDto> StartTripAsync(int id, StartTripDto startTripDto);
    Task<TripDto> CompleteTripAsync(int id, CompleteTripDto completeTripDto);
    Task<TripDto> CancelTripAsync(int id, string? reason = null);
    
    /// <summary>
    /// Updates the status of a trip with validation and business rules
    /// </summary>
    /// <param name="tripId">The ID of the trip to update</param>
    /// <param name="updateDto">The status update details</param>
    /// <param name="userId">The ID of the user making the update</param>
    /// <param name="isAdminOrDispatcher">Whether the user is an admin or dispatcher</param>
    /// <returns>The updated trip</returns>
    Task<TripDto> UpdateTripStatusAsync(int tripId, UpdateTripStatusDto updateDto, Guid userId, bool isAdminOrDispatcher);
    
    Task<IEnumerable<TripDto>> GetTripsByDriverAsync(Guid driverId);
    
    Task DeleteTripAsync(int id);
    
    Task<IEnumerable<TripStatusLogDto>> GetTripStatusLogsAsync(int tripId);
}

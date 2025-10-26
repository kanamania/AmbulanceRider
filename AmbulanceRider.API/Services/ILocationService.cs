using AmbulanceRider.API.DTOs;

namespace AmbulanceRider.API.Services;

public interface ILocationService
{
    Task<IEnumerable<LocationDto>> GetAllLocationsAsync();
    Task<LocationDto?> GetLocationByIdAsync(int id);
    Task<LocationDto> CreateLocationAsync(CreateLocationDto createLocationDto);
    Task<LocationDto> UpdateLocationAsync(int id, UpdateLocationDto updateLocationDto);
    Task DeleteLocationAsync(int id);
}

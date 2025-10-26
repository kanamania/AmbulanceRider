using AmbulanceRider.API.DTOs;

namespace AmbulanceRider.API.Services;

public interface IRouteService
{
    Task<IEnumerable<RouteDto>> GetAllRoutesAsync();
    Task<RouteDto?> GetRouteByIdAsync(int id);
    Task<RouteDto> CreateRouteAsync(CreateRouteDto createRouteDto);
    Task<RouteDto> UpdateRouteAsync(int id, UpdateRouteDto updateRouteDto);
    Task DeleteRouteAsync(int id);
}

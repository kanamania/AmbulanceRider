using AmbulanceRider.API.DTOs;

namespace AmbulanceRider.API.Services;

public interface IRouteOptimizationService
{
    Task<OptimizedRouteDto> GetOptimizedRouteAsync(RouteOptimizationRequestDto request);
    Task<Dictionary<int, OptimizedRouteDto>> BatchOptimizeRoutesAsync(List<RouteOptimizationRequestDto> requests);
    Task<Dictionary<string, object>> GetDistanceMatrixAsync(List<RouteWaypointDto> origins, List<RouteWaypointDto> destinations);
    Task<string> GetStaticMapUrlAsync(string polyline, int width = 600, int height = 400);
    Task<byte[]> GetStaticMapImageAsync(string polyline, int width = 600, int height = 400);
}

using AmbulanceRider.API.DTOs;
using AmbulanceRider.API.Repositories;

namespace AmbulanceRider.API.Services;

public class RouteService : IRouteService
{
    private readonly IRouteRepository _routeRepository;

    public RouteService(IRouteRepository routeRepository)
    {
        _routeRepository = routeRepository;
    }

    public async Task<IEnumerable<RouteDto>> GetAllRoutesAsync()
    {
        var routes = await _routeRepository.GetAllAsync();
        return routes.Select(MapToDto);
    }

    public async Task<RouteDto?> GetRouteByIdAsync(int id)
    {
        var route = await _routeRepository.GetByIdAsync(id);
        return route == null ? null : MapToDto(route);
    }

    public async Task<RouteDto> CreateRouteAsync(CreateRouteDto createRouteDto)
    {
        var route = new Models.Route
        {
            Name = createRouteDto.Name,
            StartLocation = createRouteDto.StartLocation,
            EndLocation = createRouteDto.EndLocation,
            Distance = createRouteDto.Distance,
            EstimatedDuration = createRouteDto.EstimatedDuration,
            Description = createRouteDto.Description,
            CreatedAt = DateTime.UtcNow
        };

        await _routeRepository.AddAsync(route);
        return MapToDto(route);
    }

    public async Task<RouteDto> UpdateRouteAsync(int id, UpdateRouteDto updateRouteDto)
    {
        var route = await _routeRepository.GetByIdAsync(id);
        if (route == null)
        {
            throw new KeyNotFoundException("Route not found");
        }

        if (!string.IsNullOrEmpty(updateRouteDto.Name))
            route.Name = updateRouteDto.Name;
        
        if (!string.IsNullOrEmpty(updateRouteDto.StartLocation))
            route.StartLocation = updateRouteDto.StartLocation;
        
        if (!string.IsNullOrEmpty(updateRouteDto.EndLocation))
            route.EndLocation = updateRouteDto.EndLocation;
        
        if (updateRouteDto.Distance.HasValue)
            route.Distance = updateRouteDto.Distance.Value;
        
        if (updateRouteDto.EstimatedDuration.HasValue)
            route.EstimatedDuration = updateRouteDto.EstimatedDuration.Value;
        
        if (updateRouteDto.Description != null)
            route.Description = updateRouteDto.Description;

        route.UpdatedAt = DateTime.UtcNow;

        await _routeRepository.UpdateAsync(route);
        return MapToDto(route);
    }

    public async Task DeleteRouteAsync(int id)
    {
        var route = await _routeRepository.GetByIdAsync(id);
        if (route == null)
        {
            throw new KeyNotFoundException("Route not found");
        }

        route.DeletedAt = DateTime.UtcNow;
        await _routeRepository.UpdateAsync(route);
    }

    private static RouteDto MapToDto(Models.Route route)
    {
        return new RouteDto
        {
            Id = route.Id,
            Name = route.Name,
            StartLocation = route.StartLocation,
            EndLocation = route.EndLocation,
            Distance = route.Distance,
            EstimatedDuration = route.EstimatedDuration,
            Description = route.Description,
            CreatedAt = route.CreatedAt
        };
    }
}

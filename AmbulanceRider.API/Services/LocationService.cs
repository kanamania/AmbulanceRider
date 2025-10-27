using AmbulanceRider.API.Data;
using AmbulanceRider.API.DTOs;
using AmbulanceRider.API.Models;
using AmbulanceRider.API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AmbulanceRider.API.Services;

public class LocationService : ILocationService
{
    private readonly ILocationRepository _locationRepository;
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public LocationService(ILocationRepository locationRepository, ApplicationDbContext context, IConfiguration configuration)
    {
        _locationRepository = locationRepository;
        _context = context;
        _configuration = configuration;
    }

    public async Task<IEnumerable<LocationDto>> GetAllLocationsAsync()
    {
        var locations = await _locationRepository.GetAllAsync();
        return locations.Select(MapToDto);
    }

    public async Task<LocationDto?> GetLocationByIdAsync(int id)
    {
        var location = await _locationRepository.GetByIdAsync(id);
        return location == null ? null : MapToDto(location);
    }

    public async Task<LocationDto> CreateLocationAsync(CreateLocationDto createLocationDto)
    {
        var existingLocation = await _locationRepository.GetByNameAsync(createLocationDto.Name);
        if (existingLocation != null)
        {
            throw new InvalidOperationException("Location with this name already exists");
        }

        var location = new Location
        {
            Name = createLocationDto.Name,
            Latitude = createLocationDto.Latitude,
            Longitude = createLocationDto.Longitude,
            CreatedAt = DateTime.UtcNow
        };

        await _locationRepository.AddAsync(location);
        return MapToDto(location);
    }

    public async Task<LocationDto> UpdateLocationAsync(int id, UpdateLocationDto updateLocationDto)
    {
        var location = await _locationRepository.GetByIdAsync(id);
        if (location == null)
            throw new KeyNotFoundException("Location not found");

        if (!string.IsNullOrEmpty(updateLocationDto.Name))
            location.Name = updateLocationDto.Name;

        if (updateLocationDto.Latitude.HasValue)
            location.Latitude = updateLocationDto.Latitude.Value;

        if (updateLocationDto.Longitude.HasValue)
            location.Longitude = updateLocationDto.Longitude.Value;

        if (updateLocationDto.ImagePath != null)
            location.ImagePath = updateLocationDto.ImagePath;
            
        if (updateLocationDto.ImageUrl != null)
            location.ImageUrl = updateLocationDto.ImageUrl;

        location.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return MapToDto(location);
    }

    public async Task DeleteLocationAsync(int id)
    {
        var location = await _locationRepository.GetByIdAsync(id);
        if (location == null)
            throw new KeyNotFoundException("Location not found");

        // Delete the image file if it exists
        if (!string.IsNullOrEmpty(location.ImagePath))
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", location.ImagePath);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        await _locationRepository.DeleteAsync(location.Id);
    }

    private static LocationDto MapToDto(Location location)
    {
        return new LocationDto
        {
            Id = location.Id,
            Name = location.Name,
            Latitude = location.Latitude,
            Longitude = location.Longitude,
            ImagePath = location.ImagePath,
            ImageUrl = location.ImageUrl,
            CreatedAt = location.CreatedAt
        };
    }
}

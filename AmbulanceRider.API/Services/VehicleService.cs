using AmbulanceRider.API.Data;
using AmbulanceRider.API.DTOs;
using AmbulanceRider.API.Models;
using AmbulanceRider.API.Repositories;

namespace AmbulanceRider.API.Services;

public class VehicleService : IVehicleService
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly ApplicationDbContext _context;

    public VehicleService(IVehicleRepository vehicleRepository, ApplicationDbContext context)
    {
        _vehicleRepository = vehicleRepository;
        _context = context;
    }

    public async Task<IEnumerable<VehicleDto>> GetAllVehiclesAsync()
    {
        var vehicles = await _vehicleRepository.GetAllWithTypesAsync();
        return vehicles.Select(MapToDto);
    }

    public async Task<VehicleDto?> GetVehicleByIdAsync(int id)
    {
        var vehicle = await _vehicleRepository.GetByIdWithTypesAsync(id);
        return vehicle == null ? null : MapToDto(vehicle);
    }

    public async Task<VehicleDto> CreateVehicleAsync(CreateVehicleDto createVehicleDto)
    {
        var vehicle = new Vehicle
        {
            Name = createVehicleDto.Name,
            Image = createVehicleDto.ImagePath,
            CreatedAt = DateTime.UtcNow
        };

        await _vehicleRepository.AddAsync(vehicle);

        // Add types
        foreach (var typeName in createVehicleDto.Types)
        {
            var vehicleType = new VehicleType
            {
                Name = typeName
            };
            _context.VehicleTypes.Add(vehicleType);
        }
        await _context.SaveChangesAsync();

        return (await GetVehicleByIdAsync(vehicle.Id))!;
    }

    public async Task<VehicleDto> UpdateVehicleAsync(int id, UpdateVehicleDto updateVehicleDto)
    {
        var vehicle = await _vehicleRepository.GetByIdWithTypesAsync(id);
        if (vehicle == null)
        {
            throw new KeyNotFoundException("Vehicle not found");
        }

        if (!string.IsNullOrEmpty(updateVehicleDto.Name))
            vehicle.Name = updateVehicleDto.Name;
        
        if (updateVehicleDto.Image != null)
            vehicle.Image = updateVehicleDto.ImagePath;

        vehicle.UpdatedAt = DateTime.UtcNow;

        await _vehicleRepository.UpdateAsync(vehicle);

        // Update types if provided
        if (updateVehicleDto.Types != null)
        {
            var existingTypes = vehicle.VehicleTypes.ToList();
            foreach (var type in existingTypes)
            {
                _context.VehicleTypes.Remove(type);
            }

            foreach (var typeName in updateVehicleDto.Types)
            {
                var vehicleType = new VehicleType
                {
                    Name = typeName
                };
                _context.VehicleTypes.Add(vehicleType);
            }
            await _context.SaveChangesAsync();
        }

        return (await GetVehicleByIdAsync(id))!;
    }

    public async Task DeleteVehicleAsync(int id)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id);
        if (vehicle == null)
        {
            throw new KeyNotFoundException("Vehicle not found");
        }

        vehicle.DeletedAt = DateTime.UtcNow;
        await _vehicleRepository.UpdateAsync(vehicle);
    }

    private static VehicleDto MapToDto(Vehicle vehicle)
    {
        return new VehicleDto
        {
            Id = vehicle.Id,
            Name = vehicle.Name,
            ImagePath = vehicle.Image,
            Types = vehicle.VehicleTypes.Select(vt => vt.Name).ToList(),
            CreatedAt = vehicle.CreatedAt
        };
    }
}

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
            PlateNumber = createVehicleDto.PlateNumber,
            VehicleTypeId = createVehicleDto.VehicleTypeId,
            Image = createVehicleDto.ImagePath,
            CreatedAt = DateTime.UtcNow
        };

        await _vehicleRepository.AddAsync(vehicle);
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
        
        if (!string.IsNullOrEmpty(updateVehicleDto.PlateNumber))
            vehicle.PlateNumber = updateVehicleDto.PlateNumber;
        
        if (updateVehicleDto.VehicleTypeId.HasValue)
            vehicle.VehicleTypeId = updateVehicleDto.VehicleTypeId.Value;
        
        if (!string.IsNullOrEmpty(updateVehicleDto.ImagePath))
            vehicle.Image = updateVehicleDto.ImagePath;
        
        if (updateVehicleDto.RemoveImage)
            vehicle.Image = null;

        vehicle.UpdatedAt = DateTime.UtcNow;

        await _vehicleRepository.UpdateAsync(vehicle);
        await _context.SaveChangesAsync();

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
            PlateNumber = vehicle.PlateNumber,
            ImagePath = vehicle.Image,
            VehicleTypeId = vehicle.VehicleTypeId,
            VehicleTypeName = vehicle.VehicleType?.Name,
            AssignedDrivers = vehicle.VehicleDrivers
                .Where(vd => vd.DeletedAt == null)
                .Select(vd => new UserDto
                {
                    Id = vd.User.Id.ToString(),
                    Name = $"{vd.User.FirstName} {vd.User.LastName}",
                    FirstName = vd.User.FirstName,
                    LastName = vd.User.LastName,
                    Email = vd.User.Email!,
                    PhoneNumber = vd.User.PhoneNumber,
                    ImagePath = vd.User.ImagePath,
                    ImageUrl = vd.User.ImageUrl,
                    Roles = new List<string>(),
                    CreatedAt = vd.User.CreatedAt,
                    UpdatedAt = vd.User.UpdatedAt
                }).ToList(),
            CreatedAt = vehicle.CreatedAt
        };
    }
}

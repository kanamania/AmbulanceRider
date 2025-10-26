using AmbulanceRider.API.DTOs;

namespace AmbulanceRider.API.Services;

public interface IVehicleService
{
    Task<IEnumerable<VehicleDto>> GetAllVehiclesAsync();
    Task<VehicleDto?> GetVehicleByIdAsync(int id);
    Task<VehicleDto> CreateVehicleAsync(CreateVehicleDto createVehicleDto);
    Task<VehicleDto> UpdateVehicleAsync(int id, UpdateVehicleDto updateVehicleDto);
    Task DeleteVehicleAsync(int id);
}

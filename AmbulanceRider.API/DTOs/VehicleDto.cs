using Microsoft.AspNetCore.Http;

namespace AmbulanceRider.API.DTOs;

public class VehicleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PlateNumber { get; set; } = string.Empty;
    public string? ImagePath { get; set; }
    public string? ImageUrl { get; set; }
    public int VehicleTypeId { get; set; }
    public string? VehicleTypeName { get; set; }
    public List<UserDto> AssignedDrivers { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class CreateVehicleDto
{
    public string Name { get; set; } = string.Empty;
    public string PlateNumber { get; set; } = string.Empty;
    public int VehicleTypeId { get; set; }
    public string? ImagePath { get; set; }
    public string? ImageUrl { get; set; }
}

public class UpdateVehicleDto
{
    public string? Name { get; set; }
    public string? PlateNumber { get; set; }
    public int? VehicleTypeId { get; set; }
    public bool RemoveImage { get; set; }
    public string? ImagePath { get; set; }
    public string? ImageUrl { get; set; }
}

public class VehicleTypeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

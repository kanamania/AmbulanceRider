using Microsoft.AspNetCore.Http;

namespace AmbulanceRider.API.DTOs;

public class LocationDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ImagePath { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateLocationDto
{
    public string Name { get; set; } = string.Empty;
    public IFormFile? Image { get; set; }
    public string? ImagePath { get; set; }
    public string? ImageUrl { get; set; }
}

public class UpdateLocationDto
{
    public string? Name { get; set; }
    public IFormFile? Image { get; set; }
    public bool RemoveImage { get; set; }
    public string? ImagePath { get; set; }
    public string? ImageUrl { get; set; }
}

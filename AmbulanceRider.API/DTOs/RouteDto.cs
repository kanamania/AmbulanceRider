using AmbulanceRider.API.Models;

namespace AmbulanceRider.API.DTOs;

public class RouteDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Distance { get; set; }
    public int EstimatedDuration { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public Location? FromLocation { get; set; }
    public Location? ToLocation { get; set; }
    public int FromLocationId { get; set; }
    public int ToLocationId { get; set; }
}

public class CreateRouteDto
{
    public required string Name { get; set; }
    public required int FromLocationId { get; set; }
    public required int ToLocationId { get; set; }
    public double Distance { get; set; }
    public int EstimatedDuration { get; set; }
    public string? Description { get; set; }
}

public class UpdateRouteDto
{
    public string? Name { get; set; }
    public int? FromLocationId { get; set; }
    public int? ToLocationId { get; set; }
    public double? Distance { get; set; }
    public int? EstimatedDuration { get; set; }
    public string? Description { get; set; }
}

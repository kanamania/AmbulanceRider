namespace AmbulanceRider.API.DTOs;

public class RouteDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string StartLocation { get; set; } = string.Empty;
    public string EndLocation { get; set; } = string.Empty;
    public double Distance { get; set; }
    public int EstimatedDuration { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateRouteDto
{
    public string Name { get; set; } = string.Empty;
    public string StartLocation { get; set; } = string.Empty;
    public string EndLocation { get; set; } = string.Empty;
    public double Distance { get; set; }
    public int EstimatedDuration { get; set; }
    public string? Description { get; set; }
}

public class UpdateRouteDto
{
    public string? Name { get; set; }
    public string? StartLocation { get; set; }
    public string? EndLocation { get; set; }
    public double? Distance { get; set; }
    public int? EstimatedDuration { get; set; }
    public string? Description { get; set; }
}

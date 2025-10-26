namespace AmbulanceRider.Models;

public class VehicleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Image { get; set; }
    public List<string> Types { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class CreateVehicleDto
{
    public string Name { get; set; } = string.Empty;
    public string? Image { get; set; }
    public List<string> Types { get; set; } = new();
}

public class UpdateVehicleDto
{
    public string? Name { get; set; }
    public string? Image { get; set; }
    public List<string>? Types { get; set; }
}

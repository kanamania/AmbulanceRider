namespace AmbulanceRider.API.Models;

public class Route : BaseModel
{
    public string Name { get; set; } = string.Empty;
    public string StartLocation { get; set; } = string.Empty;
    public string EndLocation { get; set; } = string.Empty;
    public double Distance { get; set; }
    public int EstimatedDuration { get; set; } // in minutes
    public string? Description { get; set; }
}

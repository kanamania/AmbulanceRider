namespace AmbulanceRider.API.Models;

public class Route : BaseModel
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Distance { get; set; }
    public int EstimatedDuration { get; set; } // in minutes
    
    // Foreign keys
    public int FromLocationId { get; set; }
    public int ToLocationId { get; set; }
    
    // Navigation properties
    public virtual Location? FromLocation { get; set; }
    public virtual Location? ToLocation { get; set; }
}

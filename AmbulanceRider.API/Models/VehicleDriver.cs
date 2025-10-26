namespace AmbulanceRider.API.Models;

public class VehicleDriver : BaseModel
{
    public int UserId { get; set; }
    public int VehicleId { get; set; }
    
    // Navigation properties
    public User User { get; set; } = null!;
    public Vehicle Vehicle { get; set; } = null!;
}

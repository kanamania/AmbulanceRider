namespace AmbulanceRider.API.Models;

public class Vehicle : BaseModel
{
    public string Name { get; set; } = string.Empty;
    public string PlateNumber { get; set; } = string.Empty;
    public string? Image { get; set; }
    public int VehicleTypeId { get; set; }
    
    // Navigation properties
    public VehicleType? VehicleType { get; set; }
    public ICollection<VehicleDriver> VehicleDrivers { get; set; } = new List<VehicleDriver>();
    public ICollection<Trip> Trips { get; set; } = new List<Trip>();
}

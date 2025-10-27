using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AmbulanceRider.API.Models;

public class Location : BaseModel
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    public double Latitude { get; set; }
    
    [Required]
    public double Longitude { get; set; }
    
    public string? ImagePath { get; set; }
    
    public string? ImageUrl { get; set; }
    
    // Navigation properties
    public virtual ICollection<Route> RoutesFrom { get; set; } = new List<Route>();
    public virtual ICollection<Route> RoutesTo { get; set; } = new List<Route>();
}

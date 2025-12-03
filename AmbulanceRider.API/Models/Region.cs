using System.ComponentModel.DataAnnotations;

namespace AmbulanceRider.API.Models;

public class Region : BaseModel
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    [Required]
    [StringLength(10)]
    public string Code { get; set; } = string.Empty;
    
    public bool IsDefault { get; set; } = false;
    
    public bool IsActive { get; set; } = true;
}

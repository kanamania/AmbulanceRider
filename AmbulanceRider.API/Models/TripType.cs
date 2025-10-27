using System.ComponentModel.DataAnnotations;

namespace AmbulanceRider.API.Models;

/// <summary>
/// Defines a type of trip with custom attributes (e.g., Emergency, Routine, Transfer)
/// </summary>
public class TripType : BaseModel
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    [StringLength(50)]
    public string? Color { get; set; } // For UI display (e.g., "#FF5733")
    
    [StringLength(50)]
    public string? Icon { get; set; } // Icon name for UI
    
    public bool IsActive { get; set; } = true;
    
    public int DisplayOrder { get; set; } = 0;
    
    // Navigation properties
    public virtual ICollection<TripTypeAttribute> Attributes { get; set; } = new List<TripTypeAttribute>();
    public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();
}

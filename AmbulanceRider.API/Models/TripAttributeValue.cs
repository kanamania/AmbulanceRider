using System.ComponentModel.DataAnnotations;

namespace AmbulanceRider.API.Models;

/// <summary>
/// Stores the actual value of a custom attribute for a specific trip
/// </summary>
public class TripAttributeValue : BaseModel
{
    [Required]
    public int TripId { get; set; }
    
    [Required]
    public int TripTypeAttributeId { get; set; }
    
    [StringLength(2000)]
    public string? Value { get; set; }
    
    // Navigation properties
    public virtual Trip? Trip { get; set; }
    public virtual TripTypeAttribute? TripTypeAttribute { get; set; }
}

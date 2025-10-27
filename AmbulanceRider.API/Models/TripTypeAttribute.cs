using System.ComponentModel.DataAnnotations;

namespace AmbulanceRider.API.Models;

/// <summary>
/// Defines a custom attribute/field for a TripType
/// </summary>
public class TripTypeAttribute : BaseModel
{
    [Required]
    public int TripTypeId { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string Label { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    [Required]
    [StringLength(50)]
    public string DataType { get; set; } = "text"; // text, number, date, boolean, select, textarea
    
    public bool IsRequired { get; set; } = false;
    
    public int DisplayOrder { get; set; } = 0;
    
    [StringLength(2000)]
    public string? Options { get; set; } // JSON array for select options: ["Option1", "Option2"]
    
    [StringLength(500)]
    public string? DefaultValue { get; set; }
    
    [StringLength(500)]
    public string? ValidationRules { get; set; } // JSON object for validation rules
    
    [StringLength(200)]
    public string? Placeholder { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public virtual TripType? TripType { get; set; }
    public virtual ICollection<TripAttributeValue> Values { get; set; } = new List<TripAttributeValue>();
}

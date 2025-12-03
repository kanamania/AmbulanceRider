using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AmbulanceRider.API.Models;

public class PricingMatrix : BaseModel
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    // Dimensions
    public decimal MinWeight { get; set; }
    public decimal MaxWeight { get; set; }
    public decimal MinHeight { get; set; }
    public decimal MaxHeight { get; set; }
    public decimal MinLength { get; set; }
    public decimal MaxLength { get; set; }
    public decimal MinWidth { get; set; }
    public decimal MaxWidth { get; set; }
    
    // Pricing
    public decimal BasePrice { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TotalPrice => BasePrice * (1 + TaxRate);
    
    // Region
    public int? RegionId { get; set; }
    public virtual Region? Region { get; set; }
    
    public bool IsDefault { get; set; } = false;
    
    // Relationships
    public int? CompanyId { get; set; }
    public virtual Company? Company { get; set; }
    
    public int? VehicleTypeId { get; set; }
    public virtual VehicleType? VehicleType { get; set; }
    
    public int? TripTypeId { get; set; }
    public virtual TripType? TripType { get; set; }
}

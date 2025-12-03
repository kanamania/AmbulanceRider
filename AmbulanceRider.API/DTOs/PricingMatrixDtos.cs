namespace AmbulanceRider.API.DTOs;

public class CreatePricingMatrixDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    public decimal MinWeight { get; set; }
    public decimal MaxWeight { get; set; }
    public decimal MinHeight { get; set; }
    public decimal MaxHeight { get; set; }
    public decimal MinLength { get; set; }
    public decimal MaxLength { get; set; }
    public decimal MinWidth { get; set; }
    public decimal MaxWidth { get; set; }
    
    public decimal BasePrice { get; set; }
    public decimal TaxRate { get; set; }
    
    public int? RegionId { get; set; }
    public bool IsDefault { get; set; } = false;
    
    public int? CompanyId { get; set; }
    public int? VehicleTypeId { get; set; }
    public int? TripTypeId { get; set; }
}

public class UpdatePricingMatrixDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    
    public decimal? MinWeight { get; set; }
    public decimal? MaxWeight { get; set; }
    public decimal? MinHeight { get; set; }
    public decimal? MaxHeight { get; set; }
    public decimal? MinLength { get; set; }
    public decimal? MaxLength { get; set; }
    public decimal? MinWidth { get; set; }
    public decimal? MaxWidth { get; set; }
    
    public decimal? BasePrice { get; set; }
    public decimal? TaxRate { get; set; }
    
    public int? RegionId { get; set; }
    
    public int? CompanyId { get; set; }
    public int? VehicleTypeId { get; set; }
    public int? TripTypeId { get; set; }
}

namespace AmbulanceRider.API.DTOs;

public class TripTypeAttributeDto
{
    public int Id { get; set; }
    public int TripTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string DataType { get; set; } = "text";
    public bool IsRequired { get; set; }
    public int DisplayOrder { get; set; }
    public string? Options { get; set; }
    public string? DefaultValue { get; set; }
    public string? ValidationRules { get; set; }
    public string? Placeholder { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateTripTypeAttributeDto
{
    public int TripTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string DataType { get; set; } = "text";
    public bool IsRequired { get; set; } = false;
    public int DisplayOrder { get; set; } = 0;
    public string? Options { get; set; }
    public string? DefaultValue { get; set; }
    public string? ValidationRules { get; set; }
    public string? Placeholder { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UpdateTripTypeAttributeDto
{
    public string? Name { get; set; }
    public string? Label { get; set; }
    public string? Description { get; set; }
    public string? DataType { get; set; }
    public bool? IsRequired { get; set; }
    public int? DisplayOrder { get; set; }
    public string? Options { get; set; }
    public string? DefaultValue { get; set; }
    public string? ValidationRules { get; set; }
    public string? Placeholder { get; set; }
    public bool? IsActive { get; set; }
}

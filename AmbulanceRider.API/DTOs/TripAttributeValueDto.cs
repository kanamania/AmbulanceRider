namespace AmbulanceRider.API.DTOs;

public class TripAttributeValueDto
{
    public int Id { get; set; }
    public int TripId { get; set; }
    public int TripTypeAttributeId { get; set; }
    public string? Value { get; set; }
    
    // For convenience in UI
    public string? AttributeName { get; set; }
    public string? AttributeLabel { get; set; }
    public string? AttributeDataType { get; set; }
}

public class CreateTripAttributeValueDto
{
    public int TripTypeAttributeId { get; set; }
    public string? Value { get; set; }
}

public class UpdateTripAttributeValueDto
{
    public int TripTypeAttributeId { get; set; }
    public string? Value { get; set; }
}

using AmbulanceRider.API.DTOs;

namespace AmbulanceRider.API.Services;

public interface ITripTypeService
{
    Task<IEnumerable<TripTypeDto>> GetAllTripTypesAsync();
    Task<IEnumerable<TripTypeDto>> GetActiveTripTypesAsync();
    Task<TripTypeDto?> GetTripTypeByIdAsync(int id);
    Task<TripTypeDto> CreateTripTypeAsync(CreateTripTypeDto dto);
    Task<TripTypeDto> UpdateTripTypeAsync(int id, UpdateTripTypeDto dto);
    Task DeleteTripTypeAsync(int id);
    
    // Attributes
    Task<TripTypeAttributeDto> CreateAttributeAsync(CreateTripTypeAttributeDto dto);
    Task<TripTypeAttributeDto> UpdateAttributeAsync(int id, UpdateTripTypeAttributeDto dto);
    Task DeleteAttributeAsync(int id);
}

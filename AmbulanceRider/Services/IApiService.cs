using AmbulanceRider.Models;

namespace AmbulanceRider.Services;

public interface IApiService
{
    // Generic HTTP helpers
    Task<T?> GetAsync<T>(string endpoint);
    Task<T> PostAsync<T>(string endpoint, object data);
    Task<T> PutAsync<T>(string endpoint, object data);
    Task DeleteAsync(string endpoint);

    // Users
    Task<List<UserDto>> GetUsersAsync();
    Task<UserDto?> GetUserByIdAsync(string id);
    Task<UserDto> CreateUserAsync(CreateUserDto user);
    Task<UserDto> UpdateUserAsync(string id, UpdateUserDto user);
    Task DeleteUserAsync(string id);

    // Vehicles
    Task<List<VehicleDto>> GetVehiclesAsync();
    Task<List<VehicleTypeDto>> GetVehicleTypesAsync();
    Task<VehicleDto?> GetVehicleByIdAsync(int id);
    Task<VehicleDto> CreateVehicleAsync(CreateVehicleDto vehicle);
    Task<VehicleDto> UpdateVehicleAsync(int id, UpdateVehicleDto vehicle);
    Task DeleteVehicleAsync(int id);

    // Locations
    Task<List<LocationDto>> GetLocationsAsync();
    Task<LocationDto?> GetLocationByIdAsync(int id);
    Task<LocationDto> CreateLocationAsync(CreateLocationDto location);
    Task<LocationDto> UpdateLocationAsync(int id, UpdateLocationDto location);
    Task DeleteLocationAsync(int id);

    // Trips
    Task<List<TripDto>> GetTripsAsync();
    Task<TripDto?> GetTripByIdAsync(int id);
    Task<TripDto> CreateTripAsync(CreateTripDto trip);
    Task<TripDto> UpdateTripAsync(int id, UpdateTripDto trip);
    Task DeleteTripAsync(int id);
    Task<TripDto> ApproveTripAsync(int id, ApproveTripDto approveDto);
    Task<TripDto> StartTripAsync(int id, StartTripDto startDto);
    Task<TripDto> CompleteTripAsync(int id, CompleteTripDto completeDto);
    Task<TripDto> UpdateTripStatusAsync(int id, UpdateTripStatusDto updateDto);
    Task<List<TripStatusLogDto>> GetTripStatusLogsAsync(int tripId);

    // Trip types
    Task<List<TripTypeDto>> GetTripTypesAsync();
    Task<List<TripTypeDto>> GetActiveTripTypesAsync();
    Task<TripTypeDto?> GetTripTypeByIdAsync(int id);
    Task<TripTypeDto> CreateTripTypeAsync(CreateTripTypeDto tripType);
    Task<TripTypeDto> UpdateTripTypeAsync(int id, UpdateTripTypeDto tripType);
    Task DeleteTripTypeAsync(int id);

    // Trip type attributes
    Task<TripTypeAttributeDto> CreateTripTypeAttributeAsync(CreateTripTypeAttributeDto attribute);
    Task<TripTypeAttributeDto> UpdateTripTypeAttributeAsync(int id, UpdateTripTypeAttributeDto attribute);
    Task DeleteTripTypeAttributeAsync(int id);

    // Dashboard & analytics
    Task<CompanyStatsDto?> GetCompanyStatsAsync(int companyId);
    Task<List<CompanyDto>?> GetCompaniesAsync();

    // Invoices
    Task<List<InvoiceDto>> GetInvoicesAsync(InvoiceFilterDto filter);
    Task<InvoiceDto?> GetInvoiceByIdAsync(int id);
    Task<InvoicePreviewDto> PreviewInvoiceAsync(CreateInvoiceDto dto);
    Task<InvoiceDto> CreateInvoiceAsync(CreateInvoiceDto dto);
    Task<InvoiceDto> MarkInvoiceAsPaidAsync(int id, MarkInvoicePaidDto dto);
}

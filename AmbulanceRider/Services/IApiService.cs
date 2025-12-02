using AmbulanceRider.Models;

namespace AmbulanceRider.Services;

public interface IApiService
{
    Task<CompanyStatsDto?> GetCompanyStatsAsync(int companyId);
    // Add other existing methods from ApiService
    Task<List<TripDto>> GetTripsAsync();
    Task<List<VehicleDto>> GetVehiclesAsync();
    Task<List<UserDto>> GetUsersAsync();
    Task<List<LocationDto>> GetLocationsAsync();
    Task<List<TripTypeDto>> GetTripTypesAsync();
    Task<List<CompanyDto>?> GetCompaniesAsync();

    // Invoice methods
    Task<List<InvoiceDto>> GetInvoicesAsync(InvoiceFilterDto filter);
    Task<InvoiceDto?> GetInvoiceByIdAsync(int id);
    Task<InvoicePreviewDto> PreviewInvoiceAsync(CreateInvoiceDto dto);
    Task<InvoiceDto> CreateInvoiceAsync(CreateInvoiceDto dto);
    Task<InvoiceDto> MarkInvoiceAsPaidAsync(int id, MarkInvoicePaidDto dto);
}

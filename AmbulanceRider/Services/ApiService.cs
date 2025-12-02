using System.Net;
using System.Net.Http.Json;
using AmbulanceRider.Models;
using Microsoft.AspNetCore.Components;

namespace AmbulanceRider.Services;

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;
    private readonly NavigationManager _navigationManager;

    public ApiService(HttpClient httpClient, IConfiguration configuration, AuthService authService, NavigationManager navigationManager)
    {
        _httpClient = httpClient;
        _authService = authService;
        _navigationManager = navigationManager;
        // Don't set BaseAddress here - it's already set in Program.cs
        // Just use the configured client
    }

    private async Task<T> ExecuteAsync<T>(Func<Task<T>> action)
    {
        try
        {
            return await action();
        }
        catch (HttpRequestException ex)
        {
            await HandleHttpRequestExceptionAsync(ex);
            throw;
        }
    }

    private async Task ExecuteAsync(Func<Task> action)
    {
        try
        {
            await action();
        }
        catch (HttpRequestException ex)
        {
            await HandleHttpRequestExceptionAsync(ex);
            throw;
        }
    }

    private async Task HandleHttpRequestExceptionAsync(HttpRequestException ex)
    {
        if (ex.StatusCode == HttpStatusCode.Unauthorized)
        {
            await _authService.LogoutAsync();
            _navigationManager.NavigateTo("/login", true);
        }
    }

    // Generic HTTP methods
    public async Task<T?> GetAsync<T>(string endpoint)
    {
        return await ExecuteAsync(async () =>
            await _httpClient.GetFromJsonAsync<T>($"/api/{endpoint.TrimStart('/')}")
        );
    }

    public async Task<T> PostAsync<T>(string endpoint, object data)
    {
        return await ExecuteAsync(async () =>
        {
            var response = await _httpClient.PostAsJsonAsync($"/api/{endpoint.TrimStart('/')}", data);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>() ?? throw new Exception("Failed to post data");
        });
    }

    public async Task<T> PutAsync<T>(string endpoint, object data)
    {
        return await ExecuteAsync(async () =>
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/{endpoint.TrimStart('/')}", data);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>() ?? throw new Exception("Failed to put data");
        });
    }

    public async Task DeleteAsync(string endpoint)
    {
        await ExecuteAsync(async () =>
        {
            var response = await _httpClient.DeleteAsync($"/api/{endpoint.TrimStart('/')}");
            response.EnsureSuccessStatusCode();
        });
    }

    // Users
    public async Task<List<UserDto>> GetUsersAsync()
    {
        return await ExecuteAsync(async () =>
            await _httpClient.GetFromJsonAsync<List<UserDto>>("/api/users") ?? new List<UserDto>()
        );
    }

    public async Task<UserDto?> GetUserByIdAsync(string id)
    {
        return await _httpClient.GetFromJsonAsync<UserDto>($"/api/users/{id}");
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto user)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/users", user);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UserDto>() ?? throw new Exception("Failed to create user");
    }

    public async Task<UserDto> UpdateUserAsync(string id, UpdateUserDto user)
    {
        Console.WriteLine($"[API] Starting user update for ID: {id}");
        Console.WriteLine($"[API] Roles being submitted: {string.Join(", ", user.Roles ?? new List<string>())}");
        
        var response = await _httpClient.PutAsJsonAsync($"/api/users/{id}", user);
        response.EnsureSuccessStatusCode();
        
        var result = await response.Content.ReadFromJsonAsync<UserDto>() ?? throw new Exception("Failed to update user");
        Console.WriteLine($"[API] Update successful. New roles: {string.Join(", ", result.Roles)}");
        
        return result;
    }

    public async Task DeleteUserAsync(string id)
    {
        var response = await _httpClient.DeleteAsync($"/api/users/{id}");
        response.EnsureSuccessStatusCode();
    }

    // Vehicles
    public async Task<List<VehicleDto>> GetVehiclesAsync()
    {
        return await ExecuteAsync(async () =>
            await _httpClient.GetFromJsonAsync<List<VehicleDto>>("/api/vehicles") ?? new List<VehicleDto>()
        );
    }

    public async Task<List<VehicleTypeDto>> GetVehicleTypesAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<VehicleTypeDto>>("/api/vehicles/types") ?? new List<VehicleTypeDto>();
    }

    public async Task<VehicleDto?> GetVehicleByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<VehicleDto>($"/api/vehicles/{id}");
    }

    public async Task<VehicleDto> CreateVehicleAsync(CreateVehicleDto vehicle)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/vehicles", vehicle);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<VehicleDto>() ?? throw new Exception("Failed to create vehicle");
    }

    public async Task<VehicleDto> UpdateVehicleAsync(int id, UpdateVehicleDto vehicle)
    {
        var response = await _httpClient.PutAsJsonAsync($"/api/vehicles/{id}", vehicle);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<VehicleDto>() ?? throw new Exception("Failed to update vehicle");
    }

    public async Task DeleteVehicleAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"/api/vehicles/{id}");
        response.EnsureSuccessStatusCode();
    }

    // Locations
    public async Task<List<LocationDto>> GetLocationsAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<LocationDto>>("/api/locations") ?? new List<LocationDto>();
    }

    public async Task<LocationDto?> GetLocationByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<LocationDto>($"/api/locations/{id}");
    }

    public async Task<LocationDto> CreateLocationAsync(CreateLocationDto location)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/locations", location);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<LocationDto>() ?? throw new Exception("Failed to create location");
    }

    public async Task<LocationDto> UpdateLocationAsync(int id, UpdateLocationDto location)
    {
        var response = await _httpClient.PutAsJsonAsync($"/api/locations/{id}", location);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<LocationDto>() ?? throw new Exception("Failed to update location");
    }

    public async Task DeleteLocationAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"/api/locations/{id}");
        response.EnsureSuccessStatusCode();
    }

    // Trips
    public async Task<List<TripDto>> GetTripsAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<TripDto>>("/api/trips") ?? new List<TripDto>();
    }

    public async Task<TripDto?> GetTripByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<TripDto>($"/api/trips/{id}");
    }

    public async Task<TripDto> CreateTripAsync(CreateTripDto trip)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/trips", trip);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TripDto>() ?? throw new Exception("Failed to create trip");
    }

    public async Task<TripDto> UpdateTripAsync(int id, UpdateTripDto trip)
    {
        var response = await _httpClient.PutAsJsonAsync($"/api/trips/{id}", trip);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TripDto>() ?? throw new Exception("Failed to update trip");
    }

    public async Task DeleteTripAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"/api/trips/{id}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<TripDto> ApproveTripAsync(int id, ApproveTripDto approveDto)
    {
        var response = await _httpClient.PostAsJsonAsync($"/api/trips/{id}/approve", approveDto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TripDto>() ?? throw new Exception("Failed to approve trip");
    }

    public async Task<TripDto> StartTripAsync(int id, StartTripDto startDto)
    {
        var response = await _httpClient.PostAsJsonAsync($"/api/trips/{id}/start", startDto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TripDto>() ?? throw new Exception("Failed to start trip");
    }

    public async Task<TripDto> CompleteTripAsync(int id, CompleteTripDto completeDto)
    {
        var response = await _httpClient.PostAsJsonAsync($"/api/trips/{id}/complete", completeDto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TripDto>() ?? throw new Exception("Failed to complete trip");
    }

    public async Task<TripDto> UpdateTripStatusAsync(int id, UpdateTripStatusDto updateDto)
    {
        var response = await _httpClient.PutAsJsonAsync($"/api/trips/{id}/status", updateDto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TripDto>() ?? throw new Exception("Failed to update trip status");
    }

    public async Task<List<TripStatusLogDto>> GetTripStatusLogsAsync(int tripId)
    {
        return await _httpClient.GetFromJsonAsync<List<TripStatusLogDto>>($"/api/trips/{tripId}/status-logs") ?? new List<TripStatusLogDto>();
    }

    // Trip Types
    public async Task<List<TripTypeDto>> GetTripTypesAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<TripTypeDto>>("/api/triptypes") ?? new List<TripTypeDto>();
    }

    public async Task<List<TripTypeDto>> GetActiveTripTypesAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<TripTypeDto>>("/api/triptypes/active") ?? new List<TripTypeDto>();
    }

    public async Task<TripTypeDto?> GetTripTypeByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<TripTypeDto>($"/api/triptypes/{id}");
    }

    public async Task<TripTypeDto> CreateTripTypeAsync(CreateTripTypeDto tripType)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/triptypes", tripType);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TripTypeDto>() ?? throw new Exception("Failed to create trip type");
    }

    public async Task<TripTypeDto> UpdateTripTypeAsync(int id, UpdateTripTypeDto tripType)
    {
        var response = await _httpClient.PutAsJsonAsync($"/api/triptypes/{id}", tripType);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TripTypeDto>() ?? throw new Exception("Failed to update trip type");
    }

    public async Task DeleteTripTypeAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"/api/triptypes/{id}");
        response.EnsureSuccessStatusCode();
    }

    // Trip Type Attributes
    public async Task<TripTypeAttributeDto> CreateTripTypeAttributeAsync(CreateTripTypeAttributeDto attribute)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/triptypes/attributes", attribute);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TripTypeAttributeDto>() ?? throw new Exception("Failed to create attribute");
    }

    public async Task<TripTypeAttributeDto> UpdateTripTypeAttributeAsync(int id, UpdateTripTypeAttributeDto attribute)
    {
        var response = await _httpClient.PutAsJsonAsync($"/api/triptypes/attributes/{id}", attribute);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TripTypeAttributeDto>() ?? throw new Exception("Failed to update attribute");
    }

    public async Task DeleteTripTypeAttributeAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"/api/triptypes/attributes/{id}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<CompanyStatsDto?> GetCompanyStatsAsync(int companyId)
    {
        return await _httpClient.GetFromJsonAsync<CompanyStatsDto>($"api/dashboard/company-stats/{companyId}");
    }

    public async Task<List<CompanyDto>?> GetCompaniesAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<CompanyDto>?>($"api/companies");
    }

    public async Task<CompanyDto?> GetCompanyByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<CompanyDto>($"api/companies/{id}");
    }

    public async Task<CompanyDto> CreateCompanyAsync(CreateCompanyDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/companies", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CompanyDto>() ?? throw new InvalidOperationException("Failed to create company");
    }

    public async Task<CompanyDto> UpdateCompanyAsync(int id, UpdateCompanyDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/companies/{id}", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CompanyDto>() ?? throw new InvalidOperationException("Failed to update company");
    }

    public async Task DeleteCompanyAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/companies/{id}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<InvoiceDto>> GetInvoicesAsync(InvoiceFilterDto filter)
    {
        var queryParams = new List<string>();
        if (filter.CompanyId.HasValue)
            queryParams.Add($"CompanyId={filter.CompanyId}");
        if (!string.IsNullOrEmpty(filter.Type))
            queryParams.Add($"Type={filter.Type}");
        if (!string.IsNullOrEmpty(filter.Status))
            queryParams.Add($"Status={filter.Status}");
        if (filter.StartDate.HasValue)
            queryParams.Add($"StartDate={filter.StartDate.Value:yyyy-MM-dd}");
        if (filter.EndDate.HasValue)
            queryParams.Add($"EndDate={filter.EndDate.Value:yyyy-MM-dd}");

        var query = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
        return await _httpClient.GetFromJsonAsync<List<InvoiceDto>>($"/api/invoice{query}") ?? new List<InvoiceDto>();
    }

    public async Task<InvoiceDto?> GetInvoiceByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<InvoiceDto>($"/api/invoice/{id}");
    }

    public async Task<InvoicePreviewDto> PreviewInvoiceAsync(CreateInvoiceDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/invoice/preview", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<InvoicePreviewDto>() ?? throw new Exception("Failed to preview invoice");
    }

    public async Task<InvoiceDto> CreateInvoiceAsync(CreateInvoiceDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/invoice", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<InvoiceDto>() ?? throw new Exception("Failed to create invoice");
    }

    public async Task<InvoiceDto> MarkInvoiceAsPaidAsync(int id, MarkInvoicePaidDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync($"/api/invoice/{id}/mark-paid", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<InvoiceDto>() ?? throw new Exception("Failed to mark invoice as paid");
    }
}

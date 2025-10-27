using System.Net.Http.Json;
using AmbulanceRider.Models;

namespace AmbulanceRider.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        // Don't set BaseAddress here - it's already set in Program.cs
        // Just use the configured client
    }

    // Users
    public async Task<List<UserDto>> GetUsersAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<UserDto>>("/api/users") ?? new List<UserDto>();
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
        var response = await _httpClient.PutAsJsonAsync($"/api/users/{id}", user);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UserDto>() ?? throw new Exception("Failed to update user");
    }

    public async Task DeleteUserAsync(string id)
    {
        var response = await _httpClient.DeleteAsync($"/api/users/{id}");
        response.EnsureSuccessStatusCode();
    }

    // Vehicles
    public async Task<List<VehicleDto>> GetVehiclesAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<VehicleDto>>("/api/vehicles") ?? new List<VehicleDto>();
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
}

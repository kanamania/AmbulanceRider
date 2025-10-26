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

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<UserDto>($"/api/users/{id}");
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto user)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/users", user);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UserDto>() ?? throw new Exception("Failed to create user");
    }

    public async Task<UserDto> UpdateUserAsync(int id, UpdateUserDto user)
    {
        var response = await _httpClient.PutAsJsonAsync($"/api/users/{id}", user);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UserDto>() ?? throw new Exception("Failed to update user");
    }

    public async Task DeleteUserAsync(int id)
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

    // Routes
    public async Task<List<RouteDto>> GetRoutesAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<RouteDto>>("/api/routes") ?? new List<RouteDto>();
    }

    public async Task<RouteDto?> GetRouteByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<RouteDto>($"/api/routes/{id}");
    }

    public async Task<RouteDto> CreateRouteAsync(CreateRouteDto route)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/routes", route);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<RouteDto>() ?? throw new Exception("Failed to create route");
    }

    public async Task<RouteDto> UpdateRouteAsync(int id, UpdateRouteDto route)
    {
        var response = await _httpClient.PutAsJsonAsync($"/api/routes/{id}", route);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<RouteDto>() ?? throw new Exception("Failed to update route");
    }

    public async Task DeleteRouteAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"/api/routes/{id}");
        response.EnsureSuccessStatusCode();
    }
}

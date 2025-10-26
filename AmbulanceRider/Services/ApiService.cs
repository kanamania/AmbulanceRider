using System.Net.Http.Json;
using AmbulanceRider.Models;

namespace AmbulanceRider.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly string BaseUrl;

    public ApiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        // In Docker, the API service is available at http://api:8080
        // In development, it's at http://localhost:5000
        BaseUrl = configuration["ApiBaseUrl"] ?? "http://localhost:5000/api";
        _httpClient.BaseAddress = new Uri(BaseUrl);
    }

    // Users
    public async Task<List<UserDto>> GetUsersAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<UserDto>>("users") ?? new List<UserDto>();
    }

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<UserDto>($"users/{id}");
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto user)
    {
        var response = await _httpClient.PostAsJsonAsync("users", user);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UserDto>() ?? throw new Exception("Failed to create user");
    }

    public async Task<UserDto> UpdateUserAsync(int id, UpdateUserDto user)
    {
        var response = await _httpClient.PutAsJsonAsync($"users/{id}", user);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UserDto>() ?? throw new Exception("Failed to update user");
    }

    public async Task DeleteUserAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"users/{id}");
        response.EnsureSuccessStatusCode();
    }

    // Vehicles
    public async Task<List<VehicleDto>> GetVehiclesAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<VehicleDto>>("vehicles") ?? new List<VehicleDto>();
    }

    public async Task<VehicleDto?> GetVehicleByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<VehicleDto>($"vehicles/{id}");
    }

    public async Task<VehicleDto> CreateVehicleAsync(CreateVehicleDto vehicle)
    {
        var response = await _httpClient.PostAsJsonAsync("vehicles", vehicle);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<VehicleDto>() ?? throw new Exception("Failed to create vehicle");
    }

    public async Task<VehicleDto> UpdateVehicleAsync(int id, UpdateVehicleDto vehicle)
    {
        var response = await _httpClient.PutAsJsonAsync($"vehicles/{id}", vehicle);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<VehicleDto>() ?? throw new Exception("Failed to update vehicle");
    }

    public async Task DeleteVehicleAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"vehicles/{id}");
        response.EnsureSuccessStatusCode();
    }

    // Routes
    public async Task<List<RouteDto>> GetRoutesAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<RouteDto>>("routes") ?? new List<RouteDto>();
    }

    public async Task<RouteDto?> GetRouteByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<RouteDto>($"routes/{id}");
    }

    public async Task<RouteDto> CreateRouteAsync(CreateRouteDto route)
    {
        var response = await _httpClient.PostAsJsonAsync("routes", route);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<RouteDto>() ?? throw new Exception("Failed to create route");
    }

    public async Task<RouteDto> UpdateRouteAsync(int id, UpdateRouteDto route)
    {
        var response = await _httpClient.PutAsJsonAsync($"routes/{id}", route);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<RouteDto>() ?? throw new Exception("Failed to update route");
    }

    public async Task DeleteRouteAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"routes/{id}");
        response.EnsureSuccessStatusCode();
    }
}

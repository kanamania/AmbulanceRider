using System.Globalization;
using System.Text.Json;

namespace AmbulanceRider.API.Services;

public class GeocodingService : IGeocodingService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GeocodingService> _logger;

    public GeocodingService(HttpClient httpClient, ILogger<GeocodingService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        
        if (_httpClient.BaseAddress == null)
        {
            _httpClient.BaseAddress = new Uri("https://nominatim.openstreetmap.org/");
        }
        
        if (!_httpClient.DefaultRequestHeaders.Contains("User-Agent"))
        {
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "AmbulanceRider/1.0");
        }
    }

    public async Task<string?> GetRegionFromCoordinatesAsync(double latitude, double longitude)
    {
        try
        {
            var lat = latitude.ToString(CultureInfo.InvariantCulture);
            var lon = longitude.ToString(CultureInfo.InvariantCulture);
            
            var url = $"reverse?format=json&lat={lat}&lon={lon}&zoom=10&addressdetails=1";
            
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;
            
            if (root.TryGetProperty("address", out var address))
            {
                if (address.TryGetProperty("state", out var state))
                {
                    return state.GetString();
                }
                
                if (address.TryGetProperty("region", out var region))
                {
                    return region.GetString();
                }
                
                if (address.TryGetProperty("county", out var county))
                {
                    return county.GetString();
                }
                
                if (address.TryGetProperty("city", out var city))
                {
                    return city.GetString();
                }
            }
            
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting region from coordinates ({Lat}, {Lon})", latitude, longitude);
            return null;
        }
    }
}

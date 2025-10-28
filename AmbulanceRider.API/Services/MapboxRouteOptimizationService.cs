using System.Text.Json;
using System.Text.Json.Nodes;
using AmbulanceRider.API.DTOs;
using Microsoft.Extensions.Options;

namespace AmbulanceRider.API.Services;

public class MapboxRouteOptimizationService : IRouteOptimizationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MapboxRouteOptimizationService> _logger;
    private readonly MapboxSettings _settings;
    private readonly JsonSerializerOptions _jsonOptions;

    public MapboxRouteOptimizationService(
        HttpClient httpClient, 
        IOptions<MapboxSettings> settings,
        ILogger<MapboxRouteOptimizationService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _settings = settings.Value;
        
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
        
        _httpClient.BaseAddress = new Uri("https://api.mapbox.com/");
    }

    public async Task<OptimizedRouteDto> GetOptimizedRouteAsync(RouteOptimizationRequestDto request)
    {
        try
        {
            // Convert waypoints to Mapbox format
            var coordinates = request.Waypoints
                .OrderBy(w => w.Sequence ?? 0)
                .Select(w => $"{w.Longitude},{w.Latitude}")
                .ToArray();

            var waypointsParam = string.Join(";", coordinates);
            
            // Build the URL for the Mapbox Directions API
            var url = $"directions/v5/mapbox/driving/{waypointsParam}" +
                     $"?geometries=polyline" +
                     $"&steps=true" +
                     $"&overview=full" +
                     $"&access_token={_settings.AccessToken}";
            
            // Add optimization parameters
            if (request.OptimizeForTime)
            {
                url += "&overview=full&annotations=duration,distance";
            }
            
            if (request.AvoidTolls)
            {
                // Mapbox doesn't directly support avoiding tolls in the free tier
                // This would require additional handling or a different service
            }
            
            if (request.AvoidHighways)
            {
                url += "&exclude=highway";
            }
            
            // Make the API request
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            var json = JsonNode.Parse(content);
            
            // Parse the response into our DTO
            return ParseMapboxResponse(json, request.Waypoints);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting optimized route from Mapbox");
            throw new ApplicationException("Failed to get optimized route", ex);
        }
    }

    public async Task<Dictionary<int, OptimizedRouteDto>> BatchOptimizeRoutesAsync(List<RouteOptimizationRequestDto> requests)
    {
        var results = new Dictionary<int, OptimizedRouteDto>();
        
        // Process each request in parallel
        var tasks = requests.Select(async (request, index) =>
        {
            try
            {
                var result = await GetOptimizedRouteAsync(request);
                results[index] = result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing route optimization request {index}");
                // Optionally handle partial failures
            }
        });
        
        await Task.WhenAll(tasks);
        return results;
    }

    public async Task<Dictionary<string, object>> GetDistanceMatrixAsync(List<RouteWaypointDto> origins, List<RouteWaypointDto> destinations)
    {
        try
        {
            var originCoords = string.Join(";", origins.Select(p => $"{p.Longitude},{p.Latitude}"));
            var destCoords = string.Join(";", destinations.Select(p => $"{p.Longitude},{p.Latitude}"));
            
            var url = $"directions-matrix/v1/mapbox/driving/{originCoords};{destCoords}" +
                     $"?sources=0&destinations={origins.Count}" +
                     $"&access_token={_settings.AccessToken}";
            
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Dictionary<string, object>>(content) ?? new Dictionary<string, object>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting distance matrix from Mapbox");
            throw new ApplicationException("Failed to get distance matrix", ex);
        }
    }

    public Task<string> GetStaticMapUrlAsync(string polyline, int width = 600, int height = 400)
    {
        var encodedPolyline = Uri.EscapeDataString(polyline);
        var url = $"styles/v1/mapbox/streets-v11/static/path-5+f44-0.5({encodedPolyline})/auto/{width}x{height}" +
                 $"?access_token={_settings.AccessToken}";
        
        return Task.FromResult(url);
    }

    public async Task<byte[]> GetStaticMapImageAsync(string polyline, int width = 600, int height = 400)
    {
        var url = await GetStaticMapUrlAsync(polyline, width, height);
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadAsByteArrayAsync();
    }

    private OptimizedRouteDto ParseMapboxResponse(JsonNode? json, List<RouteWaypointDto> waypoints)
    {
        if (json == null) 
            throw new ArgumentNullException(nameof(json));
        
        var route = json["routes"]?[0];
        if (route == null)
            throw new InvalidOperationException("No route found in response");
        
        var legs = new List<RouteLegDto>();
        var routeLegs = route["legs"]?.AsArray() ?? new JsonArray();
        
        foreach (var leg in routeLegs)
        {
            if (leg == null) continue;
            
            var steps = leg["steps"]?.AsArray();
            var lastStepIndex = (steps?.Count ?? 1) - 1;
            
            legs.Add(new RouteLegDto
            {
                StartLat = steps?[0]?["maneuver"]?["location"]?[1]?.GetValue<double>() ?? 0,
                StartLng = steps?[0]?["maneuver"]?["location"]?[0]?.GetValue<double>() ?? 0,
                EndLat = steps?[lastStepIndex]?["maneuver"]?["location"]?[1]?.GetValue<double>() ?? 0,
                EndLng = steps?[lastStepIndex]?["maneuver"]?["location"]?[0]?.GetValue<double>() ?? 0,
                DistanceMeters = leg["distance"]?.GetValue<double>() ?? 0,
                DurationSeconds = (int)(leg["duration"]?.GetValue<double>() ?? 0),
                Instructions = steps?[0]?["maneuver"]?["instruction"]?.GetValue<string>(),
                Maneuver = steps?[0]?["maneuver"]?["type"]?.GetValue<string>()
            });
        }
        
        return new OptimizedRouteDto
        {
            Polyline = route["geometry"]?["coordinates"]?.ToJsonString(),
            DistanceMeters = route["distance"]?.GetValue<double>() ?? 0,
            DurationSeconds = (int)(route["duration"]?.GetValue<double>() ?? 0),
            Legs = legs,
            Metadata = new Dictionary<string, object>
            {
                ["waypoints"] = waypoints,
                ["raw_response"] = json.ToJsonString()
            }
        };
    }
}

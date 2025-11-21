using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using AmbulanceRider.API.DTOs;
using Microsoft.Extensions.Logging;

namespace AmbulanceRider.API.Services;

public class OsrmRouteOptimizationService : IRouteOptimizationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OsrmRouteOptimizationService> _logger;

    public OsrmRouteOptimizationService(HttpClient httpClient, ILogger<OsrmRouteOptimizationService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        if (_httpClient.BaseAddress == null)
        {
            _httpClient.BaseAddress = new Uri("https://router.project-osrm.org/");
        }
    }

    public async Task<OptimizedRouteDto> GetOptimizedRouteAsync(RouteOptimizationRequestDto request)
    {
        try
        {
            var ordered = request.Waypoints.OrderBy(w => w.Sequence ?? 0).ToList();
            var coords = string.Join(";", ordered.Select(w =>
                $"{w.Longitude.ToString(CultureInfo.InvariantCulture)},{w.Latitude.ToString(CultureInfo.InvariantCulture)}"));

            var url = $"route/v1/driving/{coords}?alternatives=true&overview=full&geometries=polyline&steps=false&annotations=distance,duration";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var json = JsonNode.Parse(content);
            var routes = json?["routes"]?.AsArray();
            if (routes == null || routes.Count == 0)
            {
                throw new InvalidOperationException("No route found in OSRM response");
            }

            // Pick the shortest distance route
            JsonNode? bestRoute = routes
                .OrderBy(r => r?["distance"]?.GetValue<double>() ?? double.MaxValue)
                .FirstOrDefault();

            var polyline = bestRoute?["geometry"]?.GetValue<string>() ?? string.Empty;
            var distance = bestRoute?["distance"]?.GetValue<double>() ?? 0d;
            var duration = (int)Math.Round(bestRoute?["duration"]?.GetValue<double>() ?? 0d);

            var first = ordered.FirstOrDefault();
            var last = ordered.LastOrDefault();
            var legs = new List<RouteLegDto>();
            if (first != null && last != null)
            {
                legs.Add(new RouteLegDto
                {
                    StartLat = first.Latitude,
                    StartLng = first.Longitude,
                    EndLat = last.Latitude,
                    EndLng = last.Longitude,
                    DistanceMeters = distance,
                    DurationSeconds = duration
                });
            }

            return new OptimizedRouteDto
            {
                Polyline = polyline,
                DistanceMeters = distance,
                DurationSeconds = duration,
                Legs = legs,
                Metadata = new Dictionary<string, object>
                {
                    ["raw_response"] = content
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting route from OSRM");
            throw new ApplicationException("Failed to get optimized route", ex);
        }
    }

    public async Task<Dictionary<int, OptimizedRouteDto>> BatchOptimizeRoutesAsync(List<RouteOptimizationRequestDto> requests)
    {
        var results = new Dictionary<int, OptimizedRouteDto>();
        var tasks = requests.Select(async (req, idx) =>
        {
            try
            {
                var res = await GetOptimizedRouteAsync(req);
                results[idx] = res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing OSRM route request {idx}");
            }
        });
        await Task.WhenAll(tasks);
        return results;
    }

    public async Task<Dictionary<string, object>> GetDistanceMatrixAsync(List<RouteWaypointDto> origins, List<RouteWaypointDto> destinations)
    {
        try
        {
            var all = new List<RouteWaypointDto>();
            all.AddRange(origins);
            all.AddRange(destinations);
            var coords = string.Join(";", all.Select(w =>
                $"{w.Longitude.ToString(CultureInfo.InvariantCulture)},{w.Latitude.ToString(CultureInfo.InvariantCulture)}"));
            var sourceIdx = string.Join(";", Enumerable.Range(0, origins.Count));
            var destIdx = string.Join(";", Enumerable.Range(origins.Count, destinations.Count));

            var url = $"table/v1/driving/{coords}?sources={sourceIdx}&destinations={destIdx}&annotations=distance,duration";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Dictionary<string, object>>(content) ?? new Dictionary<string, object>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting OSRM distance matrix");
            throw new ApplicationException("Failed to get distance matrix", ex);
        }
    }

    public Task<string> GetStaticMapUrlAsync(string polyline, int width = 600, int height = 400)
    {
        if (string.IsNullOrWhiteSpace(polyline))
        {
            return Task.FromResult(string.Empty);
        }

        var points = DecodePolyline(polyline);
        if (points.Count == 0)
        {
            return Task.FromResult(string.Empty);
        }

        // Sample points to avoid very long URLs
        var maxPoints = 100;
        var step = Math.Max(1, points.Count / maxPoints);
        var sampled = points.Where((p, idx) => idx % step == 0).ToList();
        if (sampled.Last() != points.Last())
        {
            sampled.Add(points.Last());
        }

        // Build path parameter
        var pathParam = "color:0x0000ff|weight:3|" + string.Join("|", sampled.Select(p =>
            $"{p.Lat.ToString(System.Globalization.CultureInfo.InvariantCulture)},{p.Lng.ToString(System.Globalization.CultureInfo.InvariantCulture)}"));

        // Compute center as midpoint
        var centerLat = sampled.Average(p => p.Lat);
        var centerLng = sampled.Average(p => p.Lng);

        var url =
            $"https://staticmap.openstreetmap.de/staticmap.php?center={centerLat.ToString(System.Globalization.CultureInfo.InvariantCulture)},{centerLng.ToString(System.Globalization.CultureInfo.InvariantCulture)}" +
            $"&size={width}x{height}&maptype=mapnik&path={Uri.EscapeDataString(pathParam)}";

        return Task.FromResult(url);
    }

    public async Task<byte[]> GetStaticMapImageAsync(string polyline, int width = 600, int height = 400)
    {
        var url = await GetStaticMapUrlAsync(polyline, width, height);
        if (string.IsNullOrEmpty(url))
        {
            return Array.Empty<byte>();
        }

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsByteArrayAsync();
    }

    // Google encoded polyline decoder
    private static List<(double Lat, double Lng)> DecodePolyline(string encoded)
    {
        var poly = new List<(double Lat, double Lng)>();
        if (string.IsNullOrEmpty(encoded)) return poly;

        int index = 0;
        int lat = 0;
        int lng = 0;

        while (index < encoded.Length)
        {
            int b;
            int shift = 0;
            int result = 0;
            do
            {
                b = encoded[index++] - 63;
                result |= (b & 0x1f) << shift;
                shift += 5;
            } while (b >= 0x20 && index < encoded.Length);
            int dlat = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
            lat += dlat;

            shift = 0;
            result = 0;
            do
            {
                b = encoded[index++] - 63;
                result |= (b & 0x1f) << shift;
                shift += 5;
            } while (b >= 0x20 && index < encoded.Length);
            int dlng = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
            lng += dlng;

            double latD = lat / 1E5;
            double lngD = lng / 1E5;
            poly.Add((latD, lngD));
        }

        return poly;
    }
}

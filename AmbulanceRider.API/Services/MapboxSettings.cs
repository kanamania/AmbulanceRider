namespace AmbulanceRider.API.Services;

public class MapboxSettings
{
    public string AccessToken { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://api.mapbox.com";
    public string MapStyle { get; set; } = "mapbox/streets-v11";
    public string RoutingProfile { get; set; } = "mapbox/driving";
    public string StaticMapBaseUrl { get; set; } = "https://api.mapbox.com/styles/v1";
    public int StaticMapWidth { get; set; } = 600;
    public int StaticMapHeight { get; set; } = 400;
    public double DefaultZoom { get; set; } = 14.0;
    public int MaxWaypoints { get; set; } = 25;
    public int MaxDistanceMeters { get; set; } = 100000; // 100km
    public int MaxDurationSeconds { get; set; } = 10800; // 3 hours
    public string DefaultLanguage { get; set; } = "en";
}

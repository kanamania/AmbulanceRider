namespace AmbulanceRider.API.Services;

public class RoutingSettings
{
    public string? Provider { get; set; } = "OSRM"; // Options: OSRM, Mapbox
}

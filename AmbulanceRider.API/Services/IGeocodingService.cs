namespace AmbulanceRider.API.Services;

public interface IGeocodingService
{
    Task<string?> GetRegionFromCoordinatesAsync(double latitude, double longitude);
}

namespace AmbulanceRider.API.DTOs
{
    public class DataHashResponseDto
    {
        public string UserHash { get; set; } = string.Empty;
        public string ProfileHash { get; set; } = string.Empty;
        public string TripTypesHash { get; set; } = string.Empty;
        public string LocationsHash { get; set; } = string.Empty;
        public string TripsHash { get; set; } = string.Empty;
    }
}

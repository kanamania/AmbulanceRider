namespace AmbulanceRider.API.DTOs
{
    public class SystemDataResponseDto
    {
        public List<TripDto> Trips { get; set; } = new();
        public List<LocationDto> Locations { get; set; } = new();
        public List<TripTypeDto> TripTypes { get; set; } = new();
        public List<VehicleDto> Vehicles { get; set; } = new();
    }
}

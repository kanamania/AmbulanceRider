using AmbulanceRider.API.DTOs;
using AmbulanceRider.API.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace AmbulanceRider.API.Services
{
    public interface IDataHashNotificationService
    {
        Task NotifyUserDataChanged(string userId);
        Task NotifyTripTypesChanged();
        Task NotifyLocationsChanged();
    }

    public class DataHashNotificationService : IDataHashNotificationService
    {
        private readonly IDataHashService _hashService;
        private readonly IHubContext<NotificationHub> _hubContext;

        public DataHashNotificationService(
            IDataHashService hashService,
            IHubContext<NotificationHub> hubContext)
        {
            _hashService = hashService;
            _hubContext = hubContext;
        }

        public async Task NotifyUserDataChanged(string userId)
        {
            var hashes = new DataHashResponseDto
            {
                UserHash = await _hashService.GenerateUserHashAsync(userId),
                ProfileHash = await _hashService.GenerateProfileHashAsync(userId),
                TripTypesHash = await _hashService.GenerateTripTypesHashAsync(),
                LocationsHash = await _hashService.GenerateLocationsHashAsync(),
                TripsHash = await _hashService.GenerateTripsHashAsync(userId)
            };

            await _hubContext.Clients.User(userId).SendAsync("DataHashChanged", hashes);
        }

        public async Task NotifyTripTypesChanged()
        {
            var tripTypesHash = await _hashService.GenerateTripTypesHashAsync();
            await _hubContext.Clients.All.SendAsync("TripTypesHashChanged", tripTypesHash);
        }

        public async Task NotifyLocationsChanged()
        {
            var locationsHash = await _hashService.GenerateLocationsHashAsync();
            await _hubContext.Clients.All.SendAsync("LocationsHashChanged", locationsHash);
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace AmbulanceRider.API.Hubs;

/// <summary>
/// SignalR hub for real-time trip updates
/// </summary>
[Authorize]
public class TripHub : Hub
{
    private readonly ILogger<TripHub> _logger;

    public TripHub(ILogger<TripHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst("sub")?.Value ?? Context.User?.FindFirst("id")?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
            _logger.LogInformation($"User {userId} connected to TripHub with connection ID {Context.ConnectionId}");
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirst("sub")?.Value ?? Context.User?.FindFirst("id")?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
            _logger.LogInformation($"User {userId} disconnected from TripHub");
        }
        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Subscribe to updates for a specific trip
    /// </summary>
    public async Task SubscribeToTrip(int tripId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"trip_{tripId}");
        _logger.LogInformation($"Connection {Context.ConnectionId} subscribed to trip {tripId}");
    }

    /// <summary>
    /// Unsubscribe from updates for a specific trip
    /// </summary>
    public async Task UnsubscribeFromTrip(int tripId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"trip_{tripId}");
        _logger.LogInformation($"Connection {Context.ConnectionId} unsubscribed from trip {tripId}");
    }

    /// <summary>
    /// Send location update for a trip
    /// </summary>
    public async Task SendLocationUpdate(int tripId, double latitude, double longitude)
    {
        await Clients.Group($"trip_{tripId}").SendAsync("ReceiveLocationUpdate", new
        {
            TripId = tripId,
            Latitude = latitude,
            Longitude = longitude,
            Timestamp = DateTime.UtcNow
        });
    }
}

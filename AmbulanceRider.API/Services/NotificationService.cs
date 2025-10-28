using AmbulanceRider.API.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace AmbulanceRider.API.Services;

public class NotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _notificationHub;
    private readonly IHubContext<TripHub> _tripHub;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        IHubContext<NotificationHub> notificationHub,
        IHubContext<TripHub> tripHub,
        ILogger<NotificationService> logger)
    {
        _notificationHub = notificationHub;
        _tripHub = tripHub;
        _logger = logger;
    }

    public async Task SendNotificationToUserAsync(string userId, string title, string message, object? data = null)
    {
        try
        {
            await _notificationHub.Clients.Group($"user_{userId}").SendAsync("ReceiveNotification", new
            {
                Title = title,
                Message = message,
                Data = data,
                Timestamp = DateTime.UtcNow
            });
            _logger.LogInformation($"Notification sent to user {userId}: {title}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error sending notification to user {userId}");
        }
    }

    public async Task SendNotificationToGroupAsync(string groupName, string title, string message, object? data = null)
    {
        try
        {
            await _notificationHub.Clients.Group(groupName).SendAsync("ReceiveNotification", new
            {
                Title = title,
                Message = message,
                Data = data,
                Timestamp = DateTime.UtcNow
            });
            _logger.LogInformation($"Notification sent to group {groupName}: {title}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error sending notification to group {groupName}");
        }
    }

    public async Task SendNotificationToAllAsync(string title, string message, object? data = null)
    {
        try
        {
            await _notificationHub.Clients.All.SendAsync("ReceiveNotification", new
            {
                Title = title,
                Message = message,
                Data = data,
                Timestamp = DateTime.UtcNow
            });
            _logger.LogInformation($"Notification sent to all users: {title}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification to all users");
        }
    }

    public async Task SendTripUpdateAsync(int tripId, string updateType, object data)
    {
        try
        {
            await _tripHub.Clients.Group($"trip_{tripId}").SendAsync("ReceiveTripUpdate", new
            {
                TripId = tripId,
                UpdateType = updateType,
                Data = data,
                Timestamp = DateTime.UtcNow
            });
            _logger.LogInformation($"Trip update sent for trip {tripId}: {updateType}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error sending trip update for trip {tripId}");
        }
    }

    public async Task SendTripStatusChangeAsync(int tripId, string oldStatus, string newStatus, object? additionalData = null)
    {
        try
        {
            await _tripHub.Clients.Group($"trip_{tripId}").SendAsync("ReceiveTripStatusChange", new
            {
                TripId = tripId,
                OldStatus = oldStatus,
                NewStatus = newStatus,
                AdditionalData = additionalData,
                Timestamp = DateTime.UtcNow
            });
            _logger.LogInformation($"Trip status change sent for trip {tripId}: {oldStatus} -> {newStatus}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error sending trip status change for trip {tripId}");
        }
    }
}

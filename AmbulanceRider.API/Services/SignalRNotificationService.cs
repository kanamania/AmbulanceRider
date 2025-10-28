using System.Text.Json;
using AmbulanceRider.API.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace AmbulanceRider.API.Services;

public class SignalRNotificationService : INotificationService
{
    private readonly IHubContext<TripHub> _tripHub;
    private readonly ILogger<SignalRNotificationService> _logger;

    public SignalRNotificationService(IHubContext<TripHub> tripHub, ILogger<SignalRNotificationService> logger)
    {
        _tripHub = tripHub;
        _logger = logger;
    }

    public async Task SendNotificationToUserAsync(string userId, string title, string message, object? data = null)
    {
        try
        {
            await _tripHub.Clients.User(userId).SendAsync("ReceiveNotification", new
            {
                Title = title,
                Message = message,
                Data = data,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification to user {UserId}", userId);
        }
    }

    public async Task SendNotificationToGroupAsync(string groupName, string title, string message, object? data = null)
    {
        try
        {
            await _tripHub.Clients.Group(groupName).SendAsync("ReceiveNotification", new
            {
                Title = title,
                Message = message,
                Data = data,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification to group {GroupName}", groupName);
        }
    }

    public async Task SendNotificationToAllAsync(string title, string message, object? data = null)
    {
        try
        {
            await _tripHub.Clients.All.SendAsync("ReceiveNotification", new
            {
                Title = title,
                Message = message,
                Data = data,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error broadcasting notification to all users");
        }
    }

    public async Task SendTripUpdateAsync(int tripId, string updateType, object data)
    {
        try
        {
            await _tripHub.Clients.Group(GetTripGroupName(tripId)).SendAsync("TripUpdated", new
            {
                TripId = tripId,
                UpdateType = updateType,
                Data = data,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending trip update for trip {TripId}", tripId);
        }
    }

    public async Task SendTripStatusChangeAsync(int tripId, string oldStatus, string newStatus, object? additionalData = null)
    {
        try
        {
            await _tripHub.Clients.Group(GetTripGroupName(tripId)).SendAsync("TripStatusChanged", new
            {
                TripId = tripId,
                OldStatus = oldStatus,
                NewStatus = newStatus,
                Data = additionalData,
                Timestamp = DateTime.UtcNow
            });
            
            // Also notify relevant user groups based on the new status
            await NotifyUserGroupsAboutStatusChange(tripId, newStatus, additionalData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending status change notification for trip {TripId}", tripId);
        }
    }

    private async Task NotifyUserGroupsAboutStatusChange(int tripId, string newStatus, object? additionalData = null)
    {
        var message = $"Trip {tripId} status changed to {newStatus}";
        
        // Notify dispatchers and admins about all status changes
        await SendNotificationToGroupAsync("dispatchers", "Trip Status Update", message, additionalData);
        await SendNotificationToGroupAsync("admins", "Trip Status Update", message, additionalData);
        
        // Notify drivers about relevant status changes
        if (newStatus == "Assigned" || newStatus == "InProgress" || newStatus == "Completed")
        {
            await SendNotificationToGroupAsync("drivers", "Trip Update", message, additionalData);
        }
    }

    private static string GetTripGroupName(int tripId) => $"trip-{tripId}";
}

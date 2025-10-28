namespace AmbulanceRider.API.Services;

public interface INotificationService
{
    Task SendNotificationToUserAsync(string userId, string title, string message, object? data = null);
    Task SendNotificationToGroupAsync(string groupName, string title, string message, object? data = null);
    Task SendNotificationToAllAsync(string title, string message, object? data = null);
    Task SendTripUpdateAsync(int tripId, string updateType, object data);
    Task SendTripStatusChangeAsync(int tripId, string oldStatus, string newStatus, object? additionalData = null);
}

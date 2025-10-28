using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;

namespace AmbulanceRider.Services;

public class SignalRService : IAsyncDisposable
{
    private HubConnection? _notificationConnection;
    private HubConnection? _tripConnection;
    private readonly string _apiBaseUrl;
    private readonly AuthService _authService;

    public event Action<NotificationMessage>? OnNotificationReceived;
    public event Action<TripUpdate>? OnTripUpdateReceived;
    public event Action<TripStatusChange>? OnTripStatusChangeReceived;
    public event Action<LocationUpdate>? OnLocationUpdateReceived;

    public SignalRService(IConfiguration configuration, AuthService authService)
    {
        _apiBaseUrl = configuration["ApiBaseUrl"] ?? "https://localhost:5001";
        _authService = authService;
    }

    public async Task StartNotificationHubAsync()
    {
        if (_notificationConnection != null)
            return;

        var token = await _authService.GetTokenAsync();
        
        _notificationConnection = new HubConnectionBuilder()
            .WithUrl($"{_apiBaseUrl}/hubs/notifications", options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(token);
            })
            .WithAutomaticReconnect()
            .Build();

        _notificationConnection.On<NotificationMessage>("ReceiveNotification", notification =>
        {
            OnNotificationReceived?.Invoke(notification);
        });

        await _notificationConnection.StartAsync();
    }

    public async Task StartTripHubAsync()
    {
        if (_tripConnection != null)
            return;

        var token = await _authService.GetTokenAsync();
        
        _tripConnection = new HubConnectionBuilder()
            .WithUrl($"{_apiBaseUrl}/hubs/trips", options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(token);
            })
            .WithAutomaticReconnect()
            .Build();

        _tripConnection.On<TripUpdate>("ReceiveTripUpdate", update =>
        {
            OnTripUpdateReceived?.Invoke(update);
        });

        _tripConnection.On<TripStatusChange>("ReceiveTripStatusChange", statusChange =>
        {
            OnTripStatusChangeReceived?.Invoke(statusChange);
        });

        _tripConnection.On<LocationUpdate>("ReceiveLocationUpdate", locationUpdate =>
        {
            OnLocationUpdateReceived?.Invoke(locationUpdate);
        });

        await _tripConnection.StartAsync();
    }

    public async Task SubscribeToTripAsync(int tripId)
    {
        if (_tripConnection?.State == HubConnectionState.Connected)
        {
            await _tripConnection.InvokeAsync("SubscribeToTrip", tripId);
        }
    }

    public async Task UnsubscribeFromTripAsync(int tripId)
    {
        if (_tripConnection?.State == HubConnectionState.Connected)
        {
            await _tripConnection.InvokeAsync("UnsubscribeFromTrip", tripId);
        }
    }

    public async Task SendLocationUpdateAsync(int tripId, double latitude, double longitude)
    {
        if (_tripConnection?.State == HubConnectionState.Connected)
        {
            await _tripConnection.InvokeAsync("SendLocationUpdate", tripId, latitude, longitude);
        }
    }

    public async Task JoinGroupAsync(string groupName)
    {
        if (_notificationConnection?.State == HubConnectionState.Connected)
        {
            await _notificationConnection.InvokeAsync("JoinGroup", groupName);
        }
    }

    public async Task LeaveGroupAsync(string groupName)
    {
        if (_notificationConnection?.State == HubConnectionState.Connected)
        {
            await _notificationConnection.InvokeAsync("LeaveGroup", groupName);
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_notificationConnection != null)
        {
            await _notificationConnection.DisposeAsync();
        }
        if (_tripConnection != null)
        {
            await _tripConnection.DisposeAsync();
        }
    }

    // DTOs for SignalR messages
    public class NotificationMessage
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public object? Data { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class TripUpdate
    {
        public int TripId { get; set; }
        public string UpdateType { get; set; } = string.Empty;
        public object? Data { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class TripStatusChange
    {
        public int TripId { get; set; }
        public string OldStatus { get; set; } = string.Empty;
        public string NewStatus { get; set; } = string.Empty;
        public object? AdditionalData { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class LocationUpdate
    {
        public int TripId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime Timestamp { get; set; }
    }
}

# Advanced Features Implementation Guide

This document provides a comprehensive guide to the advanced features implemented in the AmbulanceRider system.

## Table of Contents

1. [Real-time Push Notifications with SignalR](#real-time-push-notifications-with-signalr)
2. [Advanced Reporting and Analytics Dashboard](#advanced-reporting-and-analytics-dashboard)
3. [Telemetry Analytics Dashboard](#telemetry-analytics-dashboard)
4. [Telemetry Data Export](#telemetry-data-export)
5. [Telemetry Aggregation and Statistics](#telemetry-aggregation-and-statistics)
6. [Multi-language Support](#multi-language-support)
7. [Email Notifications](#email-notifications)
8. [Performance Monitoring Dashboard](#performance-monitoring-dashboard)

---

## Real-time Push Notifications with SignalR

### Overview
SignalR enables real-time, bi-directional communication between the server and clients, allowing instant notifications for trip updates, status changes, and system events.

### Hub Endpoints

#### NotificationHub
- **Endpoint**: `/hubs/notifications`
- **Purpose**: General notifications for users
- **Features**:
  - User-specific notifications
  - Group-based notifications
  - Broadcast notifications

#### TripHub
- **Endpoint**: `/hubs/trips`
- **Purpose**: Real-time trip updates
- **Features**:
  - Trip-specific subscriptions
  - Location updates
  - Status change notifications

### Client Connection Example

```javascript
// Connect to NotificationHub
const notificationConnection = new signalR.HubConnectionBuilder()
    .withUrl("https://your-api-url/hubs/notifications", {
        accessTokenFactory: () => yourAccessToken
    })
    .withAutomaticReconnect()
    .build();

// Listen for notifications
notificationConnection.on("ReceiveNotification", (notification) => {
    console.log("Notification received:", notification);
    // Handle notification (title, message, data, timestamp)
});

// Start connection
await notificationConnection.start();

// Connect to TripHub
const tripConnection = new signalR.HubConnectionBuilder()
    .withUrl("https://your-api-url/hubs/trips", {
        accessTokenFactory: () => yourAccessToken
    })
    .withAutomaticReconnect()
    .build();

// Subscribe to a specific trip
await tripConnection.invoke("SubscribeToTrip", tripId);

// Listen for trip updates
tripConnection.on("ReceiveTripUpdate", (update) => {
    console.log("Trip update:", update);
});

tripConnection.on("ReceiveTripStatusChange", (statusChange) => {
    console.log("Status changed:", statusChange);
});

// Send location update
await tripConnection.invoke("SendLocationUpdate", tripId, latitude, longitude);
```

### Server-side Usage

```csharp
// Inject INotificationService
private readonly INotificationService _notificationService;

// Send notification to specific user
await _notificationService.SendNotificationToUserAsync(
    userId, 
    "Trip Approved", 
    "Your trip has been approved",
    new { TripId = tripId }
);

// Send notification to group
await _notificationService.SendNotificationToGroupAsync(
    "Drivers", 
    "New Trip Available", 
    "A new trip is available for assignment"
);

// Send trip update
await _notificationService.SendTripUpdateAsync(
    tripId, 
    "LocationUpdate", 
    new { Latitude = lat, Longitude = lng }
);

// Send trip status change
await _notificationService.SendTripStatusChangeAsync(
    tripId, 
    "Pending", 
    "Approved"
);
```

---

## Advanced Reporting and Analytics Dashboard

### Dashboard Statistics API

**Endpoint**: `GET /api/analytics/dashboard`

**Query Parameters**:
- `startDate` (optional): Start date for statistics
- `endDate` (optional): End date for statistics

**Response**:
```json
{
  "totalTrips": 150,
  "pendingTrips": 10,
  "approvedTrips": 20,
  "inProgressTrips": 15,
  "completedTrips": 100,
  "cancelledTrips": 3,
  "rejectedTrips": 2,
  "totalVehicles": 25,
  "activeVehicles": 15,
  "totalDrivers": 30,
  "activeDrivers": 18,
  "totalUsers": 50,
  "startDate": "2025-09-28T00:00:00Z",
  "endDate": "2025-10-28T00:00:00Z"
}
```

### Trip Statistics by Status

**Endpoint**: `GET /api/analytics/trips/by-status`

**Response**:
```json
[
  {
    "status": "Completed",
    "count": 100,
    "percentage": 66.67
  },
  {
    "status": "Pending",
    "count": 10,
    "percentage": 6.67
  }
]
```

### Trip Statistics by Date

**Endpoint**: `GET /api/analytics/trips/by-date`

**Query Parameters**:
- `groupBy`: `day`, `week`, or `month`

**Response**:
```json
[
  {
    "date": "2025-10-01T00:00:00Z",
    "count": 15,
    "label": "2025-10-01"
  },
  {
    "date": "2025-10-02T00:00:00Z",
    "count": 20,
    "label": "2025-10-02"
  }
]
```

### Vehicle Utilization

**Endpoint**: `GET /api/analytics/vehicles/utilization`

**Response**:
```json
[
  {
    "vehicleId": 1,
    "vehicleName": "Ambulance 001",
    "plateNumber": "ABC-123",
    "totalTrips": 50,
    "completedTrips": 45,
    "inProgressTrips": 2,
    "utilizationRate": 90.0
  }
]
```

### Driver Performance

**Endpoint**: `GET /api/analytics/drivers/performance`

**Response**:
```json
[
  {
    "driverId": "guid-here",
    "driverName": "John Doe",
    "email": "john@example.com",
    "totalTrips": 75,
    "completedTrips": 70,
    "inProgressTrips": 3,
    "cancelledTrips": 2,
    "completionRate": 93.33
  }
]
```

---

## Telemetry Analytics Dashboard

### Telemetry Statistics

**Endpoint**: `GET /api/telemetryanalytics/stats`

**Response**:
```json
{
  "totalEvents": 5000,
  "uniqueUsers": 150,
  "eventTypeCounts": {
    "Login": 500,
    "TripCreate": 300,
    "TripStatusUpdate": 450
  },
  "deviceTypeCounts": {
    "Mobile": 3000,
    "Desktop": 2000
  },
  "browserCounts": {
    "Chrome": 2500,
    "Firefox": 1500,
    "Safari": 1000
  },
  "osCounts": {
    "Android": 1800,
    "iOS": 1200,
    "Windows": 2000
  },
  "startDate": "2025-09-28T00:00:00Z",
  "endDate": "2025-10-28T00:00:00Z"
}
```

### Telemetry Events

**Endpoint**: `GET /api/telemetryanalytics/events`

**Query Parameters**:
- `startDate`, `endDate`: Date range
- `eventType`: Filter by event type
- `userId`: Filter by user
- `page`, `pageSize`: Pagination

**Response Headers**:
- `X-Total-Count`: Total number of events
- `X-Page`: Current page
- `X-Page-Size`: Page size

### Telemetry Aggregation

**Endpoint**: `GET /api/telemetryanalytics/aggregation/by-event-type`

**Query Parameters**:
- `groupBy`: `hour`, `day`, `week`, or `month`

**Response**:
```json
{
  "Login": [
    {
      "date": "2025-10-01T00:00:00Z",
      "count": 50
    }
  ],
  "TripCreate": [
    {
      "date": "2025-10-01T00:00:00Z",
      "count": 30
    }
  ]
}
```

### Telemetry Heatmap

**Endpoint**: `GET /api/telemetryanalytics/heatmap`

**Response**:
```json
[
  {
    "latitude": 40.7128,
    "longitude": -74.0060,
    "eventType": "Login"
  }
]
```

---

## Telemetry Data Export

### Export to CSV

**Endpoint**: `GET /api/telemetryanalytics/export/csv`

**Query Parameters**:
- `startDate`, `endDate`: Date range
- `eventType`: Filter by event type

**Response**: CSV file download

**File Format**:
```csv
Id,EventType,EventDetails,UserId,UserName,DeviceType,Browser,OperatingSystem,Latitude,Longitude,IpAddress,CreatedAt
1,Login,"{...}",guid-here,John Doe,Mobile,Chrome,Android,40.7128,-74.0060,192.168.1.1,2025-10-28T10:00:00Z
```

### Export to JSON

**Endpoint**: `GET /api/telemetryanalytics/export/json`

**Query Parameters**: Same as CSV export

**Response**: JSON file download

**File Format**:
```json
[
  {
    "id": 1,
    "eventType": "Login",
    "eventDetails": "{...}",
    "userId": "guid-here",
    "userName": "John Doe",
    "deviceType": "Mobile",
    "location": {
      "latitude": 40.7128,
      "longitude": -74.0060
    },
    "network": {
      "ipAddress": "192.168.1.1"
    },
    "createdAt": "2025-10-28T10:00:00Z"
  }
]
```

---

## Multi-language Support

### Supported Languages

- English (en)
- Spanish (es)
- French (fr)

### Get All Translations

**Endpoint**: `GET /api/localization/{culture}`

**Example**: `GET /api/localization/es`

**Response**:
```json
{
  "Common": {
    "Save": "Guardar",
    "Cancel": "Cancelar",
    "Delete": "Eliminar"
  },
  "Auth": {
    "Login": "Iniciar sesión",
    "Logout": "Cerrar sesión"
  }
}
```

### Get Specific Translation

**Endpoint**: `GET /api/localization/{culture}/{key}`

**Example**: `GET /api/localization/es/Common.Save`

**Response**:
```json
{
  "key": "Common.Save",
  "value": "Guardar"
}
```

### Get Supported Cultures

**Endpoint**: `GET /api/localization/cultures`

**Response**:
```json
["en", "es", "fr"]
```

### Client-side Usage

```javascript
// Fetch translations for Spanish
const response = await fetch('/api/localization/es');
const translations = await response.json();

// Use translations
console.log(translations.Common.Save); // "Guardar"
console.log(translations.Auth.Login); // "Iniciar sesión"
```

### Adding New Languages

1. Create a new resource file: `Resources/SharedResources.{culture}.json`
2. Copy the structure from an existing file
3. Translate all strings
4. Restart the application

---

## Email Notifications

### Configuration

Add the following to `appsettings.json`:

```json
{
  "Email": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "SmtpUsername": "your-email@gmail.com",
    "SmtpPassword": "your-app-password",
    "FromEmail": "noreply@ambulancerider.com",
    "FromName": "AmbulanceRider",
    "EnableSsl": true
  }
}
```

### Usage

```csharp
// Inject IEmailService
private readonly IEmailService _emailService;

// Send email
await _emailService.SendEmailAsync(
    to: "user@example.com",
    subject: "Trip Status Update",
    body: "<h1>Your trip has been approved</h1><p>Trip details...</p>"
);
```

### Email Templates

Create HTML email templates for:
- Trip approval notifications
- Trip rejection notifications
- Trip status changes
- System alerts

---

## Performance Monitoring Dashboard

### Performance Metrics

**Endpoint**: `GET /api/performance/metrics`

**Query Parameters**:
- `startDate`, `endDate`: Date range (default: last 24 hours)

**Response**:
```json
{
  "averageResponseTime": 125.5,
  "totalRequests": 10000,
  "failedRequests": 50,
  "errorRate": 0.5,
  "endpointCounts": {
    "/api/trips": 3000,
    "/api/vehicles": 2000
  },
  "endpointAverageResponseTimes": {
    "/api/trips": 150.2,
    "/api/vehicles": 100.8
  },
  "startDate": "2025-10-27T14:00:00Z",
  "endDate": "2025-10-28T14:00:00Z"
}
```

### Slow Requests

**Endpoint**: `GET /api/performance/slow-requests`

**Query Parameters**:
- `threshold`: Response time threshold in ms (default: 1000)

**Response**:
```json
[
  {
    "id": 1,
    "endpoint": "/api/trips/123",
    "httpMethod": "GET",
    "statusCode": 200,
    "responseTimeMs": 2500.5,
    "timestamp": "2025-10-28T10:00:00Z",
    "userId": "guid-here",
    "ipAddress": "192.168.1.1"
  }
]
```

### Error Logs

**Endpoint**: `GET /api/performance/errors`

**Response**:
```json
[
  {
    "id": 1,
    "endpoint": "/api/trips/999",
    "httpMethod": "GET",
    "statusCode": 404,
    "responseTimeMs": 50.2,
    "timestamp": "2025-10-28T10:00:00Z",
    "errorMessage": "Trip not found"
  }
]
```

### Performance Trends

**Endpoint**: `GET /api/performance/trends`

**Query Parameters**:
- `groupBy`: `minute`, `hour`, or `day`

**Response**:
```json
[
  {
    "timestamp": "2025-10-28T10:00:00Z",
    "averageResponseTime": 120.5,
    "requestCount": 500,
    "errorCount": 5
  }
]
```

### Endpoint Statistics

**Endpoint**: `GET /api/performance/endpoints`

**Response**:
```json
[
  {
    "endpoint": "/api/trips",
    "httpMethod": "GET",
    "totalRequests": 3000,
    "averageResponseTime": 150.2,
    "minResponseTime": 50.0,
    "maxResponseTime": 500.0,
    "errorCount": 10,
    "errorRate": 0.33
  }
]
```

---

## Integration Examples

### Dashboard with Charts

```javascript
// Fetch dashboard stats
const stats = await fetch('/api/analytics/dashboard').then(r => r.json());

// Create pie chart for trip status
const statusData = await fetch('/api/analytics/trips/by-status').then(r => r.json());

// Create line chart for trips over time
const dateData = await fetch('/api/analytics/trips/by-date?groupBy=day').then(r => r.json());

// Use Chart.js, D3.js, or any charting library
new Chart(ctx, {
    type: 'pie',
    data: {
        labels: statusData.map(s => s.status),
        datasets: [{
            data: statusData.map(s => s.count)
        }]
    }
});
```

### Real-time Dashboard Updates

```javascript
// Connect to SignalR
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/notifications")
    .build();

connection.on("ReceiveNotification", (notification) => {
    // Update dashboard in real-time
    updateDashboard();
});

await connection.start();
```

---

## Security Considerations

1. **Authentication**: All endpoints require JWT authentication
2. **Authorization**: Admin-only endpoints are protected with `[Authorize(Roles = "Admin")]`
3. **CORS**: Configure allowed origins in `appsettings.json`
4. **Email**: Use app-specific passwords, not account passwords
5. **SignalR**: Tokens are validated for hub connections

---

## Performance Optimization

1. **Caching**: Consider implementing Redis for frequently accessed data
2. **Pagination**: All list endpoints support pagination
3. **Indexing**: Database indexes are configured for common queries
4. **Async Operations**: All operations are asynchronous
5. **Connection Pooling**: Entity Framework handles database connection pooling

---

## Troubleshooting

### SignalR Connection Issues
- Verify CORS settings allow SignalR
- Check JWT token is valid
- Ensure WebSocket support is enabled

### Email Not Sending
- Verify SMTP credentials
- Check firewall/network settings
- Enable "Less secure app access" or use app passwords

### Performance Monitoring
- Check database connection
- Verify middleware is registered correctly
- Monitor disk space for log storage

---

## Next Steps

1. Create frontend components for dashboards
2. Implement data visualization with charts
3. Add more language translations
4. Configure email templates
5. Set up monitoring alerts
6. Implement data retention policies

---

## Support

For issues or questions, please refer to the main documentation or contact the development team.

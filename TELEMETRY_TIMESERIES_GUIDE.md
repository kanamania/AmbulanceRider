# Telemetry Timeseries Logging Guide

## Overview

The Ambulance Rider system now supports **timeseries telemetry logging**, enabling efficient batch logging and time-based querying of telemetry data. This is essential for tracking user behavior, device metrics, location history, and system events over time.

## Features

### 1. **Batch Telemetry Logging**
- Log multiple telemetry events in a single API call
- Efficient bulk inserts for high-volume data
- Ideal for offline data synchronization and periodic uploads

### 2. **Timeseries Queries**
- Query telemetry data by time ranges
- Filter by event type and user
- Support for large datasets with configurable limits

### 3. **User Privacy Controls**
- Users can only access their own telemetry data
- Admin and Dispatcher roles have full access
- Secure authorization checks on all endpoints

## API Endpoints

### 1. Log Single Telemetry Event

**POST** `/api/telemetry`

```json
{
  "eventType": "LocationUpdate",
  "eventDetails": "Driver en route to pickup",
  "telemetry": {
    "latitude": 40.7128,
    "longitude": -74.0060,
    "speed": 15.5,
    "batteryLevel": 0.85,
    "timestamp": "2025-10-27T22:00:00Z",
    "deviceType": "Mobile",
    "operatingSystem": "Android",
    "connectionType": "cellular"
  }
}
```

**Response:**
```json
{
  "message": "Telemetry logged successfully"
}
```

### 2. Log Batch Telemetry (Timeseries)

**POST** `/api/telemetry/batch`

```json
{
  "events": [
    {
      "eventType": "LocationUpdate",
      "eventDetails": "Periodic location update",
      "telemetry": {
        "latitude": 40.7128,
        "longitude": -74.0060,
        "speed": 15.5,
        "batteryLevel": 0.85,
        "timestamp": "2025-10-27T22:00:00Z"
      }
    },
    {
      "eventType": "LocationUpdate",
      "eventDetails": "Periodic location update",
      "telemetry": {
        "latitude": 40.7130,
        "longitude": -74.0062,
        "speed": 16.2,
        "batteryLevel": 0.84,
        "timestamp": "2025-10-27T22:01:00Z"
      }
    },
    {
      "eventType": "LocationUpdate",
      "eventDetails": "Periodic location update",
      "telemetry": {
        "latitude": 40.7135,
        "longitude": -74.0065,
        "speed": 17.0,
        "batteryLevel": 0.83,
        "timestamp": "2025-10-27T22:02:00Z"
      }
    }
  ]
}
```

**Response:**
```json
{
  "message": "Batch telemetry logged successfully",
  "count": 3
}
```

### 3. Query Timeseries Data (Admin/Dispatcher)

**POST** `/api/telemetry/timeseries`

**Authorization:** Requires `Admin` or `Dispatcher` role

```json
{
  "startTime": "2025-10-27T20:00:00Z",
  "endTime": "2025-10-27T23:00:00Z",
  "eventType": "LocationUpdate",
  "limit": 1000
}
```

**Response:**
```json
[
  {
    "id": 1,
    "eventType": "LocationUpdate",
    "eventDetails": "Periodic location update",
    "userId": "123e4567-e89b-12d3-a456-426614174000",
    "userName": "John Doe",
    "latitude": 40.7128,
    "longitude": -74.0060,
    "speed": 15.5,
    "batteryLevel": 0.85,
    "createdAt": "2025-10-27T22:00:00Z",
    "eventTimestamp": "2025-10-27T22:00:00Z"
  },
  {
    "id": 2,
    "eventType": "LocationUpdate",
    "eventDetails": "Periodic location update",
    "userId": "123e4567-e89b-12d3-a456-426614174000",
    "userName": "John Doe",
    "latitude": 40.7130,
    "longitude": -74.0062,
    "speed": 16.2,
    "batteryLevel": 0.84,
    "createdAt": "2025-10-27T22:01:00Z",
    "eventTimestamp": "2025-10-27T22:01:00Z"
  }
]
```

### 4. Get User Timeseries Data

**GET** `/api/telemetry/user/{userId}/timeseries?startTime={start}&endTime={end}&eventType={type}`

**Authorization:** User can only access their own data, or Admin/Dispatcher can access any user

**Query Parameters:**
- `startTime` (required): ISO 8601 datetime
- `endTime` (required): ISO 8601 datetime
- `eventType` (optional): Filter by event type

**Example:**
```
GET /api/telemetry/user/123e4567-e89b-12d3-a456-426614174000/timeseries?startTime=2025-10-27T20:00:00Z&endTime=2025-10-27T23:00:00Z&eventType=LocationUpdate
```

### 5. Get Current User's Timeseries Data

**GET** `/api/telemetry/me/timeseries?startTime={start}&endTime={end}&eventType={type}`

**Authorization:** Requires authenticated user

**Example:**
```
GET /api/telemetry/me/timeseries?startTime=2025-10-27T20:00:00Z&endTime=2025-10-27T23:00:00Z
```

## Use Cases

### 1. **Real-time Location Tracking**

Track driver/ambulance location in real-time during trips:

```javascript
// Client-side: Collect location every 30 seconds
const locationUpdates = [];

setInterval(async () => {
  const position = await getCurrentPosition();
  locationUpdates.push({
    eventType: "LocationUpdate",
    eventDetails: `Trip ${tripId} location update`,
    telemetry: {
      latitude: position.coords.latitude,
      longitude: position.coords.longitude,
      speed: position.coords.speed,
      accuracy: position.coords.accuracy,
      batteryLevel: await getBatteryLevel(),
      timestamp: new Date().toISOString()
    }
  });
  
  // Send batch every 5 minutes or when 10 updates collected
  if (locationUpdates.length >= 10) {
    await sendBatchTelemetry(locationUpdates);
    locationUpdates.length = 0;
  }
}, 30000);
```

### 2. **Trip Route Visualization**

Retrieve and display the complete route taken during a trip:

```javascript
// Fetch all location updates for a trip
const response = await fetch(
  `/api/telemetry/timeseries`,
  {
    method: 'POST',
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      startTime: trip.startTime,
      endTime: trip.endTime,
      eventType: 'LocationUpdate',
      userId: trip.driverId,
      limit: 5000
    })
  }
);

const telemetryData = await response.json();

// Plot on map
const route = telemetryData.map(t => ({
  lat: t.latitude,
  lng: t.longitude,
  timestamp: t.eventTimestamp,
  speed: t.speed
}));

displayRouteOnMap(route);
```

### 3. **Performance Analytics**

Analyze driver behavior and system performance:

```javascript
// Get all telemetry for a driver over the past week
const weekAgo = new Date();
weekAgo.setDate(weekAgo.getDate() - 7);

const response = await fetch(
  `/api/telemetry/user/${driverId}/timeseries?` +
  `startTime=${weekAgo.toISOString()}&` +
  `endTime=${new Date().toISOString()}`,
  {
    headers: { 'Authorization': `Bearer ${token}` }
  }
);

const data = await response.json();

// Calculate metrics
const avgSpeed = data.reduce((sum, t) => sum + (t.speed || 0), 0) / data.length;
const batteryDrain = data[0].batteryLevel - data[data.length - 1].batteryLevel;
const totalDistance = calculateDistance(data);
```

### 4. **Offline Data Sync**

Collect telemetry offline and sync when connection is restored:

```javascript
// Store telemetry locally when offline
const offlineQueue = [];

async function logTelemetry(eventType, telemetryData) {
  const event = {
    eventType,
    telemetry: telemetryData,
    timestamp: new Date().toISOString()
  };
  
  if (navigator.onLine) {
    await sendToServer([event]);
  } else {
    offlineQueue.push(event);
    localStorage.setItem('telemetryQueue', JSON.stringify(offlineQueue));
  }
}

// Sync when online
window.addEventListener('online', async () => {
  const queue = JSON.parse(localStorage.getItem('telemetryQueue') || '[]');
  if (queue.length > 0) {
    await fetch('/api/telemetry/batch', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ events: queue })
    });
    localStorage.removeItem('telemetryQueue');
  }
});
```

## Data Model

### TelemetryDto (Full)
All available telemetry fields:

```csharp
public class TelemetryDto
{
    // Device Information
    public string? DeviceType { get; set; }        // "Mobile", "Desktop", "Tablet"
    public string? DeviceModel { get; set; }
    public string? OperatingSystem { get; set; }
    public string? OsVersion { get; set; }
    public string? Browser { get; set; }
    public string? BrowserVersion { get; set; }
    public string? AppVersion { get; set; }
    
    // Account Information
    public string? GoogleAccount { get; set; }
    public string? AppleAccount { get; set; }
    public string? AccountType { get; set; }
    
    // GPS/Location Information
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public double? Accuracy { get; set; }          // in meters
    public double? Altitude { get; set; }
    public double? Speed { get; set; }             // in m/s
    public double? Heading { get; set; }           // in degrees
    public DateTime? LocationTimestamp { get; set; }
    
    // Network Information
    public string? IpAddress { get; set; }
    public string? ConnectionType { get; set; }    // "wifi", "cellular", "ethernet"
    public bool? IsOnline { get; set; }
    
    // Screen Information
    public int? ScreenWidth { get; set; }
    public int? ScreenHeight { get; set; }
    public string? Orientation { get; set; }       // "portrait", "landscape"
    
    // Battery Information
    public double? BatteryLevel { get; set; }      // 0-1
    public bool? IsCharging { get; set; }
    
    // Timestamp
    public DateTime Timestamp { get; set; }
}
```

### TelemetryTimeseriesDto (Response)
Optimized response for timeseries queries:

```csharp
public class TelemetryTimeseriesDto
{
    public int Id { get; set; }
    public string EventType { get; set; }
    public string? EventDetails { get; set; }
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public double? Speed { get; set; }
    public double? BatteryLevel { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? EventTimestamp { get; set; }
}
```

## Event Types

Common event types for telemetry logging:

- **LocationUpdate**: Periodic location tracking
- **TripStart**: Trip started
- **TripEnd**: Trip completed
- **TripStatusUpdate**: Trip status changed
- **Login**: User logged in
- **Logout**: User logged out
- **Register**: New user registration
- **AppOpen**: Application opened
- **AppClose**: Application closed
- **NetworkChange**: Network connectivity changed
- **BatteryLow**: Battery level below threshold
- **ErrorOccurred**: Application error

## Performance Considerations

### Batch Size Recommendations

- **Real-time tracking**: Batch every 5-10 location updates (2.5-5 minutes at 30s intervals)
- **Background sync**: Batch up to 100 events
- **Offline sync**: Split large queues into batches of 100-200 events

### Query Limits

- Default limit: 1000 records
- Maximum recommended: 5000 records per query
- For larger datasets, use pagination with time windows

### Database Indexing

The `Telemetries` table should have indexes on:
- `CreatedAt` (for time-based queries)
- `UserId` (for user-specific queries)
- `EventType` (for event filtering)
- Composite index on `(CreatedAt, EventType)` for optimal timeseries queries

## Security & Privacy

### Authorization Rules

1. **Anonymous logging**: Allowed for single events (userId will be null)
2. **Batch logging**: Requires authentication
3. **Timeseries queries**: 
   - Users can only query their own data
   - Admin/Dispatcher can query all data
4. **Data retention**: Consider implementing automatic cleanup of old telemetry data

### Best Practices

1. **Don't log sensitive data**: Avoid logging passwords, tokens, or PII in event details
2. **Rate limiting**: Implement rate limits on batch endpoints to prevent abuse
3. **Data anonymization**: Consider anonymizing old telemetry data
4. **GDPR compliance**: Provide endpoints for users to request deletion of their telemetry data

## Integration Example

### Complete Client Implementation

```javascript
class TelemetryService {
  constructor(apiUrl, authToken) {
    this.apiUrl = apiUrl;
    this.authToken = authToken;
    this.queue = [];
    this.maxBatchSize = 10;
    this.flushInterval = 300000; // 5 minutes
    
    this.startAutoFlush();
  }
  
  async logEvent(eventType, telemetryData, eventDetails = null) {
    const event = {
      eventType,
      eventDetails,
      telemetry: {
        ...telemetryData,
        timestamp: new Date().toISOString()
      }
    };
    
    this.queue.push(event);
    
    if (this.queue.length >= this.maxBatchSize) {
      await this.flush();
    }
  }
  
  async flush() {
    if (this.queue.length === 0) return;
    
    const batch = [...this.queue];
    this.queue = [];
    
    try {
      await fetch(`${this.apiUrl}/api/telemetry/batch`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${this.authToken}`
        },
        body: JSON.stringify({ events: batch })
      });
    } catch (error) {
      console.error('Failed to send telemetry batch', error);
      // Re-queue failed events
      this.queue.unshift(...batch);
    }
  }
  
  startAutoFlush() {
    setInterval(() => this.flush(), this.flushInterval);
  }
  
  async getTimeseries(startTime, endTime, eventType = null) {
    const response = await fetch(`${this.apiUrl}/api/telemetry/me/timeseries?` +
      `startTime=${startTime.toISOString()}&` +
      `endTime=${endTime.toISOString()}` +
      (eventType ? `&eventType=${eventType}` : ''),
      {
        headers: { 'Authorization': `Bearer ${this.authToken}` }
      }
    );
    
    return await response.json();
  }
}

// Usage
const telemetry = new TelemetryService('https://api.example.com', authToken);

// Log location updates
navigator.geolocation.watchPosition(async (position) => {
  await telemetry.logEvent('LocationUpdate', {
    latitude: position.coords.latitude,
    longitude: position.coords.longitude,
    speed: position.coords.speed,
    accuracy: position.coords.accuracy,
    batteryLevel: await getBatteryLevel()
  });
});

// Query historical data
const lastWeek = new Date();
lastWeek.setDate(lastWeek.getDate() - 7);
const history = await telemetry.getTimeseries(lastWeek, new Date(), 'LocationUpdate');
```

## Summary

The timeseries telemetry logging system provides:

✅ **Efficient batch logging** for high-volume data  
✅ **Flexible time-based queries** for analytics  
✅ **Secure access controls** for privacy  
✅ **Offline sync support** for mobile apps  
✅ **Comprehensive tracking** of location, device, and system metrics  

This enables powerful features like route visualization, performance analytics, and real-time monitoring while maintaining user privacy and system performance.

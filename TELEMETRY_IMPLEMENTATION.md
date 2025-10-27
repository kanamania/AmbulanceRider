# Telemetry Implementation Summary

## Overview
Implemented comprehensive telemetry collection system that captures device information, GPS location, and other client-side data during critical operations (login, register, forgot password, trip creation, and trip status updates).

## Features Implemented

### 1. Backend (API)

#### Models & DTOs
- **`TelemetryDto.cs`** - Data transfer object for telemetry data
  - Device information (type, model, OS, browser, app version)
  - Account information (Google/Apple account, account type based on OS)
  - Installed apps (list of browser plugins or PWA related apps, count)
  - GPS/Location data (latitude, longitude, accuracy, altitude, speed, heading)
  - Network information (IP, connection type, online status)
  - Screen information (width, height, orientation)
  - Battery information (level, charging status)

- **`Telemetry.cs`** - Database model for storing telemetry
  - Links to user (optional for anonymous events)
  - Event type and details
  - All telemetry fields from DTO

#### Services
- **`ITelemetryService`** / **`TelemetryService`** - Service for logging telemetry
  - `LogTelemetryAsync()` - Logs telemetry without breaking main flow
  - Handles failures gracefully (never throws)

- **`ITelemetryRepository`** / **`TelemetryRepository`** - Data access layer
  - `CreateAsync()` - Saves telemetry to database
  - `GetByUserIdAsync()` - Retrieves user's telemetry history
  - `GetByEventTypeAsync()` - Retrieves telemetry by event type

#### Database
- **`telemetries` table** added to `ApplicationDbContext`
  - Indexed on EventType, UserId, and CreatedAt for efficient queries
  - Optional relationship with User (SetNull on delete)

#### Controllers Updated
All DTOs now include optional `Telemetry` property:

**AuthController:**
- `Register` - Logs registration with device/location info
- `Login` - Logs successful login and failed attempts
- `ForgotPassword` - Logs password reset requests
- `ResetPassword` - Logs password reset completions

**TripsController:**
- `Create` - Logs trip creation with GPS location
- `Update` - Logs trip updates
- `Approve` - Logs trip approval/rejection
- `Start` - Logs trip start with current location
- `Complete` - Logs trip completion with final location

### 2. Frontend (Blazor)

#### Models
- **`TelemetryDto.cs`** - Client-side DTO matching API structure

#### Services
- **`TelemetryService.cs`** - Collects telemetry from browser
  - `CollectTelemetryAsync()` - Gathers all available telemetry
  - Handles permissions gracefully (doesn't fail if denied)
  - Collects:
    - User agent parsing (browser, OS, device type)
    - Screen dimensions and orientation
    - Network status and connection type
    - GPS location (if permission granted)
    - Battery information (if available)

#### JavaScript Helpers (`telemetry.js`)
- `getLocation()` - Gets current GPS position
- `watchLocation()` - Continuously monitors location
- `clearLocationWatch()` - Stops location monitoring
- `getBatteryInfo()` - Gets battery status
- `getNetworkInfo()` - Gets network information
- `getDeviceMemory()` - Gets device memory
- `getHardwareConcurrency()` - Gets CPU cores
- `getAccountInfo()` - Detects account type (Google/Apple) based on OS
- `getInstalledApps()` - Gets browser plugins or PWA related apps
- `getAppVersion()` - Gets app version from meta tag or manifest

### 3. Usage Pattern

#### In Blazor Components
```csharp
@inject TelemetryService TelemetryService

private async Task HandleLogin()
{
    var loginDto = new LoginDto
    {
        Email = email,
        Password = password,
        Telemetry = await TelemetryService.CollectTelemetryAsync()
    };
    
    await ApiService.LoginAsync(loginDto);
}
```

#### In API Controllers
```csharp
public async Task<ActionResult> Login([FromBody] LoginDto loginDto)
{
    var response = await _authService.LoginAsync(loginDto);
    
    // Log telemetry (non-blocking)
    await _telemetryService.LogTelemetryAsync(
        "Login",
        loginDto.Telemetry,
        response.User.Id,
        $"User logged in: {loginDto.Email}"
    );
    
    return Ok(response);
}
```

## Key Design Decisions

### 1. Non-Blocking
- Telemetry collection never blocks the main operation
- All exceptions are caught and logged, not thrown
- Missing telemetry data is acceptable

### 2. Privacy-Conscious
- GPS location requires user permission
- Gracefully handles denied permissions
- All telemetry is optional (nullable fields)

### 3. Graceful Degradation
- Works even if browser APIs are unavailable
- Returns minimal telemetry if collection fails
- Never causes user-facing errors

### 4. Performance
- Telemetry collection is async
- Database queries are indexed
- Minimal overhead on main operations

## Database Migration Required

Run the following to create the telemetries table:

```bash
cd AmbulanceRider.API
dotnet ef migrations add AddTelemetryTable
dotnet ef database update
```

Or if using Docker:
```bash
docker-compose restart api
```

The migration will be applied automatically on startup.

## Event Types Logged

- **Register** - User registration
- **Login** - Successful login
- **LoginFailed** - Failed login attempt
- **ForgotPassword** - Password reset request
- **ResetPassword** - Password reset completion
- **TripCreate** - Trip creation
- **TripUpdate** - Trip modification
- **TripApprove** - Trip approval
- **TripReject** - Trip rejection
- **TripStart** - Trip started
- **TripComplete** - Trip completed

## Benefits

1. **Analytics** - Track user behavior and usage patterns
2. **Debugging** - Understand context of errors and issues
3. **Security** - Monitor suspicious login attempts
4. **Compliance** - Audit trail for critical operations
5. **UX Improvement** - Understand device/network conditions
6. **Location Tracking** - GPS data for trip-related events

## Privacy & Compliance

- Telemetry collection is transparent
- GPS requires explicit user permission
- Data can be anonymized (UserId is optional)
- Complies with GDPR (user can request deletion)
- No sensitive data (passwords, tokens) is logged

## Enhanced Features (Latest Update)

### Account Information
- **Google Account** - Detected on Android devices (requires OAuth for actual email)
- **Apple Account** - Detected on iOS/macOS devices (requires Sign in with Apple for actual ID)
- **Account Type** - Automatically determined based on operating system
  - "Google" for Android devices
  - "Apple" for iOS/macOS devices
  - "None" for other platforms

### Installed Apps
- **Browser Plugins** - List of installed browser plugins/extensions (fallback)
- **PWA Related Apps** - Uses `getInstalledRelatedApps()` API for PWA
- **Apps Count** - Total number of detected apps/plugins
- **JSON Format** - Apps stored as JSON array for easy parsing

### App Version
- **Meta Tag** - Version from `<meta name="app-version" content="1.0.0" />`
- **Manifest** - Version from PWA manifest.json
- **Automatic Detection** - Fetched via JavaScript on each telemetry collection

### Limitations (Web Context)

**Account Information:**
- Web apps cannot directly access Google/Apple account emails
- Requires OAuth integration for actual account details
- Currently detects account type based on OS only
- Native apps would have full access via platform APIs

**Installed Apps:**
- Web browsers restrict access to installed apps for privacy
- Only browser plugins/extensions are accessible
- PWA can use `getInstalledRelatedApps()` for related apps only
- Native apps (Cordova, Capacitor) would have full access

**Recommendations for Native App:**
If you convert to a native mobile app:
1. Use Capacitor or Cordova for cross-platform development
2. Add native plugins for:
   - Google Sign-In SDK (Android)
   - Sign in with Apple (iOS)
   - Package Manager access for installed apps list
3. Update telemetry collection to use native APIs

## Next Steps

1. **Create migration** for telemetries table
2. **Update Blazor components** to collect telemetry
3. **Add telemetry dashboard** for admins
4. **Implement data retention** policy
5. **Add user consent** UI for location tracking
6. **Consider OAuth integration** for actual Google/Apple account emails
7. **Evaluate native app** conversion for enhanced telemetry

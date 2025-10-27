# Telemetry Enhancements - Account & Apps Tracking

## Summary
Enhanced the telemetry system to capture Google/Apple account information, installed apps list, and app version information.

## New Fields Added

### 1. Account Information
- **`GoogleAccount`** (string?) - Google account email (Android)
- **`AppleAccount`** (string?) - Apple ID (iOS/macOS)
- **`AccountType`** (string?) - Account type: "Google", "Apple", or "None"

### 2. Installed Apps
- **`InstalledApps`** (string?) - JSON array of installed apps/plugins
- **`InstalledAppsCount`** (int?) - Total count of apps/plugins

### 3. App Version
- **`AppVersion`** (string?) - Application version (e.g., "1.0.0")

## Implementation Details

### Backend Changes

#### 1. Updated DTOs
**File:** `AmbulanceRider.API/DTOs/TelemetryDto.cs`
```csharp
// Account Information
public string? GoogleAccount { get; set; }
public string? AppleAccount { get; set; }
public string? AccountType { get; set; }

// Installed Apps
public string? InstalledApps { get; set; }
public int? InstalledAppsCount { get; set; }
```

#### 2. Updated Database Model
**File:** `AmbulanceRider.API/Models/Telemetry.cs`
- Added same fields as DTO
- Configured in `ApplicationDbContext.cs` with appropriate max lengths

#### 3. Updated Service
**File:** `AmbulanceRider.API/Services/TelemetryService.cs`
- Maps new fields from DTO to database model

### Frontend Changes

#### 1. Updated Client DTO
**File:** `AmbulanceRider/Models/TelemetryDto.cs`
- Added same fields to match API DTO

#### 2. Enhanced TelemetryService
**File:** `AmbulanceRider/Services/TelemetryService.cs`

**New Methods:**
- `GetAccountInfoAsync()` - Detects account type based on OS
- `GetInstalledAppsAsync()` - Retrieves browser plugins or PWA apps
- App version detection from meta tag or manifest

**Collection Logic:**
```csharp
// Account detection
if (OS == "Android") -> AccountType = "Google"
if (OS == "iOS" || "macOS") -> AccountType = "Apple"
else -> AccountType = "None"

// Apps collection
1. Try getInstalledRelatedApps() for PWA
2. Fallback to navigator.plugins for browser
3. Return JSON array + count
```

#### 3. JavaScript Helpers
**File:** `wwwroot/js/telemetry.js`

**New Functions:**
```javascript
// Detects account type based on user agent
window.getAccountInfo = async function()

// Gets browser plugins or PWA related apps
window.getInstalledApps = async function()

// Gets app version from meta tag or manifest
window.getAppVersion = function()
```

#### 4. HTML Updates
**File:** `wwwroot/index.html`
```html
<meta name="app-version" content="1.0.0" />
```

**File:** `wwwroot/manifest.json`
```json
"version": "1.0.0"
```

## How It Works

### Account Detection Flow
1. JavaScript checks user agent
2. Determines OS (Android, iOS, macOS, etc.)
3. Sets account type accordingly
4. Returns to C# service
5. Stored in telemetry DTO

### Apps Collection Flow
1. JavaScript tries `navigator.getInstalledRelatedApps()` (PWA)
2. If unavailable, falls back to `navigator.plugins`
3. Creates JSON array of app/plugin names
4. Returns array + count
5. Stored in telemetry DTO

### Version Detection Flow
1. JavaScript checks for `<meta name="app-version">`
2. If not found, checks manifest.json
3. Returns version string
4. Stored in telemetry DTO

## Web Browser Limitations

### Account Information
❌ **Cannot access actual email/ID** without OAuth
✅ **Can detect account type** based on OS

**Why:**
- Privacy and security restrictions
- Requires explicit user consent via OAuth
- Web APIs don't expose account information

**Solution for Full Access:**
- Implement Google Sign-In OAuth
- Implement Sign in with Apple
- User must explicitly authorize

### Installed Apps
❌ **Cannot access system-level installed apps**
✅ **Can access browser plugins/extensions**
✅ **Can access PWA related apps** (limited)

**Why:**
- Privacy restrictions in web browsers
- Security sandboxing
- Platform limitations

**Solution for Full Access:**
- Convert to native app (Capacitor/Cordova)
- Use native plugins for package manager access
- Requires platform-specific permissions

## Data Examples

### Account Information
```json
{
  "accountType": "Google",
  "googleAccount": null,  // Requires OAuth
  "appleAccount": null
}
```

### Installed Apps (Browser Plugins)
```json
{
  "installedApps": "[\"Chrome PDF Plugin\",\"Native Client\",\"Widevine Content Decryption Module\"]",
  "installedAppsCount": 3
}
```

### Installed Apps (PWA Related)
```json
{
  "installedApps": "[{\"platform\":\"webapp\",\"url\":\"https://example.com\",\"id\":\"com.example.app\"}]",
  "installedAppsCount": 1
}
```

## Database Migration Required

After pulling these changes, run:

```bash
cd AmbulanceRider.API
dotnet ef migrations add AddAccountAndAppsToTelemetry
dotnet ef database update
```

Or with Docker:
```bash
docker-compose restart api
```

## Privacy Considerations

### What We Collect
✅ Account type (Google/Apple/None) - Based on OS detection
✅ Browser plugins list - Limited to browser extensions
✅ App version - Public information from manifest

### What We DON'T Collect
❌ Actual email addresses (requires OAuth consent)
❌ System-level installed apps (not accessible in web)
❌ Personal account information

### User Privacy
- All data collection is transparent
- No PII without explicit consent
- Account type is inferred, not accessed
- Apps list is limited to browser context
- Complies with privacy regulations

## Future Enhancements

### For OAuth Integration
1. Add Google Sign-In button
2. Request email scope
3. Store email in `GoogleAccount` field
4. Same for Apple Sign-In

### For Native App
1. Use Capacitor or Cordova
2. Add native plugins:
   - `@capacitor/device` for device info
   - `cordova-plugin-app-list` for installed apps
   - Platform-specific account APIs
3. Full access to device capabilities

## Testing

### Test Account Detection
1. Open app on Android device/emulator
2. Check telemetry: `accountType` should be "Google"
3. Open app on iOS device/simulator
4. Check telemetry: `accountType` should be "Apple"

### Test Apps Collection
1. Open app in Chrome with extensions installed
2. Check telemetry: `installedApps` should list plugins
3. Install as PWA
4. Check telemetry: May show related apps

### Test Version
1. Check `<meta name="app-version">` in HTML
2. Telemetry should show "1.0.0"
3. Update version in meta tag
4. Telemetry should reflect new version

## Files Modified

### Backend
- ✅ `AmbulanceRider.API/DTOs/TelemetryDto.cs`
- ✅ `AmbulanceRider.API/Models/Telemetry.cs`
- ✅ `AmbulanceRider.API/Data/ApplicationDbContext.cs`
- ✅ `AmbulanceRider.API/Services/TelemetryService.cs`

### Frontend
- ✅ `AmbulanceRider/Models/TelemetryDto.cs`
- ✅ `AmbulanceRider/Services/TelemetryService.cs`
- ✅ `AmbulanceRider/wwwroot/js/telemetry.js`
- ✅ `AmbulanceRider/wwwroot/index.html`
- ✅ `AmbulanceRider/wwwroot/manifest.json`

### Documentation
- ✅ `TELEMETRY_IMPLEMENTATION.md` (updated)
- ✅ `TELEMETRY_ENHANCEMENTS.md` (new)

## Summary

Successfully enhanced telemetry system with:
- ✅ Account type detection (Google/Apple)
- ✅ Installed apps tracking (browser plugins/PWA)
- ✅ App version tracking
- ✅ Privacy-conscious implementation
- ✅ Web browser limitations documented
- ✅ Future enhancement path defined

All changes are backward compatible and non-breaking. Telemetry collection gracefully handles missing data.

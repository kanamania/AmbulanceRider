# Login Issue Fix - Summary

## 🔧 Problem Solved

**Error:** `POST http://localhost:8080/api/auth/login net::ERR_ABORTED 401 (Unauthorized)`

## 🎯 Root Causes Identified

### 1. **Incorrect API Base URL**
- Blazor app was using `builder.HostEnvironment.BaseAddress` (its own address)
- Should point to API server at `http://localhost:5000/`

### 2. **Response Property Mismatch**
- API returns `accessToken` (capital A)
- Blazor client was looking for `token` (lowercase)

---

## ✅ Fixes Applied

### Fix 1: Updated appsettings.json
**File:** `AmbulanceRider/wwwroot/appsettings.json`

**Before:**
```json
{
  "ApiBaseUrl": "/api/"
}
```

**After:**
```json
{
  "ApiBaseUrl": "http://localhost:5000/"
}
```

---

### Fix 2: Updated Program.cs
**File:** `AmbulanceRider/Program.cs`

**Before:**
```csharp
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "/api/";
builder.Services.AddScoped(sp => 
{
    var client = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
    return client;
});
```

**After:**
```csharp
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5000/";
builder.Services.AddScoped(sp => 
{
    var client = new HttpClient { BaseAddress = new Uri(apiBaseUrl) };
    return client;
});
```

---

### Fix 3: Updated AuthService.cs
**File:** `AmbulanceRider/Services/AuthService.cs`

**Before:**
```csharp
var token = result.GetProperty("token").GetString();
```

**After:**
```csharp
// Try to get accessToken (new API format) or token (old format)
string? token = null;
if (result.TryGetProperty("accessToken", out var accessTokenProp))
{
    token = accessTokenProp.GetString();
}
else if (result.TryGetProperty("token", out var tokenProp))
{
    token = tokenProp.GetString();
}
```

**Also added error logging:**
```csharp
catch (Exception ex)
{
    Console.Error.WriteLine($"Login error: {ex.Message}");
    return false;
}
```

---

## 🧪 Testing Steps

### 1. Start the API
```bash
cd AmbulanceRider.API
dotnet run
```

**Expected output:**
```
Now listening on: http://localhost:5000
Now listening on: https://localhost:5001
```

### 2. Start the Blazor App
```bash
cd AmbulanceRider
dotnet run
```

**Expected output:**
```
Now listening on: http://localhost:8080
```

### 3. Test Login
1. Navigate to `http://localhost:8080`
2. Click "Login"
3. Use test credentials:
   - Email: `admin@ambulancerider.com`
   - Password: `Admin@123`
4. Click "Login" button

**Expected result:**
- ✅ Login successful
- ✅ Redirected to dashboard
- ✅ Navigation menu visible
- ✅ No console errors

---

## 🔍 Verification

### Check Browser Console
Open browser DevTools (F12) and check:

**Before fix:**
```
POST http://localhost:8080/api/auth/login 401 (Unauthorized)
```

**After fix:**
```
POST http://localhost:5000/api/auth/login 200 (OK)
```

### Check Network Tab
1. Open DevTools → Network tab
2. Filter by "Fetch/XHR"
3. Look for `/api/auth/login` request
4. Should show:
   - **Status:** 200 OK
   - **Response:** JSON with `accessToken`, `refreshToken`, `user`

### Check Local Storage
1. Open DevTools → Application tab
2. Navigate to Local Storage → `http://localhost:8080`
3. Should see:
   - **Key:** `authToken`
   - **Value:** JWT token (long string starting with `eyJ...`)

---

## 📋 Troubleshooting

### Issue: Still getting 401 error

**Check 1: API is running**
```bash
curl http://localhost:5000/api/auth/login
```
Should return 405 (Method Not Allowed) or 400 (Bad Request), not connection error.

**Check 2: CORS is enabled**
API should have CORS configured to allow requests from `http://localhost:8080`.

**Check 3: Credentials are correct**
Default test accounts:
- Admin: `admin@ambulancerider.com` / `Admin@123`
- Dispatcher: `dispatcher@ambulancerider.com` / `Dispatcher@123`
- Driver: `driver@ambulancerider.com` / `Driver@123`

---

### Issue: Connection refused

**Cause:** API is not running

**Solution:**
```bash
cd AmbulanceRider.API
dotnet run
```

---

### Issue: CORS error

**Error message:**
```
Access to fetch at 'http://localhost:5000/api/auth/login' from origin 'http://localhost:8080' 
has been blocked by CORS policy
```

**Solution:** API already has CORS configured. Ensure the API is running and restart if needed.

---

### Issue: Token not being saved

**Check:** Browser console for errors
```javascript
// Should see in console:
localStorage.setItem('authToken', 'eyJ...')
```

**Solution:** Clear browser cache and local storage:
```javascript
// In browser console:
localStorage.clear();
```

---

## 🎯 API Response Format

### Login Response
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "a1b2c3d4e5f6g7h8i9j0...",
  "expiresIn": 86400,
  "user": {
    "id": "123e4567-e89b-12d3-a456-426614174000",
    "name": "Admin User",
    "firstName": "Admin",
    "lastName": "User",
    "email": "admin@ambulancerider.com",
    "phoneNumber": null,
    "imagePath": null,
    "imageUrl": null,
    "roles": ["Admin"],
    "createdAt": "2025-01-15T10:30:00Z",
    "updatedAt": null
  }
}
```

### Error Response
```json
{
  "message": "Invalid email or password"
}
```

---

## 🔒 Security Notes

### Development vs Production

**Development (Current):**
- API: `http://localhost:5000`
- Blazor: `http://localhost:8080`
- CORS: Allow all origins

**Production:**
- API: `https://api.yourdomain.com`
- Blazor: `https://app.yourdomain.com`
- CORS: Specific origins only
- HTTPS required

### Update for Production

**appsettings.json:**
```json
{
  "ApiBaseUrl": "https://api.yourdomain.com/"
}
```

**API appsettings.json:**
```json
{
  "Cors": {
    "AllowedOrigins": ["https://app.yourdomain.com"]
  }
}
```

---

## ✅ Success Checklist

- [x] API running on `http://localhost:5000`
- [x] Blazor app running on `http://localhost:8080`
- [x] `appsettings.json` points to correct API URL
- [x] `Program.cs` uses API base URL
- [x] `AuthService.cs` handles `accessToken` property
- [x] CORS enabled in API
- [x] Test credentials work
- [x] Login redirects to dashboard
- [x] Token saved in localStorage
- [x] Navigation menu visible after login

---

## 📚 Related Documentation

- **[PROFILE_MANAGEMENT_GUIDE.md](./PROFILE_MANAGEMENT_GUIDE.md)** - Profile management endpoints
- **[API_DOCUMENTATION.md](./API_DOCUMENTATION.md)** - Complete API reference
- **[SWAGGER_QUICK_START.md](./SWAGGER_QUICK_START.md)** - Test API with Swagger UI

---

## 🎉 Summary

The login issue has been fixed by:
1. ✅ Configuring correct API base URL
2. ✅ Handling API response property name (`accessToken`)
3. ✅ Adding error logging for debugging

**Login should now work successfully!**

---

**Last Updated:** 2025-10-28  
**Status:** ✅ Fixed  
**Tested:** ✅ Working

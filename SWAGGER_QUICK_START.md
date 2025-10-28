# Swagger Documentation - Quick Start Guide

## 🚀 Getting Started in 5 Minutes

### Step 1: Start the API (30 seconds)
```bash
cd AmbulanceRider.API
dotnet run
```

### Step 2: Open Swagger UI (10 seconds)
Open your browser and navigate to:
- **HTTP**: http://localhost:5000
- **HTTPS**: https://localhost:5001

The Swagger UI will load automatically at the root URL.

### Step 3: Authenticate (2 minutes)

1. **Scroll to Authentication section** or use the filter to search for "login"

2. **Expand** `POST /api/auth/login`

3. **Click** "Try it out" button

4. **Use test credentials**:
   ```json
   {
     "email": "admin@ambulancerider.com",
     "password": "Admin@123"
   }
   ```

5. **Click** "Execute"

6. **Copy the accessToken** from the response (looks like: `eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`)

7. **Click the green "Authorize" button** at the top of the page

8. **Enter**: `Bearer {paste-your-token-here}`
   - Example: `Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`

9. **Click** "Authorize" then "Close"

✅ **You're now authenticated!** All requests will include your JWT token.

### Step 4: Test an Endpoint (1 minute)

1. **Navigate to** `GET /api/trips`

2. **Click** "Try it out"

3. **Click** "Execute"

4. **View the response** - you should see a list of trips

## 🎯 Common Tasks

### Create a New Trip
1. Go to `POST /api/trips`
2. Click "Try it out"
3. Modify the example JSON:
   ```json
   {
     "pickupLocationId": 1,
     "dropoffLocationId": 2,
     "vehicleId": 1,
     "patientName": "John Doe",
     "patientAge": 45,
     "patientGender": "Male",
     "emergencyLevel": "High",
     "notes": "Chest pain, difficulty breathing"
   }
   ```
4. Click "Execute"
5. Check the response for the created trip

### View Analytics Dashboard
1. Go to `GET /api/analytics/dashboard`
2. Click "Try it out"
3. Optionally set date range parameters
4. Click "Execute"
5. View comprehensive statistics

### Update Trip Status
1. Go to `PUT /api/trips/{id}/status`
2. Click "Try it out"
3. Enter trip ID (e.g., `1`)
4. Set status in request body:
   ```json
   {
     "status": "InProgress",
     "notes": "Driver en route to pickup location"
   }
   ```
5. Click "Execute"

## 📋 Available Test Accounts

| Role | Email | Password | Access Level |
|------|-------|----------|--------------|
| **Admin** | admin@ambulancerider.com | Admin@123 | Full system access |
| **Dispatcher** | dispatcher@ambulancerider.com | Dispatcher@123 | Manage trips & assignments |
| **Driver** | driver@ambulancerider.com | Driver@123 | Accept & complete trips |
| **User** | user@ambulancerider.com | User@123 | Request services |

## 🔍 Key Features

### Filter & Search
- Use the **Filter** box at the top to search for specific endpoints
- Example: Type "trip" to see all trip-related endpoints

### Collapsible Sections
- Click on section headers (e.g., "Authentication", "Trips") to expand/collapse
- Keeps the UI organized when working with specific features

### Request Duration
- After executing a request, see how long it took
- Useful for performance monitoring

### Response Examples
- Each endpoint shows example responses for all status codes
- Click on response codes (200, 400, 401, etc.) to see examples

### Schema Documentation
- Scroll to bottom to see "Schemas" section
- Click any model to see its structure
- Shows required fields, data types, and examples

## 🎨 UI Tips

### Color Coding
- **Green**: POST (Create)
- **Blue**: GET (Read)
- **Orange**: PUT (Update)
- **Red**: DELETE (Remove)
- **Teal**: PATCH (Partial Update)

### Required Fields
- Fields marked with `*` are required
- Example: `email *` means email is required

### Authorization Lock
- 🔒 **Locked**: Endpoint requires authentication
- Click "Authorize" button to unlock

## ⚡ Keyboard Shortcuts

- **Ctrl/Cmd + F**: Search in browser (filter endpoints)
- **Tab**: Navigate between fields in forms
- **Enter**: Submit forms when focused on input

## 🔧 Troubleshooting

### "401 Unauthorized" Error
**Problem**: Token expired or not set
**Solution**: 
1. Login again to get new token
2. Click "Authorize" and enter new token

### "400 Bad Request" Error
**Problem**: Invalid input data
**Solution**: 
1. Check the example in Swagger
2. Ensure all required fields are provided
3. Verify data types match (e.g., numbers vs strings)

### Swagger UI Not Loading
**Problem**: API not running or wrong URL
**Solution**:
1. Ensure API is running (`dotnet run`)
2. Check console for correct port
3. Try both HTTP and HTTPS URLs

### Can't See Response
**Problem**: Response collapsed or hidden
**Solution**:
1. Scroll down after clicking "Execute"
2. Look for "Response body" section
3. Click "Download" if response is large

## 📚 Next Steps

### Export OpenAPI Spec
1. Click on the link at top: `/swagger/v1/swagger.json`
2. Save the JSON file
3. Import into Postman, Insomnia, or other API tools

### Generate Client Code
Use the OpenAPI spec to generate client libraries:
```bash
# TypeScript/JavaScript
npx @openapitools/openapi-generator-cli generate \
  -i http://localhost:5000/swagger/v1/swagger.json \
  -g typescript-axios \
  -o ./api-client

# C#
dotnet tool install --global dotnet-openapi
dotnet openapi add url http://localhost:5000/swagger/v1/swagger.json
```

### Explore SignalR Hubs
Real-time features are available via SignalR:
- **Trip Updates**: `/hubs/trips`
- **Notifications**: `/hubs/notifications`

See the main documentation for SignalR integration examples.

## 🆘 Need Help?

- **Full Documentation**: See `SWAGGER_DOCUMENTATION.md`
- **API Details**: See `API_DOCUMENTATION.md`
- **Implementation Guide**: See `IMPLEMENTATION_SUMMARY.md`
- **Support**: support@ambulancerider.com

## ✅ Checklist

- [ ] API is running
- [ ] Swagger UI loaded successfully
- [ ] Logged in with test account
- [ ] Copied access token
- [ ] Clicked "Authorize" button
- [ ] Entered token with "Bearer " prefix
- [ ] Successfully tested an endpoint
- [ ] Explored different sections
- [ ] Checked response schemas

**Happy Testing! 🚑**

# Swagger UI - Visual Guide

## 🎨 What You'll See

This guide shows you what to expect when you open the Swagger UI for the AmbulanceRider API.

---

## 📱 Main Interface

### Header Section
```
┌─────────────────────────────────────────────────────────────┐
│  🚑 AmbulanceRider API                    [Authorize] 🔓    │
│  Version 1.0.0                                               │
└─────────────────────────────────────────────────────────────┘
```

**Features:**
- **Title**: AmbulanceRider API (in red)
- **Version**: v1.0.0
- **Authorize Button**: Green button in top-right (click to authenticate)

---

## 📋 API Overview Section

```
┌─────────────────────────────────────────────────────────────┐
│  AmbulanceRider Emergency Medical Dispatch System API       │
│                                                              │
│  Overview                                                    │
│  This API provides comprehensive endpoints for managing     │
│  emergency medical dispatch operations including:           │
│  • Authentication & Authorization                            │
│  • Trip Management                                           │
│  • Vehicle Management                                        │
│  • Location Services                                         │
│  • Analytics                                                 │
│  • Telemetry                                                 │
│  • Notifications                                             │
│                                                              │
│  Authentication                                              │
│  Most endpoints require JWT Bearer token authentication...   │
│                                                              │
│  [Filter by tags] 🔍                                        │
└─────────────────────────────────────────────────────────────┘
```

---

## 🔐 Authentication Section

### Collapsed View
```
┌─────────────────────────────────────────────────────────────┐
│  ▶ Authentication                                            │
│     Manage user authentication, registration, and password   │
│     operations                                               │
└─────────────────────────────────────────────────────────────┘
```

### Expanded View
```
┌─────────────────────────────────────────────────────────────┐
│  ▼ Authentication                                            │
│                                                              │
│  POST /api/auth/register                                     │
│  ├─ Register a new user                                      │
│  └─ [Try it out]                                            │
│                                                              │
│  POST /api/auth/login                                        │
│  ├─ User login                                               │
│  └─ [Try it out]                                            │
│                                                              │
│  POST /api/auth/refresh                                      │
│  ├─ Refresh access token                                     │
│  └─ [Try it out]                                            │
│                                                              │
│  GET /api/auth/me                                      🔒    │
│  ├─ Get current user                                         │
│  └─ [Try it out]                                            │
│                                                              │
│  POST /api/auth/logout                                 🔒    │
│  ├─ User logout                                              │
│  └─ [Try it out]                                            │
│                                                              │
│  POST /api/auth/forgot-password                              │
│  ├─ Forgot password                                          │
│  └─ [Try it out]                                            │
│                                                              │
│  POST /api/auth/reset-password                               │
│  ├─ Reset password                                           │
│  └─ [Try it out]                                            │
└─────────────────────────────────────────────────────────────┘
```

**Color Coding:**
- 🟢 **POST** endpoints: Green background
- 🔵 **GET** endpoints: Blue background
- 🟠 **PUT** endpoints: Orange background
- 🔴 **DELETE** endpoints: Red background
- 🔒 **Lock icon**: Requires authentication

---

## 🧪 Testing an Endpoint

### Step 1: Click "Try it out"
```
┌─────────────────────────────────────────────────────────────┐
│  POST /api/auth/login                                        │
│  User login                                                  │
│                                                              │
│  Authenticate with email and password to receive JWT tokens  │
│                                                              │
│  Default Test Accounts:                                      │
│  • Admin: admin@ambulancerider.com / Admin@123              │
│  • Dispatcher: dispatcher@ambulancerider.com / Dispatcher@123│
│  • Driver: driver@ambulancerider.com / Driver@123           │
│                                                              │
│  [Try it out]  [Cancel]                                     │
└─────────────────────────────────────────────────────────────┘
```

### Step 2: Edit Request Body
```
┌─────────────────────────────────────────────────────────────┐
│  Request body                                          * required│
│                                                              │
│  ┌──────────────────────────────────────────────────────┐  │
│  │ {                                                     │  │
│  │   "email": "admin@ambulancerider.com",               │  │
│  │   "password": "Admin@123",                           │  │
│  │   "telemetry": {                                     │  │
│  │     "latitude": -1.2921,                             │  │
│  │     "longitude": 36.8219,                            │  │
│  │     "accuracy": 10.5,                                │  │
│  │     "deviceInfo": "Mozilla/5.0"                      │  │
│  │   }                                                   │  │
│  │ }                                                     │  │
│  └──────────────────────────────────────────────────────┘  │
│                                                              │
│  [Execute]                                                   │
└─────────────────────────────────────────────────────────────┘
```

### Step 3: View Response
```
┌─────────────────────────────────────────────────────────────┐
│  Responses                                                   │
│                                                              │
│  Server response                                             │
│  Code: 200    Duration: 245ms                               │
│                                                              │
│  Response body                                               │
│  ┌──────────────────────────────────────────────────────┐  │
│  │ {                                                     │  │
│  │   "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6...", │  │
│  │   "refreshToken": "a1b2c3d4e5f6...",                 │  │
│  │   "expiresIn": 86400,                                │  │
│  │   "user": {                                           │  │
│  │     "id": "123e4567-e89b-12d3-a456-426614174000",   │  │
│  │     "email": "admin@ambulancerider.com",             │  │
│  │     "firstName": "Admin",                            │  │
│  │     "lastName": "User",                              │  │
│  │     "role": "Admin"                                  │  │
│  │   }                                                   │  │
│  │ }                                                     │  │
│  └──────────────────────────────────────────────────────┘  │
│                                                              │
│  Response headers                                            │
│  content-type: application/json; charset=utf-8              │
│  date: Tue, 28 Oct 2025 15:48:00 GMT                       │
│  server: Kestrel                                             │
│                                                              │
│  [Download] 📥                                              │
└─────────────────────────────────────────────────────────────┘
```

---

## 🔓 Authorization Dialog

### Click the "Authorize" Button
```
┌─────────────────────────────────────────────────────────────┐
│  Available authorizations                              ✕     │
│                                                              │
│  Bearer (apiKey)                                             │
│                                                              │
│  JWT Authorization header using the Bearer scheme.           │
│  Enter 'Bearer' [space] and then your token in the text     │
│  input below.                                                │
│                                                              │
│  Example: 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...'  │
│                                                              │
│  Value: *                                                    │
│  ┌──────────────────────────────────────────────────────┐  │
│  │ Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...       │  │
│  └──────────────────────────────────────────────────────┘  │
│                                                              │
│  [Authorize]  [Close]                                       │
└─────────────────────────────────────────────────────────────┘
```

### After Authorization
```
┌─────────────────────────────────────────────────────────────┐
│  🚑 AmbulanceRider API                    [Authorize] 🔒    │
│  Version 1.0.0                            [Logout]          │
└─────────────────────────────────────────────────────────────┘
```
**Note:** Lock icon changes from 🔓 to 🔒 when authenticated

---

## 🚑 Trips Section

```
┌─────────────────────────────────────────────────────────────┐
│  ▼ Trips                                                     │
│     Manage emergency trips and their lifecycle               │
│                                                              │
│  GET /api/trips                                        🔒    │
│  ├─ Get all trips                                            │
│  └─ [Try it out]                                            │
│                                                              │
│  POST /api/trips                                       🔒    │
│  ├─ Create a new trip                                        │
│  └─ [Try it out]                                            │
│                                                              │
│  GET /api/trips/{id}                                   🔒    │
│  ├─ Get trip by ID                                           │
│  └─ [Try it out]                                            │
│                                                              │
│  PUT /api/trips/{id}                                   🔒    │
│  ├─ Update trip                                              │
│  └─ [Try it out]                                            │
│                                                              │
│  DELETE /api/trips/{id}                                🔒    │
│  ├─ Delete trip                                              │
│  └─ [Try it out]                                            │
│                                                              │
│  GET /api/trips/status/{status}                        🔒    │
│  ├─ Get trips by status                                      │
│  └─ [Try it out]                                            │
│                                                              │
│  POST /api/trips/{id}/approve                          🔒    │
│  ├─ Approve trip                                             │
│  └─ [Try it out]                                            │
│                                                              │
│  POST /api/trips/{id}/start                            🔒    │
│  ├─ Start trip                                               │
│  └─ [Try it out]                                            │
│                                                              │
│  POST /api/trips/{id}/complete                         🔒    │
│  ├─ Complete trip                                            │
│  └─ [Try it out]                                            │
└─────────────────────────────────────────────────────────────┘
```

---

## 📊 Response Codes

### Success Responses
```
┌─────────────────────────────────────────────────────────────┐
│  Responses                                                   │
│                                                              │
│  ▼ 200  Successful Response                                 │
│     Description: Request successful                          │
│     Media type: application/json                             │
│     Example Value | Schema                                   │
│                                                              │
│  ▼ 201  Created                                             │
│     Description: Resource created successfully               │
│     Media type: application/json                             │
│     Example Value | Schema                                   │
└─────────────────────────────────────────────────────────────┘
```

### Error Responses
```
┌─────────────────────────────────────────────────────────────┐
│  ▼ 400  Bad Request                                         │
│     Description: Invalid input data                          │
│                                                              │
│  ▼ 401  Unauthorized                                        │
│     Description: Missing or invalid authentication           │
│                                                              │
│  ▼ 403  Forbidden                                           │
│     Description: Insufficient permissions                    │
│                                                              │
│  ▼ 404  Not Found                                           │
│     Description: Resource doesn't exist                      │
└─────────────────────────────────────────────────────────────┘
```

---

## 📖 Schemas Section (Bottom of Page)

```
┌─────────────────────────────────────────────────────────────┐
│  Schemas                                                     │
│                                                              │
│  ▼ LoginDto                                                 │
│     {                                                        │
│       email* (string): User email address                    │
│       password* (string): User password                      │
│       telemetry (TelemetryDto): Device telemetry data       │
│     }                                                        │
│                                                              │
│  ▼ AuthResponseDto                                          │
│     {                                                        │
│       accessToken (string): JWT access token                 │
│       refreshToken (string): Refresh token                   │
│       expiresIn (integer): Token expiration in seconds       │
│       user (UserDto): User information                       │
│     }                                                        │
│                                                              │
│  ▼ TripDto                                                  │
│     {                                                        │
│       id (integer): Trip ID                                  │
│       pickupLocation (LocationDto): Pickup location          │
│       dropoffLocation (LocationDto): Dropoff location        │
│       vehicle (VehicleDto): Assigned vehicle                 │
│       driver (UserDto): Assigned driver                      │
│       status (string): Trip status                           │
│       patientName (string): Patient name                     │
│       createdAt (string): Creation timestamp                 │
│       ...                                                    │
│     }                                                        │
└─────────────────────────────────────────────────────────────┘
```

---

## 🎨 Color Scheme

### HTTP Methods
- 🟢 **POST** - Green (#49cc90)
- 🔵 **GET** - Blue (#61affe)
- 🟠 **PUT** - Orange (#fca130)
- 🔴 **DELETE** - Red (#f93e3e)
- 🟦 **PATCH** - Teal (#50e3c2)

### Status Codes
- 🟢 **2xx Success** - Green background
- 🔴 **4xx/5xx Errors** - Red background

### UI Elements
- **Primary Color**: Red (#dc3545) - Emergency theme
- **Buttons**: Blue (#4990e2)
- **Authorize Button**: Red (#dc3545)
- **Background**: White/Light gray
- **Text**: Dark gray (#3b4151)

---

## 💡 Interactive Features

### Filter/Search
```
┌─────────────────────────────────────────────────────────────┐
│  🔍 Filter by tags                                          │
│  ┌──────────────────────────────────────────────────────┐  │
│  │ Search...                                             │  │
│  └──────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
```
Type to filter endpoints (e.g., "trip", "auth", "vehicle")

### Collapsible Sections
- Click section headers to expand/collapse
- Click endpoint rows to see details
- Click response codes to see examples

### Copy to Clipboard
- Hover over code blocks to see copy button
- Click to copy JSON examples
- Click to copy curl commands

---

## 📱 Mobile View

The Swagger UI is responsive and works on mobile devices:
- Collapsible sections for easier navigation
- Touch-friendly buttons
- Scrollable code blocks
- Optimized for smaller screens

---

## 🎯 Quick Actions

### Common Workflows

#### 1. Test Login
```
1. Expand "Authentication" section
2. Click POST /api/auth/login
3. Click "Try it out"
4. Click "Execute" (uses default example)
5. Copy accessToken from response
```

#### 2. Authorize
```
1. Click green "Authorize" button
2. Paste: Bearer {your-token}
3. Click "Authorize"
4. Click "Close"
```

#### 3. Create Trip
```
1. Expand "Trips" section
2. Click POST /api/trips
3. Click "Try it out"
4. Modify JSON as needed
5. Click "Execute"
```

#### 4. View Analytics
```
1. Expand "Analytics" section
2. Click GET /api/analytics/dashboard
3. Click "Try it out"
4. Set date range (optional)
5. Click "Execute"
```

---

## 🖼️ Visual Hierarchy

```
Header (Red bar with logo)
    ↓
API Description (Markdown formatted)
    ↓
Filter Box
    ↓
┌─ Authentication Section
│  ├─ POST /register
│  ├─ POST /login
│  └─ ...
│
├─ Trips Section
│  ├─ GET /trips
│  ├─ POST /trips
│  └─ ...
│
├─ Vehicles Section
│  └─ ...
│
├─ Users Section
│  └─ ...
│
└─ Analytics Section
   └─ ...
       ↓
Schemas Section (All DTOs)
```

---

## ✨ Pro Tips

### 1. Use the Filter
Type keywords to quickly find endpoints:
- "login" → Authentication endpoints
- "trip" → All trip-related endpoints
- "vehicle" → Vehicle management

### 2. Check Examples First
Before testing, click on the example to see the expected format

### 3. Copy curl Commands
Each request can be copied as a curl command for terminal use

### 4. Download Responses
Large responses can be downloaded as JSON files

### 5. Check Duration
Response duration helps identify slow endpoints

---

## 🎓 Learning the UI

### First Time Users
1. Start with the API description at the top
2. Scroll through sections to see what's available
3. Try the login endpoint first
4. Explore other endpoints after authenticating

### Keyboard Navigation
- **Tab**: Move between fields
- **Enter**: Submit forms
- **Escape**: Close dialogs
- **Ctrl/Cmd + F**: Search page

---

This visual guide helps you understand what to expect when using the Swagger UI. For hands-on practice, see **[SWAGGER_QUICK_START.md](./SWAGGER_QUICK_START.md)**.

**Happy Exploring! 🚑**

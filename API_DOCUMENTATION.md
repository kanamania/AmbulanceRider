# AmbulanceRider API Documentation

## Base URL
```
http://localhost:5000/api
```

## Authentication
Most endpoints require authentication using a Bearer token. Include the token in the Authorization header:
```
Authorization: Bearer {token}
```

---

## **Authentication Endpoints**

### **POST** `/api/auth/login`
Login with email and password.

**Authorization:** None  
**Content-Type:** `application/json`

**Request Body:**
```json
{
  "email": "string",
  "password": "string"
}
```

**Response:** `200 OK`
```json
{
  "token": "string",
  "user": {
    "id": 1,
    "firstName": "string",
    "lastName": "string",
    "email": "string",
    "phoneNumber": "string",
    "imagePath": "string?",
    "imageUrl": "string?",
    "roles": ["Admin", "Dispatcher"],
    "createdAt": "2025-10-27T15:00:00Z"
  }
}
```

**Error Responses:**
- `401 Unauthorized` - Invalid credentials

---

## **User Endpoints**

### **GET** `/api/users`
Get all users.

**Authorization:** Admin, Dispatcher  
**Response:** `200 OK`
```json
[
  {
    "id": 1,
    "firstName": "string",
    "lastName": "string",
    "email": "string",
    "phoneNumber": "string",
    "imagePath": "string?",
    "imageUrl": "string?",
    "roles": ["Admin"],
    "createdAt": "2025-10-27T15:00:00Z"
  }
]
```

---

### **GET** `/api/users/{id}`
Get user by ID.

**Authorization:** Authenticated  
**Path Parameters:**
- `id` (integer) - User ID

**Response:** `200 OK`
```json
{
  "id": 1,
  "firstName": "string",
  "lastName": "string",
  "email": "string",
  "phoneNumber": "string",
  "imagePath": "string?",
  "imageUrl": "string?",
  "roles": ["Admin"],
  "createdAt": "2025-10-27T15:00:00Z"
}
```

**Error Responses:**
- `404 Not Found` - User not found

---

### **POST** `/api/users`
Create a new user.

**Authorization:** Admin  
**Content-Type:** `multipart/form-data`

**Request Body:**
```
firstName: string (required)
lastName: string (required)
email: string (required)
phoneNumber: string (required)
password: string (required)
image: file (optional, jpg/jpeg/png/gif, max 5MB)
roleIds: int[] (required)
```

**Response:** `201 Created`
```json
{
  "id": 1,
  "firstName": "string",
  "lastName": "string",
  "email": "string",
  "phoneNumber": "string",
  "imagePath": "uploads/users/guid.jpg",
  "imageUrl": "http://localhost:5000/uploads/users/guid.jpg",
  "roles": ["Admin"],
  "createdAt": "2025-10-27T15:00:00Z"
}
```

**Error Responses:**
- `400 Bad Request` - Invalid data or email already exists

---

### **PUT** `/api/users/{id}`
Update an existing user.

**Authorization:** Admin  
**Content-Type:** `multipart/form-data`  
**Path Parameters:**
- `id` (integer) - User ID

**Request Body:**
```
firstName: string (optional)
lastName: string (optional)
email: string (optional)
phoneNumber: string (optional)
password: string (optional)
image: file (optional, jpg/jpeg/png/gif, max 5MB)
removeImage: bool (optional, default: false)
roleIds: int[] (optional)
```

**Response:** `200 OK`
```json
{
  "id": 1,
  "firstName": "string",
  "lastName": "string",
  "email": "string",
  "phoneNumber": "string",
  "imagePath": "uploads/users/guid.jpg",
  "imageUrl": "http://localhost:5000/uploads/users/guid.jpg",
  "roles": ["Admin"],
  "createdAt": "2025-10-27T15:00:00Z"
}
```

**Error Responses:**
- `404 Not Found` - User not found
- `400 Bad Request` - Invalid data

---

### **DELETE** `/api/users/{id}`
Delete a user (soft delete).

**Authorization:** Admin  
**Path Parameters:**
- `id` (integer) - User ID

**Response:** `204 No Content`

**Error Responses:**
- `404 Not Found` - User not found

---

## **Location Endpoints**

### **GET** `/api/locations`
Get all locations.

**Authorization:** Authenticated  
**Response:** `200 OK`
```json
[
  {
    "id": 1,
    "name": "string",
    "imagePath": "string?",
    "imageUrl": "string?",
    "createdAt": "2025-10-27T15:00:00Z"
  }
]
```

---

### **GET** `/api/locations/{id}`
Get location by ID.

**Authorization:** Authenticated  
**Path Parameters:**
- `id` (integer) - Location ID

**Response:** `200 OK`
```json
{
  "id": 1,
  "name": "string",
  "imagePath": "uploads/locations/guid.jpg",
  "imageUrl": "http://localhost:5000/uploads/locations/guid.jpg",
  "createdAt": "2025-10-27T15:00:00Z"
}
```

**Error Responses:**
- `404 Not Found` - Location not found

---

### **POST** `/api/locations`
Create a new location.

**Authorization:** Admin, Dispatcher  
**Content-Type:** `multipart/form-data`

**Request Body:**
```
name: string (required)
image: file (optional, jpg/jpeg/png/gif, max 5MB)
```

**Response:** `201 Created`
```json
{
  "id": 1,
  "name": "string",
  "imagePath": "uploads/locations/guid.jpg",
  "imageUrl": "http://localhost:5000/uploads/locations/guid.jpg",
  "createdAt": "2025-10-27T15:00:00Z"
}
```

**Error Responses:**
- `400 Bad Request` - Invalid data

---

### **PUT** `/api/locations/{id}`
Update an existing location.

**Authorization:** Admin, Dispatcher  
**Content-Type:** `multipart/form-data`  
**Path Parameters:**
- `id` (integer) - Location ID

**Request Body:**
```
name: string (optional)
image: file (optional, jpg/jpeg/png/gif, max 5MB)
removeImage: bool (optional, default: false)
```

**Response:** `200 OK`
```json
{
  "id": 1,
  "name": "string",
  "imagePath": "uploads/locations/guid.jpg",
  "imageUrl": "http://localhost:5000/uploads/locations/guid.jpg",
  "createdAt": "2025-10-27T15:00:00Z"
}
```

**Error Responses:**
- `404 Not Found` - Location not found
- `400 Bad Request` - Invalid data

---

### **DELETE** `/api/locations/{id}`
Delete a location.

**Authorization:** Admin  
**Path Parameters:**
- `id` (integer) - Location ID

**Response:** `204 No Content`

**Error Responses:**
- `404 Not Found` - Location not found

---

## **Route Endpoints**

### **GET** `/api/routes`
Get all routes.

**Authorization:** Authenticated  
**Response:** `200 OK`
```json
[
  {
    "id": 1,
    "name": "string",
    "distance": 10.5,
    "estimatedDuration": 30,
    "description": "string?",
    "createdAt": "2025-10-27T15:00:00Z",
    "fromLocation": {
      "id": 1,
      "name": "Location A"
    },
    "toLocation": {
      "id": 2,
      "name": "Location B"
    },
    "fromLocationId": 1,
    "toLocationId": 2
  }
]
```

---

### **GET** `/api/routes/{id}`
Get route by ID.

**Authorization:** Authenticated  
**Path Parameters:**
- `id` (integer) - Route ID

**Response:** `200 OK`
```json
{
  "id": 1,
  "name": "string",
  "distance": 10.5,
  "estimatedDuration": 30,
  "description": "string?",
  "createdAt": "2025-10-27T15:00:00Z",
  "fromLocation": {
    "id": 1,
    "name": "Location A"
  },
  "toLocation": {
    "id": 2,
    "name": "Location B"
  },
  "fromLocationId": 1,
  "toLocationId": 2
}
```

**Error Responses:**
- `404 Not Found` - Route not found

---

### **POST** `/api/routes`
Create a new route.

**Authorization:** Admin, Dispatcher  
**Content-Type:** `application/json`

**Request Body:**
```json
{
  "name": "string",
  "fromLocation": null,
  "toLocation": null,
  "fromLocationId": 1,
  "toLocationId": 2,
  "distance": 10.5,
  "estimatedDuration": 30,
  "description": "string (optional)"
}
```

**Response:** `201 Created`
```json
{
  "id": 1,
  "name": "string",
  "distance": 10.5,
  "estimatedDuration": 30,
  "description": "string?",
  "createdAt": "2025-10-27T15:00:00Z",
  "fromLocation": {
    "id": 1,
    "name": "Location A"
  },
  "toLocation": {
    "id": 2,
    "name": "Location B"
  },
  "fromLocationId": 1,
  "toLocationId": 2
}
```

---

### **PUT** `/api/routes/{id}`
Update an existing route.

**Authorization:** Admin, Dispatcher  
**Content-Type:** `application/json`  
**Path Parameters:**
- `id` (integer) - Route ID

**Request Body:**
```json
{
  "name": "string (optional)",
  "fromLocation": null,
  "toLocation": null,
  "fromLocationId": 1,
  "toLocationId": 2,
  "distance": 10.5,
  "estimatedDuration": 30,
  "description": "string (optional)"
}
```

**Response:** `200 OK`
```json
{
  "id": 1,
  "name": "string",
  "distance": 10.5,
  "estimatedDuration": 30,
  "description": "string?",
  "createdAt": "2025-10-27T15:00:00Z",
  "fromLocation": {
    "id": 1,
    "name": "Location A"
  },
  "toLocation": {
    "id": 2,
    "name": "Location B"
  },
  "fromLocationId": 1,
  "toLocationId": 2
}
```

**Error Responses:**
- `404 Not Found` - Route not found

---

### **DELETE** `/api/routes/{id}`
Delete a route (soft delete).

**Authorization:** Admin  
**Path Parameters:**
- `id` (integer) - Route ID

**Response:** `204 No Content`

**Error Responses:**
- `404 Not Found` - Route not found

---

## **Vehicle Endpoints**

### **GET** `/api/vehicles`
Get all vehicles.

**Authorization:** Authenticated  
**Response:** `200 OK`
```json
[
  {
    "id": 1,
    "name": "string",
    "imagePath": "string?",
    "imageUrl": "string?",
    "types": ["Ambulance", "Emergency"],
    "createdAt": "2025-10-27T15:00:00Z"
  }
]
```

---

### **GET** `/api/vehicles/types`
Get all vehicle types.

**Authorization:** Authenticated  
**Response:** `200 OK`
```json
[
  {
    "id": 1,
    "name": "Ambulance"
  },
  {
    "id": 2,
    "name": "Emergency"
  }
]
```

---

### **GET** `/api/vehicles/{id}`
Get vehicle by ID.

**Authorization:** Authenticated  
**Path Parameters:**
- `id` (integer) - Vehicle ID

**Response:** `200 OK`
```json
{
  "id": 1,
  "name": "string",
  "imagePath": "uploads/vehicles/guid.jpg",
  "imageUrl": "http://localhost:5000/uploads/vehicles/guid.jpg",
  "types": ["Ambulance", "Emergency"],
  "createdAt": "2025-10-27T15:00:00Z"
}
```

**Error Responses:**
- `404 Not Found` - Vehicle not found

---

### **POST** `/api/vehicles`
Create a new vehicle.

**Authorization:** Admin, Dispatcher  
**Content-Type:** `multipart/form-data`

**Request Body:**
```
name: string (required)
image: file (optional, jpg/jpeg/png/gif, max 5MB)
types: string[] (required)
```

**Response:** `201 Created`
```json
{
  "id": 1,
  "name": "string",
  "imagePath": "uploads/vehicles/guid.jpg",
  "imageUrl": "http://localhost:5000/uploads/vehicles/guid.jpg",
  "types": ["Ambulance", "Emergency"],
  "createdAt": "2025-10-27T15:00:00Z"
}
```

**Error Responses:**
- `400 Bad Request` - Invalid data

---

### **PUT** `/api/vehicles/{id}`
Update an existing vehicle.

**Authorization:** Admin, Dispatcher  
**Content-Type:** `multipart/form-data`  
**Path Parameters:**
- `id` (integer) - Vehicle ID

**Request Body:**
```
name: string (optional)
image: file (optional, jpg/jpeg/png/gif, max 5MB)
removeImage: bool (optional, default: false)
types: string[] (optional)
```

**Response:** `200 OK`
```json
{
  "id": 1,
  "name": "string",
  "imagePath": "uploads/vehicles/guid.jpg",
  "imageUrl": "http://localhost:5000/uploads/vehicles/guid.jpg",
  "types": ["Ambulance", "Emergency"],
  "createdAt": "2025-10-27T15:00:00Z"
}
```

**Error Responses:**
- `404 Not Found` - Vehicle not found
- `400 Bad Request` - Invalid data

---

### **DELETE** `/api/vehicles/{id}`
Delete a vehicle (soft delete).

**Authorization:** Admin  
**Path Parameters:**
- `id` (integer) - Vehicle ID

**Response:** `204 No Content`

**Error Responses:**
- `404 Not Found` - Vehicle not found

---

## **Trip Endpoints**

### **GET** `/api/trips`
Get all trips.

**Authorization:** Authenticated  
**Response:** `200 OK`
```json
[
  {
    "id": 1,
    "name": "string",
    "description": "string?",
    "scheduledStartTime": "2025-10-27T15:00:00Z",
    "actualStartTime": "2025-10-27T15:05:00Z",
    "actualEndTime": "2025-10-27T16:00:00Z",
    "status": "Completed",
    "rejectionReason": null,
    "routeId": 1,
    "route": {
      "id": 1,
      "name": "Route A to B",
      "distance": 10.5,
      "estimatedDuration": 30,
      "fromLocation": {...},
      "toLocation": {...}
    },
    "vehicleId": 1,
    "vehicle": {...},
    "driverId": 2,
    "driver": {...},
    "approvedBy": 3,
    "approver": {...},
    "approvedAt": "2025-10-27T14:00:00Z",
    "createdAt": "2025-10-27T13:00:00Z"
  }
]
```

---

### **GET** `/api/trips/{id}`
Get trip by ID.

**Authorization:** Authenticated  
**Path Parameters:**
- `id` (integer) - Trip ID

**Response:** `200 OK` - Same as single trip object above  
**Error Responses:**
- `404 Not Found` - Trip not found

---

### **GET** `/api/trips/status/{status}`
Get trips by status.

**Authorization:** Authenticated  
**Path Parameters:**
- `status` (enum) - Trip status: `Pending`, `Approved`, `Rejected`, `InProgress`, `Completed`, `Cancelled`

**Response:** `200 OK` - Array of trip objects

---

### **GET** `/api/trips/pending`
Get all pending trips awaiting approval.

**Authorization:** Admin, Dispatcher  
**Response:** `200 OK` - Array of pending trip objects

---

### **GET** `/api/trips/route/{routeId}`
Get trips by route.

**Authorization:** Authenticated  
**Path Parameters:**
- `routeId` (integer) - Route ID

**Response:** `200 OK` - Array of trip objects

---

### **GET** `/api/trips/driver/{driverId}`
Get trips by driver.

**Authorization:** Authenticated  
**Path Parameters:**
- `driverId` (integer) - Driver (User) ID

**Response:** `200 OK` - Array of trip objects

---

### **POST** `/api/trips`
Create a new trip.

**Authorization:** Admin, Dispatcher  
**Content-Type:** `application/json`

**Request Body:**
```json
{
  "name": "string",
  "description": "string (optional)",
  "scheduledStartTime": "2025-10-27T15:00:00Z",
  "routeId": 1,
  "vehicleId": 1,
  "driverId": 2
}
```

**Response:** `201 Created`
```json
{
  "id": 1,
  "name": "string",
  "description": "string?",
  "scheduledStartTime": "2025-10-27T15:00:00Z",
  "actualStartTime": null,
  "actualEndTime": null,
  "status": "Pending",
  "rejectionReason": null,
  "routeId": 1,
  "route": {...},
  "vehicleId": 1,
  "vehicle": {...},
  "driverId": 2,
  "driver": {...},
  "approvedBy": null,
  "approver": null,
  "approvedAt": null,
  "createdAt": "2025-10-27T13:00:00Z"
}
```

**Error Responses:**
- `400 Bad Request` - Invalid data or referenced entities not found

---

### **PUT** `/api/trips/{id}`
Update an existing trip (only pending trips can be updated).

**Authorization:** Admin, Dispatcher  
**Content-Type:** `application/json`  
**Path Parameters:**
- `id` (integer) - Trip ID

**Request Body:**
```json
{
  "name": "string (optional)",
  "description": "string (optional)",
  "scheduledStartTime": "2025-10-27T15:00:00Z (optional)",
  "routeId": 1,
  "vehicleId": 1,
  "driverId": 2
}
```

**Response:** `200 OK` - Returns updated TripDto  
**Error Responses:**
- `404 Not Found` - Trip not found
- `400 Bad Request` - Only pending trips can be updated

---

### **POST** `/api/trips/{id}/approve`
Approve or reject a trip.

**Authorization:** Admin, Dispatcher  
**Content-Type:** `application/json`  
**Path Parameters:**
- `id` (integer) - Trip ID

**Request Body:**
```json
{
  "approve": true,
  "rejectionReason": "string (required if approve is false)"
}
```

**Response:** `200 OK`
```json
{
  "id": 1,
  "name": "string",
  "status": "Approved",
  "approvedBy": 3,
  "approver": {...},
  "approvedAt": "2025-10-27T14:00:00Z",
  ...
}
```

**Error Responses:**
- `404 Not Found` - Trip not found
- `400 Bad Request` - Only pending trips can be approved/rejected, or rejection reason missing

---

### **POST** `/api/trips/{id}/start`
Start a trip (change status to in-progress).

**Authorization:** Authenticated  
**Content-Type:** `application/json`  
**Path Parameters:**
- `id` (integer) - Trip ID

**Request Body:**
```json
{
  "actualStartTime": "2025-10-27T15:05:00Z"
}
```

**Response:** `200 OK` - Returns updated TripDto with status `InProgress`  
**Error Responses:**
- `404 Not Found` - Trip not found
- `400 Bad Request` - Only approved trips can be started

---

### **POST** `/api/trips/{id}/complete`
Complete a trip.

**Authorization:** Authenticated  
**Content-Type:** `application/json`  
**Path Parameters:**
- `id` (integer) - Trip ID

**Request Body:**
```json
{
  "actualEndTime": "2025-10-27T16:00:00Z"
}
```

**Response:** `200 OK` - Returns updated TripDto with status `Completed`  
**Error Responses:**
- `404 Not Found` - Trip not found
- `400 Bad Request` - Only in-progress trips can be completed

---

### **POST** `/api/trips/{id}/cancel`
Cancel a trip.

**Authorization:** Admin, Dispatcher  
**Path Parameters:**
- `id` (integer) - Trip ID

**Response:** `200 OK` - Returns updated TripDto with status `Cancelled`  
**Error Responses:**
- `404 Not Found` - Trip not found
- `400 Bad Request` - Completed trips cannot be cancelled

---

### **DELETE** `/api/trips/{id}`
Delete a trip (soft delete).

**Authorization:** Admin  
**Path Parameters:**
- `id` (integer) - Trip ID

**Response:** `204 No Content`  
**Error Responses:**
- `404 Not Found` - Trip not found

---

## **File Upload Endpoints**

### **POST** `/api/fileupload/vehicle-image`
Upload a vehicle image.

**Authorization:** Admin, Dispatcher  
**Content-Type:** `multipart/form-data`

**Request Body:**
```
file: file (required, jpg/jpeg/png/gif, max 5MB)
```

**Response:** `200 OK`
```json
{
  "filePath": "uploads/vehicles/guid.jpg",
  "fileUrl": "http://localhost:5000/uploads/vehicles/guid.jpg"
}
```

**Error Responses:**
- `400 Bad Request` - No file uploaded, file too large, or invalid file type

---

### **DELETE** `/api/fileupload/vehicle-image`
Delete a vehicle image.

**Authorization:** Admin, Dispatcher  
**Query Parameters:**
- `filePath` (string) - Path to the file to delete

**Response:** `204 No Content`

**Error Responses:**
- `400 Bad Request` - File path is required
- `404 Not Found` - File not found
- `500 Internal Server Error` - Error deleting file

---

## **Authorization Roles**

The API uses role-based authorization with the following roles:

- **Admin** - Full access to all endpoints
- **Dispatcher** - Can manage locations, routes, and vehicles
- **Authenticated** - Basic access to read operations

---

## **Common Response Codes**

- `200 OK` - Request successful
- `201 Created` - Resource created successfully
- `204 No Content` - Request successful with no content to return
- `400 Bad Request` - Invalid request data
- `401 Unauthorized` - Authentication required or invalid credentials
- `403 Forbidden` - Insufficient permissions
- `404 Not Found` - Resource not found
- `500 Internal Server Error` - Server error

---

## **File Upload Specifications**

- **Allowed formats:** JPG, JPEG, PNG, GIF
- **Maximum file size:** 5MB
- **Upload locations:**
  - Users: `wwwroot/uploads/users/`
  - Locations: `wwwroot/uploads/locations/`
  - Vehicles: `wwwroot/uploads/vehicles/`

---

## **Trip Workflow**

The trip module follows a structured approval and lifecycle workflow:

### **Trip Statuses**
1. **Pending** - Initial status when trip is created, awaiting approval
2. **Approved** - Trip has been approved by Admin or Dispatcher
3. **Rejected** - Trip has been rejected (requires rejection reason)
4. **InProgress** - Trip has started (driver has begun the journey)
5. **Completed** - Trip has been completed successfully
6. **Cancelled** - Trip has been cancelled (cannot cancel completed trips)

### **Workflow Steps**
1. **Create Trip** (Admin/Dispatcher) → Status: `Pending`
2. **Approve/Reject Trip** (Admin/Dispatcher) → Status: `Approved` or `Rejected`
3. **Start Trip** (Any authenticated user) → Status: `InProgress` (only if approved)
4. **Complete Trip** (Any authenticated user) → Status: `Completed` (only if in progress)
5. **Cancel Trip** (Admin/Dispatcher) → Status: `Cancelled` (anytime except completed)

### **Business Rules**
- Only **pending** trips can be updated or approved/rejected
- Only **approved** trips can be started
- Only **in-progress** trips can be completed
- **Completed** trips cannot be cancelled
- Rejection requires a rejection reason
- Trips are associated with a route, and optionally with a vehicle and driver

---

## **Notes**

1. All endpoints except `/api/auth/login` require authentication via Bearer token
2. Soft delete is implemented for Users, Locations, Routes, Vehicles, and Trips
3. Image uploads automatically generate unique filenames using GUIDs
4. When updating with `removeImage: true`, the existing image will be deleted
5. All datetime values are in ISO 8601 format (UTC)
6. The `BaseUrl` configuration can be set in `appsettings.json` or will default to the request host
7. Trip approval workflow requires Admin or Dispatcher role
8. Trip status transitions are enforced by business logic

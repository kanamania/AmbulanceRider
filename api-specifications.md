# API Specifications

**Last Updated:** 2025-12-04T08:56:00+03:00  
**Version:** 0.0.19  
**Base URL:** `http://localhost:5000/api` (Development)  
**Production URL:** `https://app.globalexpress.co.tz/api`

---

## Overview

The AmbulanceRider API is a RESTful API built with ASP.NET Core 9.0. It provides endpoints for managing emergency medical dispatch operations.

### API Features
- JWT Bearer token authentication
- Role-based authorization
- Swagger/OpenAPI documentation
- Health check endpoints
- SignalR real-time hubs

---

## Authentication

### Authentication Flow

1. Client sends credentials to `/api/auth/login`
2. Server validates and returns JWT access token + refresh token
3. Client includes token in `Authorization` header for subsequent requests
4. When access token expires, use refresh token to get new tokens

### Token Format
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Token Expiration
- Access Token: 24 hours (configurable)
- Refresh Token: 7 days

---

## API Endpoints

### Authentication

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | /auth/login | User login | No |
| POST | /auth/register | User registration | No |
| POST | /auth/refresh | Refresh tokens | No |
| POST | /auth/logout | Logout user | Yes |
| POST | /auth/forgot-password | Request password reset | No |
| POST | /auth/reset-password | Reset password | No |
| GET | /auth/me | Get current user | Yes |

### Users

| Method | Endpoint | Description | Roles |
|--------|----------|-------------|-------|
| GET | /users | Get all users | Admin, Dispatcher |
| GET | /users/{id} | Get user by ID | Admin, Dispatcher |
| POST | /users | Create user | Admin |
| PUT | /users/{id} | Update user | Admin |
| DELETE | /users/{id} | Delete user | Admin |
| GET | /users/drivers | Get all drivers | Admin, Dispatcher |

### Trips

| Method | Endpoint | Description | Roles |
|--------|----------|-------------|-------|
| GET | /trips | Get all trips | All authenticated |
| GET | /trips/{id} | Get trip by ID | All authenticated |
| GET | /trips/status/{status} | Get trips by status | All authenticated |
| GET | /trips/pending | Get pending trips | Admin, Dispatcher |
| GET | /trips/driver/{driverId} | Get driver's trips | All authenticated |
| POST | /trips | Create trip | All authenticated |
| PUT | /trips/{id} | Update trip | All authenticated |
| DELETE | /trips/{id} | Delete trip | Admin |
| PUT | /trips/{id}/status | Update trip status | All authenticated |
| POST | /trips/{id}/approve | Approve trip | Admin, Dispatcher |
| POST | /trips/{id}/reject | Reject trip | Admin, Dispatcher |
| POST | /trips/{id}/start | Start trip | Driver |
| POST | /trips/{id}/complete | Complete trip | Driver, Admin |
| POST | /trips/{id}/cancel | Cancel trip | All authenticated |
| GET | /trips/{id}/status-logs | Get status history | All authenticated |

### Trip Types

| Method | Endpoint | Description | Roles |
|--------|----------|-------------|-------|
| GET | /triptypes | Get all trip types | All authenticated |
| GET | /triptypes/{id} | Get trip type by ID | All authenticated |
| POST | /triptypes | Create trip type | Admin |
| PUT | /triptypes/{id} | Update trip type | Admin |
| DELETE | /triptypes/{id} | Delete trip type | Admin |

### Vehicles

| Method | Endpoint | Description | Roles |
|--------|----------|-------------|-------|
| GET | /vehicles | Get all vehicles | All authenticated |
| GET | /vehicles/{id} | Get vehicle by ID | All authenticated |
| POST | /vehicles | Create vehicle | Admin, Dispatcher |
| PUT | /vehicles/{id} | Update vehicle | Admin, Dispatcher |
| DELETE | /vehicles/{id} | Delete vehicle | Admin |
| GET | /vehicles/types | Get vehicle types | All authenticated |

### Locations

| Method | Endpoint | Description | Roles |
|--------|----------|-------------|-------|
| GET | /locations | Get all locations | All authenticated |
| GET | /locations/{id} | Get location by ID | All authenticated |
| POST | /locations | Create location | Admin, Dispatcher |
| PUT | /locations/{id} | Update location | Admin, Dispatcher |
| DELETE | /locations/{id} | Delete location | Admin |

### Invoices

| Method | Endpoint | Description | Roles |
|--------|----------|-------------|-------|
| GET | /invoices | Get all invoices | Admin, Dispatcher |
| GET | /invoices/{id} | Get invoice by ID | Admin, Dispatcher |
| POST | /invoices | Create invoice | Admin, Dispatcher |
| PUT | /invoices/{id}/mark-paid | Mark as paid | Admin, Dispatcher |
| GET | /invoices/{id}/pdf | Download PDF | Admin, Dispatcher |
| GET | /invoices/{id}/excel | Download Excel | Admin, Dispatcher |
| GET | /invoices/{id}/download | Download both (ZIP) | Admin, Dispatcher |
| POST | /invoices/{id}/send-email | Send via email | Admin, Dispatcher |
| GET | /invoices/companies-with-unpaid-trips | Get companies | Admin, Dispatcher |

### Pricing

| Method | Endpoint | Description | Roles |
|--------|----------|-------------|-------|
| GET | /pricing | Get all pricing matrices | All authenticated |
| GET | /pricing/{id} | Get pricing by ID | All authenticated |
| POST | /pricing | Create pricing | Admin |
| PUT | /pricing/{id} | Update pricing | Admin |
| DELETE | /pricing/{id} | Delete pricing | Admin |
| GET | /pricing/calculate | Calculate price | All authenticated |

### Regions

| Method | Endpoint | Description | Roles |
|--------|----------|-------------|-------|
| GET | /regions | Get all regions | All authenticated |
| GET | /regions/{id} | Get region by ID | All authenticated |
| POST | /regions | Create region | Admin |
| PUT | /regions/{id} | Update region | Admin |
| DELETE | /regions/{id} | Delete region | Admin |

### Companies

| Method | Endpoint | Description | Roles |
|--------|----------|-------------|-------|
| GET | /companies | Get all companies | Admin, Dispatcher |
| GET | /companies/{id} | Get company by ID | Admin, Dispatcher |

### Telemetry

| Method | Endpoint | Description | Roles |
|--------|----------|-------------|-------|
| POST | /telemetry | Log telemetry event | All authenticated |
| POST | /telemetry/batch | Log batch events | All authenticated |
| POST | /telemetry/timeseries | Query timeseries | Admin, Dispatcher |
| GET | /telemetry/user/{userId}/timeseries | Get user timeseries | Admin, Dispatcher |
| GET | /telemetry/me/timeseries | Get own timeseries | All authenticated |

### Dashboard

| Method | Endpoint | Description | Roles |
|--------|----------|-------------|-------|
| GET | /dashboard/stats | Get dashboard stats | All authenticated |
| GET | /dashboard/company/{id}/stats | Get company stats | Admin, Dispatcher |

### Analytics

| Method | Endpoint | Description | Roles |
|--------|----------|-------------|-------|
| GET | /analytics/trips | Trip analytics | Admin, Dispatcher |
| GET | /analytics/revenue | Revenue analytics | Admin, Dispatcher |
| GET | /analytics/performance | Performance metrics | Admin, Dispatcher |

### System

| Method | Endpoint | Description | Roles |
|--------|----------|-------------|-------|
| GET | /health | Health check | Public |
| GET | /health/ready | Readiness check | Public |
| GET | /health/live | Liveness check | Public |

---

## Request/Response Formats

### Standard Success Response
```json
{
  "data": { ... },
  "message": "Success",
  "timestamp": "2025-12-04T08:45:00Z"
}
```

### Standard Error Response
```json
{
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "Validation failed",
    "details": [
      {
        "field": "email",
        "message": "Email is required"
      }
    ]
  },
  "timestamp": "2025-12-04T08:45:00Z"
}
```

### Pagination Response
```json
{
  "data": [ ... ],
  "pagination": {
    "page": 1,
    "pageSize": 20,
    "totalItems": 100,
    "totalPages": 5
  }
}
```

---

## Data Models

### User
| Field | Type | Description |
|-------|------|-------------|
| id | guid | Unique identifier |
| email | string | Email address |
| firstName | string | First name |
| lastName | string | Last name |
| phoneNumber | string | Phone number |
| roles | string[] | Assigned roles |
| companyId | guid? | Associated company |
| imagePath | string | Profile image path |
| createdAt | datetime | Creation timestamp |
| updatedAt | datetime | Last update timestamp |

### Trip
| Field | Type | Description |
|-------|------|-------------|
| id | int | Unique identifier |
| name | string | Trip name |
| description | string | Trip description |
| status | enum | Pending, Approved, InProgress, Completed, Cancelled, Rejected |
| fromLatitude | decimal | Origin latitude |
| fromLongitude | decimal | Origin longitude |
| toLatitude | decimal | Destination latitude |
| toLongitude | decimal | Destination longitude |
| fromLocationName | string | Origin name |
| toLocationName | string | Destination name |
| vehicleId | int? | Assigned vehicle |
| driverId | guid? | Assigned driver |
| tripTypeId | int? | Trip type |
| scheduledStartTime | datetime | Scheduled start |
| actualStartTime | datetime? | Actual start |
| actualEndTime | datetime? | Actual end |
| basePrice | decimal? | Base price |
| taxAmount | decimal? | Tax amount |
| totalPrice | decimal? | Total price |
| createdBy | guid | Creator user ID |
| createdAt | datetime | Creation timestamp |

### Invoice
| Field | Type | Description |
|-------|------|-------------|
| id | int | Unique identifier |
| invoiceNumber | string | Invoice number (auto-generated) |
| companyId | guid | Company ID |
| type | enum | Proforma, Final |
| status | enum | Draft, Sent, Paid, Overdue, Cancelled |
| subtotal | decimal | Subtotal amount |
| taxAmount | decimal | Tax amount |
| totalAmount | decimal | Total amount |
| issueDate | datetime | Issue date |
| dueDate | datetime | Due date |
| paidDate | datetime? | Payment date |
| trips | InvoiceTrip[] | Associated trips |

### Vehicle
| Field | Type | Description |
|-------|------|-------------|
| id | int | Unique identifier |
| name | string | Vehicle name |
| plateNumber | string | License plate |
| vehicleTypeId | int | Vehicle type |
| image | string | Vehicle image path |
| assignedDrivers | User[] | Assigned drivers |
| createdAt | datetime | Creation timestamp |

### PricingMatrix
| Field | Type | Description |
|-------|------|-------------|
| id | int | Unique identifier |
| name | string | Pricing name |
| minWeight | decimal | Minimum weight |
| maxWeight | decimal | Maximum weight |
| minHeight | decimal | Minimum height |
| maxHeight | decimal | Maximum height |
| minLength | decimal | Minimum length |
| maxLength | decimal | Maximum length |
| minWidth | decimal | Minimum width |
| maxWidth | decimal | Maximum width |
| basePrice | decimal | Base price |
| taxRate | decimal | Tax rate (percentage) |
| totalPrice | decimal | Total price |
| regionId | int? | Region ID |
| isDefault | bool | Default pricing flag |

---

## Status Codes

| Code | Description |
|------|-------------|
| 200 | OK - Request successful |
| 201 | Created - Resource created |
| 204 | No Content - Successful, no response body |
| 400 | Bad Request - Invalid request data |
| 401 | Unauthorized - Authentication required |
| 403 | Forbidden - Insufficient permissions |
| 404 | Not Found - Resource not found |
| 409 | Conflict - Resource conflict |
| 422 | Unprocessable Entity - Validation error |
| 500 | Internal Server Error - Server error |

---

## Rate Limiting

**Current Status:** Not implemented

**Planned Limits:**
| Endpoint Category | Limit |
|-------------------|-------|
| Authentication | 10 requests/minute |
| Read Operations | 100 requests/minute |
| Write Operations | 30 requests/minute |
| File Downloads | 10 requests/minute |

---

## SignalR Hubs

### Notification Hub
**URL:** `/hubs/notifications`

| Event | Description |
|-------|-------------|
| ReceiveNotification | General notification |
| TripStatusChanged | Trip status update |
| NewTripCreated | New trip notification |

### Trip Hub
**URL:** `/hubs/trips`

| Event | Description |
|-------|-------------|
| TripUpdated | Trip data updated |
| TripLocationUpdated | Driver location update |
| TripStatusChanged | Status change notification |

---

## Swagger Documentation

Swagger UI is available at the API root in development:
- **URL:** `http://localhost:5000/`
- **OpenAPI Spec:** `http://localhost:5000/swagger/v1/swagger.json`

### Using Swagger
1. Navigate to Swagger UI
2. Click "Authorize" button
3. Login via `/api/auth/login` to get token
4. Enter: `Bearer {your-token}`
5. Test endpoints

---

## CORS Configuration

### Development
All origins allowed with credentials support.

### Production
Configured origins only:
- `https://app.globalexpress.co.tz`
- Additional origins via `Cors:AllowedOrigins` configuration

---

## Versioning

**Current Version:** v1.0.0

**Versioning Strategy:** URL-based (planned)
- Current: `/api/endpoint`
- Future: `/api/v1/endpoint`, `/api/v2/endpoint`

---

## Deprecation Policy

- Deprecated endpoints will be marked in Swagger
- 6-month notice before removal
- Migration guides provided for breaking changes

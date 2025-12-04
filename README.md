# 🚑 AmbulanceRider - Emergency Medical Dispatch System

A full-stack web application for managing emergency medical dispatch operations with comprehensive trip management, status workflow, audit trails, real-time tracking, and advanced telemetry.

**Version:** 0.0.19 | **Last Updated:** 2025-12-04

> **Latest Updates (2025-12-04):** 
> - Invoice PDF: Removed Route/Driver columns, added letterhead image, aligned Invoice Period right ✅
> - Consolidated documentation into 6 core files ✅
> - Created planning-implementation.md with roadmap & recommendations ✅
> - Created end-user-documentation.md with user guides ✅
> - Created testing.md with testing strategy ✅
> - Created api-specifications.md with API docs ✅
> - Fixed invoice download 401 Unauthorized ✅
> - Invoice System with PDF & Excel Generation ⭐
> - Region-aware Pricing Matrix ⭐
> - Comprehensive Telemetry & Analytics System ⭐

## 📚 Core Documentation

| Document | Description |
|----------|-------------|
| [README.md](./README.md) | Project overview and quick start |
| [changelog.md](./changelog.md) | Version history and changes |
| [planning-implementation.md](./planning-implementation.md) | Roadmap, weaknesses, recommendations |
| [end-user-documentation.md](./end-user-documentation.md) | User guides for all modules |
| [testing.md](./testing.md) | Testing strategy and procedures |
| [api-specifications.md](./api-specifications.md) | API endpoint documentation |

## 🎯 Features

### Backend API
- ✅ **RESTful API** with ASP.NET Core 9.0
- ✅ **JWT Authentication** with role-based authorization
- ✅ **PostgreSQL Database** with Entity Framework Core
- ✅ **Swagger UI** for API documentation and testing
- ✅ **Repository Pattern** for data access
- ✅ **Soft Delete** implementation
- ✅ **Automatic Auditing** (CreatedAt, UpdatedAt, CreatedBy, etc.)
- ✅ **CORS** enabled for cross-origin requests
- ✅ **Telemetry & Analytics** - Device, GPS, and user behavior tracking ⭐ NEW

### Frontend
- ✅ **Blazor WebAssembly** (.NET 9.0)
- ✅ **Bootstrap 5.3** for responsive UI
- ✅ **Multi-level Navigation** with dropdown menus
- ✅ **Complete CRUD Operations** for all modules
- ✅ **Real-time Validation**
- ✅ **Loading States** and error handling

### Modules
1. **Users Management**
   - Create, Read, Update, Delete users
   - Role assignment (Admin, Driver, Dispatcher, User)
   - Password hashing with BCrypt
   - Email and phone validation

2. **Vehicles Management**
   - Vehicle information with images
   - Multiple vehicle types per vehicle
   - Card-based display with images
   - Driver assignment capability
   - Auto-fill driver selection based on vehicle

3. **Trips Management** ⭐ ENHANCED
   - Interactive map-based location selection
   - Coordinate-based trip planning (latitude/longitude)
   - Complete status workflow (Pending → Approved → InProgress → Completed)
   - Driver/User trip creation and management
   - Admin approval/rejection with reasons
   - Trip cancellation with optional reasons
   - Force complete capability (Admin override)
   - **Audit Trail System** - Complete status change history
   - **Visual Timeline UI** - See who changed what, when, and why
   - **Dynamic Trip Types** ⭐ NEW - Categorize trips with custom attributes
   - **Custom Attributes** ⭐ NEW - Add dynamic fields per trip type
   - Real-time status updates
   - Role-based permissions

5. **Trip Types Management** ⭐
   - Define trip categories (Emergency, Routine, Transfer, etc.)
   - Add custom attributes with multiple data types
   - Support for text, number, date, boolean, select, textarea fields
   - Validation rules and default values
   - UI metadata (colors, icons, labels, placeholders)
   - Active/inactive status management
   - Display order configuration

6. **Telemetry & Analytics** ⭐
   - **Device Information** - OS, browser, device type, app version
   - **Account Detection** - Google/Apple account type based on OS
   - **GPS Tracking** - Real-time location data with accuracy
   - **Network Monitoring** - Connection type, online status
   - **Battery Status** - Level and charging state
   - **Installed Apps** - Browser plugins and PWA apps (limited)
   - **Event Tracking** - Login, register, trip operations
   - **Timeseries Logging** - Batch telemetry for high-volume data
   - **Time-based Queries** - Query telemetry by date ranges
   - **Route Visualization** - Track location history over time
   - **Privacy-Conscious** - Graceful permission handling
   - **Non-Blocking** - Never interrupts main operations

7. **Invoice Management** ⭐ NEW
   - **Proforma & Final Invoices** - Generate both invoice types
   - **PDF Generation** - Professional invoices with QuestPDF
   - **Excel Export** - Detailed spreadsheets with ClosedXML
   - **Invoice Preview** - Preview before creating
   - **Payment Tracking** - Mark invoices as paid
   - **Automatic Trip Updates** - Mark all trips paid when invoice is paid
   - **Flexible Date Ranges** - Weekly, monthly, or custom periods
   - **Company Filtering** - Filter by company, type, status, date
   - **Download Options** - PDF, Excel, or both as ZIP
   - **Sequential Numbering** - Automatic invoice number generation
   - **Detailed Breakdown** - Complete trip-level details in invoices

## 🏗️ Architecture

```
AmbulanceRider/
├── AmbulanceRider.API/          # Backend API
│   ├── Controllers/             # API endpoints
│   ├── Data/                    # DbContext
│   ├── DTOs/                    # Data Transfer Objects
│   ├── Models/                  # Database entities
│   ├── Repositories/            # Data access layer
│   ├── Services/                # Business logic
│   └── Migrations/              # EF Core migrations
│
├── AmbulanceRider/              # Blazor WebAssembly
│   ├── Components/
│   │   ├── Layout/              # Navigation & layout
│   │   ├── Shared/              # Reusable components (MapPicker, etc.)
│   │   └── Pages/               # CRUD pages
│   │       ├── Users/           # User management
│   │       ├── Vehicles/        # Vehicle management
│   │       ├── Locations/       # Location management
│   │       └── Trips/           # Trip management ⭐
│   │           ├── Trips.razor           # Trip list
│   │           ├── CreateTrip.razor      # Create trip with maps
│   │           ├── EditTrip.razor        # Edit trip
│   │           ├── TripActions.razor     # Status actions ⭐
│   │           └── TripStatusHistory.razor # Audit timeline ⭐
│   ├── Models/                  # DTOs
│   └── Services/                # API communication
│
└── docker-compose.yaml          # Container orchestration
```

## 🚀 Quick Start

### Prerequisites
- .NET 9.0 SDK
- Docker Desktop (for PostgreSQL)
- PowerShell 7

### Running with Docker Compose

```bash
# Start all services
docker-compose up --build

# Access the application
# API: http://localhost:5000
# Swagger: http://localhost:5000
# Blazor App: http://localhost:8080
```

### Running Locally

1. **Start PostgreSQL:**
```bash
docker-compose up db -d
```

2. **Run the API:**
```bash
cd AmbulanceRider.API
dotnet run
```
API will be available at: http://localhost:5000

3. **Run the Blazor App:**
```bash
cd AmbulanceRider
dotnet run
```
App will be available at the configured port (check console output)

## 📊 Database Schema

### Tables
- **users** - User accounts with authentication
- **roles** - User roles (Admin, Driver, Dispatcher, User)
- **user_roles** - Many-to-many relationship
- **vehicles** - Vehicle information
- **vehicle_types** - Vehicle type classifications
- **vehicle_drivers** - Driver-vehicle assignments
- **locations** - Predefined locations
- **trips** - Trip management with coordinates
- **trip_status_logs** - Audit trail for status changes
- **trip_types** - Trip categories with metadata ⭐
- **trip_type_attributes** - Custom field definitions ⭐
- **trip_attribute_values** - Actual field values per trip ⭐
- **telemetries** - Device, GPS, and analytics data ⭐ NEW
- **refresh_tokens** - JWT refresh token management

### Relationships
- Users ↔ Roles (Many-to-Many)
- Users ↔ Vehicles (Many-to-Many via vehicle_drivers)
- Vehicles → VehicleTypes (One-to-Many)
- Trips → Users (Driver, Approver, Creator)
- Trips → Vehicles (One-to-Many)
- Trips → TripTypes (Many-to-One) ⭐
- TripStatusLogs → Trips (One-to-Many, cascade delete)
- TripStatusLogs → Users (One-to-Many)
- TripTypes → TripTypeAttributes (One-to-Many, cascade delete) ⭐
- TripAttributeValues → Trips (Many-to-One, cascade delete) ⭐
- TripAttributeValues → TripTypeAttributes (Many-to-One) ⭐
- Telemetries → Users (Many-to-One, optional) ⭐ NEW

## 🔐 Authentication & Authorization

### JWT Token Authentication
The API uses JWT Bearer tokens for authentication. Include the token in requests:

```http
Authorization: Bearer <your-token-here>
```

### Roles & Permissions

| Role | Users | Vehicles | Trips | Trip Status |
|------|-------|----------|--------|-------|-------------|
| **Admin** | Full CRUD | Full CRUD | Full CRUD | Approve, Reject, Force Complete |
| **Dispatcher** | Read | Create, Read, Update | Full CRUD | Approve, Reject, Force Complete |
| **Driver** | Read | Read | Create, Read, Update | Complete, Cancel (own trips) |
| **User** | Read | Read | Create, Read, Update | Complete, Cancel (own trips) |

**Note:** Driver and User roles have the same permissions for trip management. Both can create trips and manage their own trips.

### Login Endpoint

**POST** `/api/auth/login`

```json
{
  "email": "admin@ambulancerider.com",
  "password": "Admin123!"
}
```

Response:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": 1,
    "firstName": "Admin",
    "lastName": "User",
    "email": "admin@ambulancerider.com",
    "roles": ["Admin"]
  }
}
```

## 📚 API Endpoints

### Users
- `GET /api/users` - Get all users (Admin, Dispatcher)
- `GET /api/users/{id}` - Get user by ID
- `POST /api/users` - Create user (Admin)
- `PUT /api/users/{id}` - Update user (Admin)
- `DELETE /api/users/{id}` - Delete user (Admin)

### Vehicles
- `GET /api/vehicles` - Get all vehicles
- `GET /api/vehicles/{id}` - Get vehicle by ID
- `POST /api/vehicles` - Create vehicle (Admin, Dispatcher)
- `PUT /api/vehicles/{id}` - Update vehicle (Admin, Dispatcher)
- `DELETE /api/vehicles/{id}` - Delete vehicle (Admin)

### Trips ⭐
- `GET /api/trips` - Get all trips
- `GET /api/trips/{id}` - Get trip by ID
- `GET /api/trips/status/{status}` - Get trips by status
- `GET /api/trips/pending` - Get pending trips
- `POST /api/trips` - Create trip (with telemetry)
- `PUT /api/trips/{id}` - Update trip (with telemetry)
- `PUT /api/trips/{id}/status` - Update trip status (with validation & telemetry)
- `POST /api/trips/{id}/approve` - Approve trip (Admin, Dispatcher, with telemetry)
- `POST /api/trips/{id}/start` - Start trip (Driver, with telemetry)
- `POST /api/trips/{id}/complete` - Complete trip (Driver, with telemetry)
- `POST /api/trips/{id}/cancel` - Cancel trip
- `GET /api/trips/{id}/status-logs` - Get status change history ⭐
- `DELETE /api/trips/{id}` - Delete trip (Admin)

### Authentication ⭐
- `POST /api/auth/register` - Register new user (with telemetry)
- `POST /api/auth/login` - Login (with telemetry)
- `POST /api/auth/logout` - Logout
- `POST /api/auth/refresh` - Refresh token
- `POST /api/auth/forgot-password` - Request password reset (with telemetry)
- `POST /api/auth/reset-password` - Reset password (with telemetry)
- `GET /api/auth/me` - Get current user

### Telemetry ⭐ NEW
- `POST /api/telemetry` - Log single telemetry event
- `POST /api/telemetry/batch` - Log batch telemetry (timeseries)
- `POST /api/telemetry/timeseries` - Query timeseries data (Admin/Dispatcher)
- `GET /api/telemetry/user/{userId}/timeseries` - Get user timeseries
- `GET /api/telemetry/me/timeseries` - Get current user's timeseries

## 🛠️ Technologies

### Backend
- **Framework:** ASP.NET Core 9.0
- **ORM:** Entity Framework Core 9.0
- **Database:** PostgreSQL 17.6
- **Authentication:** JWT Bearer
- **Password Hashing:** BCrypt.Net
- **API Documentation:** Swagger/OpenAPI

### Frontend
- **Framework:** Blazor WebAssembly
- **UI Library:** Bootstrap 5.3
- **Icons:** Bootstrap Icons
- **HTTP Client:** System.Net.Http
- **Maps:** Leaflet.js (via MapPicker component) ⭐
- **Telemetry:** JavaScript Geolocation API, Battery API, Network Information API ⭐ NEW

### DevOps
- **Containerization:** Docker
- **Orchestration:** Docker Compose
- **Web Server:** Nginx (for Blazor)

## 📝 Configuration

### API Configuration (appsettings.json)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=ambulance_rider;Username=ambulance_rider;Password=..."
  },
  "Jwt": {
    "Key": "YourSuperSecretKey...",
    "Issuer": "AmbulanceRider",
    "Audience": "AmbulanceRiderClient"
  }
}
```

### Environment Variables (Docker)

```yaml
environment:
  - ASPNETCORE_ENVIRONMENT=Development
  - ConnectionStrings__DefaultConnection=Host=db;Port=5432;...
```

## 🧪 Testing

### Using Swagger UI
1. Navigate to http://localhost:5000
2. Click "Authorize" button
3. Login via `/api/auth/login` to get token
4. Enter token in format: `Bearer <token>`
5. Test all endpoints

### Manual Testing
```bash
# Login
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@example.com","password":"Admin123!"}'

# Get users (with token)
curl -X GET http://localhost:5000/api/users \
  -H "Authorization: Bearer <your-token>"
```

## 📦 Database Migrations

### Create Migration
```bash
cd AmbulanceRider.API
dotnet ef migrations add MigrationName
```

### Apply Migration
```bash
dotnet ef database update
```

### Remove Last Migration
```bash
dotnet ef migrations remove
```

## 🔧 Development

### Adding a New Module

1. **Create Model** in `AmbulanceRider.API/Models/`
2. **Add DbSet** to `ApplicationDbContext`
3. **Create Repository** interface and implementation
4. **Create Service** interface and implementation
5. **Create Controller** with CRUD endpoints
6. **Create DTOs** in both API and Blazor projects
7. **Add API methods** to `ApiService.cs`
8. **Create Blazor pages** (List, Create, Edit)
9. **Update navigation** menu

## 🐛 Troubleshooting

### Database Connection Issues
```bash
# Check PostgreSQL is running
docker ps

# View logs
docker-compose logs db

# Restart database
docker-compose restart db
```

### Migration Issues
```bash
# Drop database and recreate
docker-compose down -v
docker-compose up db -d
cd AmbulanceRider.API
dotnet ef database update
```

### CORS Issues
- Ensure API CORS policy allows your Blazor origin
- Check browser console for CORS errors
- Verify `AllowAll` policy in `Program.cs`

## 📈 Known Weaknesses & Recommendations

See [planning-implementation.md](./planning-implementation.md) for detailed analysis.

### Critical Issues
| Issue | Risk | Status |
|-------|------|--------|
| No automated testing | High regression risk | ❌ Open |
| Hardcoded secrets in docker-compose | Security vulnerability | ❌ Open |
| Duplicate DI registrations | Maintenance overhead | ❌ Open |

### Medium Priority
| Issue | Risk | Status |
|-------|------|--------|
| Documentation sprawl (60+ files) | Maintenance difficulty | 🔄 In Progress |
| Missing input validation | Data integrity | ❌ Open |
| No rate limiting | DoS vulnerability | ❌ Open |
| Inconsistent error handling | Client-side issues | ❌ Open |

### Recommendations
1. **Immediate:** Move secrets to .env file, remove duplicate DI registrations
2. **Short-term:** Add unit tests, implement rate limiting, standardize errors
3. **Long-term:** Add CI/CD pipeline, implement caching, add API versioning

## 📈 Future Enhancements

### Planned Features
- [ ] Unit and integration tests (Priority: High)
- [ ] CI/CD pipeline (Priority: High)
- [ ] Rate limiting (Priority: Medium)
- [ ] Real-time notifications with SignalR
- [ ] Mobile app (MAUI)
- [ ] Advanced analytics dashboard
- [ ] Multi-language support
- [ ] Email notifications on status changes

### Technical Debt
See [planning-implementation.md](./planning-implementation.md#technical-debt-register) for complete register.

## 📄 License

This project is for educational and demonstration purposes.

## 👥 Contributors

- Development Team

## 📞 Support

For issues and questions, please create an issue in the repository.

---

**Built with ❤️ using .NET 9.0 and Blazor WebAssembly**

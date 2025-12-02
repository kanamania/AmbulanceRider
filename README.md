# 🚑 AmbulanceRider - Emergency Medical Dispatch System

A full-stack web application for managing emergency medical dispatch operations with comprehensive trip management, status workflow, audit trails, real-time tracking, and advanced telemetry.

> **Latest Updates (2025-12-02):** 
> - Invoice System with PDF & Excel Generation ⭐ NEW
> - Dashboard now redirects unauthorized sessions to the login page ✅
> - Trip Status Change Logging & Audit Trail System ✅
> - Dynamic Trip Types with Custom Attributes ⭐
> - Comprehensive Telemetry & Analytics System ⭐

📚 **[Complete Documentation Index](./DOCUMENTATION_INDEX.md)** - Navigate all documentation files organized by topic and use case.

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

## 📚 Documentation

### 📖 Complete Documentation Index

#### Feature Documentation
1. **[FEATURE_SUMMARY.md](./FEATURE_SUMMARY.md)** - Complete overview of all implemented features
2. **[API_DOCUMENTATION.md](./API_DOCUMENTATION.md)** - Detailed API endpoint documentation

#### Trip Management
3. **[TRIP_STATUS_LOGGING_IMPLEMENTATION.md](./TRIP_STATUS_LOGGING_IMPLEMENTATION.md)** - Audit trail system technical details
4. **[TRIP_STATUS_LOGGING_GUIDE.md](./TRIP_STATUS_LOGGING_GUIDE.md)** - User guide for viewing status history
5. **[TRIP_STATUS_WORKFLOW_SUMMARY.md](./TRIP_STATUS_WORKFLOW_SUMMARY.md)** - Status workflow rules and transitions
6. **[QUICK_START_TRIP_STATUS.md](./QUICK_START_TRIP_STATUS.md)** - Quick guide for changing trip status
7. **[TRIP_UI_UPDATE_SUMMARY.md](./TRIP_UI_UPDATE_SUMMARY.md)** - Coordinate-based system implementation
8. **[TRIP_COORDINATES_UPDATE.md](./TRIP_COORDINATES_UPDATE.md)** - Map integration details
9. **[TRIP_MODULE_SUMMARY.md](./TRIP_MODULE_SUMMARY.md)** - Trip module overview

#### Telemetry & Analytics ⭐ NEW
10. **[TELEMETRY_IMPLEMENTATION.md](./TELEMETRY_IMPLEMENTATION.md)** - Complete telemetry system documentation
11. **[TELEMETRY_ENHANCEMENTS.md](./TELEMETRY_ENHANCEMENTS.md)** - Account & apps tracking features
12. **[TELEMETRY_TIMESERIES_GUIDE.md](./TELEMETRY_TIMESERIES_GUIDE.md)** - Timeseries logging and querying ⭐ NEW

#### Implementation Guides
13. **[IMPLEMENTATION_COMPLETE.md](./IMPLEMENTATION_COMPLETE.md)** - Status management implementation
14. **[IMPLEMENTATION_SUMMARY.md](./IMPLEMENTATION_SUMMARY.md)** - General implementation summary
15. **[VEHICLE_DRIVER_AUTO_FILL_SUMMARY.md](./VEHICLE_DRIVER_AUTO_FILL_SUMMARY.md)** - Vehicle-driver integration

#### Quick Start
16. **[QUICKSTART.md](./QUICKSTART.md)** - Getting started guide

### 🗂️ Documentation by Topic

#### For Users
- **Getting Started**: [QUICKSTART.md](./QUICKSTART.md)
- **Trip Status Management**: [QUICK_START_TRIP_STATUS.md](./QUICK_START_TRIP_STATUS.md)
- **Viewing Status History**: [TRIP_STATUS_LOGGING_GUIDE.md](./TRIP_STATUS_LOGGING_GUIDE.md)

#### For Developers
- **API Reference**: [API_DOCUMENTATION.md](./API_DOCUMENTATION.md)
- **Feature Overview**: [FEATURE_SUMMARY.md](./FEATURE_SUMMARY.md)
- **Trip Workflow**: [TRIP_STATUS_WORKFLOW_SUMMARY.md](./TRIP_STATUS_WORKFLOW_SUMMARY.md)
- **Audit System**: [TRIP_STATUS_LOGGING_IMPLEMENTATION.md](./TRIP_STATUS_LOGGING_IMPLEMENTATION.md)
- **Telemetry System**: [TELEMETRY_IMPLEMENTATION.md](./TELEMETRY_IMPLEMENTATION.md) ⭐ NEW
- **Telemetry Enhancements**: [TELEMETRY_ENHANCEMENTS.md](./TELEMETRY_ENHANCEMENTS.md) ⭐ NEW
- **Timeseries Telemetry**: [TELEMETRY_TIMESERIES_GUIDE.md](./TELEMETRY_TIMESERIES_GUIDE.md) ⭐ NEW

#### For Administrators
- **Complete Feature List**: [FEATURE_SUMMARY.md](./FEATURE_SUMMARY.md)
- **Status Workflow Rules**: [TRIP_STATUS_WORKFLOW_SUMMARY.md](./TRIP_STATUS_WORKFLOW_SUMMARY.md)
- **Implementation Details**: [IMPLEMENTATION_COMPLETE.md](./IMPLEMENTATION_COMPLETE.md)
- **Analytics & Tracking**: [TELEMETRY_IMPLEMENTATION.md](./TELEMETRY_IMPLEMENTATION.md) ⭐ NEW

## 📋 Change Log

### Version 1.0.0 (2025-10-27)

#### ⭐ New Features

**Trip Status Change Logging & Audit Trail**
- ✅ Automatic logging of all trip status changes
- ✅ Complete audit trail with user, timestamp, and reasons
- ✅ Visual timeline UI component
- ✅ Status change history API endpoint
- ✅ Database table: `trip_status_logs` with indexes
- ✅ Cascade delete protection
- ✅ Read-only logs for compliance

**Trip Status Workflow Management**
- ✅ Complete/Cancel trips with optional notes
- ✅ Approve/Reject trips with required reasons
- ✅ Force complete capability (Admin override)
- ✅ Status transition validation
- ✅ Role-based permissions
- ✅ Modal dialogs for user input
- ✅ Real-time UI updates

**Coordinate-Based Trip System**
- ✅ Interactive map pickers for location selection
- ✅ Latitude/longitude coordinate storage
- ✅ Optional location names for readability
- ✅ Visual feedback on coordinate updates
- ✅ Flexible system (not limited to predefined routes)

**Vehicle-Driver Integration**
- ✅ Auto-fill driver selection based on vehicle
- ✅ Vehicle-driver assignment management
- ✅ Driver availability tracking

**Telemetry & Analytics System** ⭐ NEW
- ✅ Device information tracking (OS, browser, device type, app version)
- ✅ Google/Apple account type detection based on OS
- ✅ GPS location tracking with accuracy metrics
- ✅ Network monitoring (connection type, online status)
- ✅ Battery status tracking (level, charging state)
- ✅ Installed apps detection (browser plugins, PWA apps)
- ✅ Event tracking (login, register, trip operations)
- ✅ **Timeseries logging** - Batch telemetry for high-volume data ⭐ NEW
- ✅ **Time-based queries** - Query telemetry by date ranges ⭐ NEW
- ✅ **Route visualization** - Track location history over time ⭐ NEW
- ✅ **User privacy controls** - Users can only access their own data ⭐ NEW
- ✅ Privacy-conscious implementation with graceful permission handling
- ✅ Non-blocking telemetry collection
- ✅ Database storage with indexed queries

#### 🔧 Technical Improvements
- ✅ Repository pattern for TripStatusLog and Telemetry
- ✅ Service layer with business logic validation
- ✅ DTOs for clean API contracts
- ✅ Database indexes for performance
- ✅ Soft delete implementation
- ✅ Comprehensive error handling
- ✅ Loading states and user feedback
- ✅ JavaScript interop for browser APIs
- ✅ Geolocation, Battery, and Network Information APIs

#### 📝 Documentation
- ✅ 16 comprehensive documentation files
- ✅ User guides and quick starts
- ✅ Technical implementation details
- ✅ API endpoint documentation
- ✅ Status workflow diagrams
- ✅ Telemetry system documentation
- ✅ Timeseries telemetry guide ⭐ NEW
- ✅ Privacy and compliance guidelines

### Version 0.9.0 (Previous)
- ✅ User management with roles
- ✅ Vehicle management with types
- ✅ Location management
- ✅ JWT authentication
- ✅ Basic trip CRUD operations

## 📈 Future Enhancements

### Planned Features
- [ ] Real-time notifications with SignalR
- [ ] Mobile app (MAUI) for enhanced telemetry access
- [ ] Advanced reporting and analytics dashboard
- [ ] Telemetry analytics dashboard with charts and graphs
- [ ] Telemetry data export (CSV, JSON)
- [ ] Telemetry aggregation and statistics
- [ ] Multi-language support
- [ ] Email notifications on status changes
- [ ] Performance monitoring dashboard
- [ ] Unit and integration tests
- [ ] CI/CD pipeline
- [ ] OAuth integration for Google/Apple account emails

### Audit Trail Enhancements
- [ ] Export logs to CSV/PDF
- [ ] Advanced filtering (date range, user, status)
- [ ] Search in notes and reasons
- [ ] Analytics dashboard for status changes
- [ ] Comparison view (before/after states)
- [ ] Slack/Teams integration for notifications

### Trip Management Enhancements
- [ ] Bulk status operations
- [ ] GPS verification for completion
- [ ] Photo/signature capture
- [ ] Auto-approve based on criteria
- [ ] Auto-start at scheduled time
- [ ] Route optimization suggestions

## 📄 License

This project is for educational and demonstration purposes.

## 👥 Contributors

- Development Team

## 📞 Support

For issues and questions, please create an issue in the repository.

---

**Built with ❤️ using .NET 9.0 and Blazor WebAssembly**

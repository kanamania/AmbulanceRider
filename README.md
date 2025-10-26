# 🚑 AmbulanceRider - Emergency Medical Dispatch System

A full-stack web application for managing emergency medical dispatch operations with user management, vehicle tracking, and route planning.

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

3. **Routes Management**
   - Route planning with start/end locations
   - Distance and duration tracking
   - Description and notes
   - Table-based list view

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
│   │   └── Pages/               # CRUD pages
│   │       ├── Users/           # User management
│   │       ├── Vehicles/        # Vehicle management
│   │       └── Routes/          # Route management
│   ├── Models/                  # DTOs
│   └── Services/                # API communication
│
└── docker-compose.yaml          # Container orchestration
```

## 🚀 Quick Start

### Prerequisites
- .NET 9.0 SDK
- Docker Desktop (for PostgreSQL)
- Visual Studio 2022 or VS Code

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
- **routes** - Emergency routes

### Relationships
- Users ↔ Roles (Many-to-Many)
- Users ↔ Vehicles (Many-to-Many via vehicle_drivers)
- Vehicles → VehicleTypes (One-to-Many)

## 🔐 Authentication & Authorization

### JWT Token Authentication
The API uses JWT Bearer tokens for authentication. Include the token in requests:

```http
Authorization: Bearer <your-token-here>
```

### Roles & Permissions

| Role | Users | Vehicles | Routes |
|------|-------|----------|--------|
| **Admin** | Full CRUD | Full CRUD | Full CRUD |
| **Dispatcher** | Read | Create, Read, Update | Create, Read, Update |
| **Driver** | Read | Read | Read |
| **User** | Read | Read | Read |

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

### Routes
- `GET /api/routes` - Get all routes
- `GET /api/routes/{id}` - Get route by ID
- `POST /api/routes` - Create route (Admin, Dispatcher)
- `PUT /api/routes/{id}` - Update route (Admin, Dispatcher)
- `DELETE /api/routes/{id}` - Delete route (Admin)

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

## 📈 Future Enhancements

- [ ] Real-time notifications with SignalR
- [ ] GPS tracking integration
- [ ] Mobile app (MAUI)
- [ ] Advanced reporting and analytics
- [ ] Multi-language support
- [ ] Email notifications
- [ ] Audit logging
- [ ] Performance monitoring
- [ ] Unit and integration tests
- [ ] CI/CD pipeline

## 📄 License

This project is for educational and demonstration purposes.

## 👥 Contributors

- Development Team

## 📞 Support

For issues and questions, please create an issue in the repository.

---

**Built with ❤️ using .NET 9.0 and Blazor WebAssembly**

# 🚀 Quick Start Guide

## Option 1: Docker (Recommended)

### Start Everything
```bash
docker-compose up --build
```

**Access:**
- API + Swagger: http://localhost:5000
- Blazor App: http://localhost:8080
- PostgreSQL: localhost:5432

### Stop Everything
```bash
docker-compose down
```

## Option 2: Local Development

### 1. Start Database
```bash
docker-compose up db -d
```

### 2. Run API (Terminal 1)
```bash
cd AmbulanceRider.API
dotnet run
```
API will be at: http://localhost:5000

### 3. Run Blazor (Terminal 2)
```bash
cd AmbulanceRider
dotnet run
```

## First Time Setup

### Create Admin User

Since there's no seed data yet, you have two options:

**Option A: Via Swagger UI**
1. Go to http://localhost:5000
2. Find `POST /api/users`
3. Click "Try it out"
4. Use this JSON:
```json
{
  "firstName": "Admin",
  "lastName": "User",
  "email": "admin@ambulancerider.com",
  "phoneNumber": "+1234567890",
  "password": "Admin123!",
  "roleIds": [1]
}
```
5. Click "Execute"

**Option B: Via curl**
```bash
curl -X POST http://localhost:5000/api/users \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "Admin",
    "lastName": "User",
    "email": "admin@ambulancerider.com",
    "phoneNumber": "+1234567890",
    "password": "Admin123!",
    "roleIds": [1]
  }'
```

### Login

**Via Swagger:**
1. Go to `POST /api/auth/login`
2. Use:
```json
{
  "email": "admin@ambulancerider.com",
  "password": "Admin123!"
}
```
3. Copy the token from response
4. Click "Authorize" button at top
5. Enter: `Bearer <paste-token-here>`

**Via curl:**
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@ambulancerider.com",
    "password": "Admin123!"
  }'
```

## Testing the Application

### 1. Create a Vehicle
Navigate to: http://localhost:8080/vehicles
- Click "Add New Vehicle"
- Fill in: Name, Image URL (optional), Types
- Click "Create Vehicle"

### 2. Create a Route
Navigate to: http://localhost:8080/routes
- Click "Add New Route"
- Fill in route details
- Click "Create Route"

### 3. Create More Users
Navigate to: http://localhost:8080/users
- Click "Add New User"
- Assign roles
- Click "Create User"

## Common Commands

### View Logs
```bash
# API logs
docker-compose logs api

# Database logs
docker-compose logs db

# All logs
docker-compose logs -f
```

### Reset Database
```bash
docker-compose down -v
docker-compose up db -d
cd AmbulanceRider.API
dotnet ef database update
```

### Build Projects
```bash
# Build API
cd AmbulanceRider.API
dotnet build

# Build Blazor
cd AmbulanceRider
dotnet build

# Build both
dotnet build
```

## Troubleshooting

### Port Already in Use
```bash
# Change ports in docker-compose.yaml
ports:
  - "5001:8080"  # API (change 5000 to 5001)
  - "8081:80"    # Blazor (change 8080 to 8081)
```

### Database Connection Failed
```bash
# Check if PostgreSQL is running
docker ps | grep postgres

# Restart database
docker-compose restart db
```

### API Not Responding
```bash
# Check API logs
docker-compose logs api

# Restart API
docker-compose restart api
```

## Next Steps

1. ✅ Create admin user
2. ✅ Login and get JWT token
3. ✅ Test CRUD operations via Swagger
4. ✅ Test Blazor UI
5. ✅ Create sample data (vehicles, routes, users)

## Default Role IDs

- 1 = Admin
- 2 = Driver
- 3 = Dispatcher
- 4 = User

## Useful URLs

- **Swagger UI:** http://localhost:5000
- **Blazor App:** http://localhost:8080
- **Users Page:** http://localhost:8080/users
- **Vehicles Page:** http://localhost:8080/vehicles
- **Routes Page:** http://localhost:8080/routes

---

**Happy Coding! 🎉**

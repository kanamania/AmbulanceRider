# Testing Documentation

**Last Updated:** 2025-12-06T18:45:00+03:00  
**Version:** 0.0.21

---

## Testing Status Overview

| Test Type | Status | Coverage |
|-----------|--------|----------|
| Unit Tests | ❌ Not Implemented | 0% |
| Integration Tests | ❌ Not Implemented | 0% |
| End-to-End Tests | ❌ Not Implemented | 0% |
| Manual Testing | ✅ Ongoing | N/A |
| Performance Tests | ❌ Not Implemented | 0% |
| Security Tests | ❌ Not Implemented | 0% |

**Current Testing Approach:** Manual testing via Swagger UI and HTTP/curl requests.

---

## Testing Strategy

### Recommended Test Pyramid

```
        /\
       /  \      E2E Tests (10%)
      /----\     - Critical user journeys
     /      \    - Cross-browser testing
    /--------\   Integration Tests (20%)
   /          \  - API endpoint testing
  /------------\ - Database integration
 /              \ Unit Tests (70%)
/----------------\ - Service logic
                   - Validation rules
                   - Business logic
```

### Test Categories

| Category | Scope | Tools |
|----------|-------|-------|
| **Unit Tests** | Individual methods/classes | xUnit, Moq, FluentAssertions |
| **Integration Tests** | API endpoints, database | xUnit, TestContainers, WebApplicationFactory |
| **Component Tests** | Blazor components | bUnit |
| **E2E Tests** | Full user workflows | Playwright, Selenium |
| **Performance Tests** | Load, stress testing | k6, NBomber |
| **Security Tests** | Vulnerability scanning | OWASP ZAP, SonarQube |

---

## Manual Testing Procedures

### Authentication Testing

| Test Case | Steps | Expected Result |
|-----------|-------|-----------------|
| Valid Login | POST /api/auth/login with valid credentials | 200 OK, JWT token returned |
| Invalid Password | POST /api/auth/login with wrong password | 401 Unauthorized |
| Invalid Email | POST /api/auth/login with non-existent email | 401 Unauthorized |
| Token Refresh | POST /api/auth/refresh with valid refresh token | 200 OK, new tokens |
| Expired Token | Request with expired JWT | 401 Unauthorized |
| Logout | POST /api/auth/logout | 200 OK, token invalidated |

### Trip Management Testing

| Test Case | Steps | Expected Result |
|-----------|-------|-----------------|
| Create Trip | POST /api/trips with valid data | 201 Created |
| Create Trip (Invalid) | POST /api/trips missing required fields | 400 Bad Request |
| Get All Trips | GET /api/trips | 200 OK, list of trips |
| Get Trip by ID | GET /api/trips/{id} | 200 OK, trip details |
| Update Trip | PUT /api/trips/{id} | 200 OK, updated trip |
| Delete Trip | DELETE /api/trips/{id} | 204 No Content |
| Approve Trip | POST /api/trips/{id}/approve | 200 OK, status updated |
| Reject Trip | POST /api/trips/{id}/reject | 200 OK, status updated |
| Complete Trip | POST /api/trips/{id}/complete | 200 OK, status updated |
| Cancel Trip | POST /api/trips/{id}/cancel | 200 OK, status updated |

### Invoice Testing

| Test Case | Steps | Expected Result |
|-----------|-------|-----------------|
| Create Invoice | POST /api/invoices | 201 Created |
| Download PDF | GET /api/invoices/{id}/pdf | 200 OK, PDF file |
| Download Excel | GET /api/invoices/{id}/excel | 200 OK, Excel file |
| Mark as Paid | PUT /api/invoices/{id}/mark-paid | 200 OK |
| Send Email | POST /api/invoices/{id}/send-email | 200 OK |

### User Management Testing

| Test Case | Steps | Expected Result |
|-----------|-------|-----------------|
| Create User | POST /api/users | 201 Created |
| Get Users | GET /api/users | 200 OK, user list |
| Update User | PUT /api/users/{id} | 200 OK |
| Delete User | DELETE /api/users/{id} | 204 No Content |
| Assign Role | PUT /api/users/{id}/roles | 200 OK |

---

## API Testing with curl

### Login
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@example.com","password":"Admin123!"}'
```

### Get Trips (Authenticated)
```bash
curl -X GET http://localhost:5000/api/trips \
  -H "Authorization: Bearer {token}"
```

### Create Trip
```bash
curl -X POST http://localhost:5000/api/trips \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {token}" \
  -d '{
    "name": "Test Trip",
    "fromLatitude": -6.7924,
    "fromLongitude": 39.2083,
    "toLatitude": -6.8000,
    "toLongitude": 39.2500
  }'
```

---

## Test Environment Setup

### Prerequisites
- .NET 9.0 SDK
- Docker Desktop
- PostgreSQL (via Docker)

### Running Test Database
```bash
docker-compose up db -d
```

### Environment Variables
```
ASPNETCORE_ENVIRONMENT=Testing
ConnectionStrings__DefaultConnection=Host=localhost;Port=5433;Database=ambulance_rider_test;...
```

---

## Planned Test Implementation

### Phase 1: Unit Tests (Priority: High)

**Services to Test:**
- [ ] AuthService
- [ ] TripService
- [ ] InvoiceService
- [ ] UserService
- [ ] VehicleService
- [ ] PricingService
- [ ] TelemetryService

**Test Coverage Goals:**
- Business logic: 90%
- Validation rules: 100%
- Error handling: 80%

### Phase 2: Integration Tests (Priority: High)

**Controllers to Test:**
- [ ] AuthController
- [ ] TripsController
- [ ] InvoiceController
- [ ] UsersController
- [ ] VehiclesController
- [ ] PricingController

**Database Tests:**
- [ ] Repository operations
- [ ] Migration verification
- [ ] Seed data validation

### Phase 3: Component Tests (Priority: Medium)

**Blazor Components:**
- [ ] Login page
- [ ] Trip list/create/edit
- [ ] Invoice list/create/detail
- [ ] Dashboard widgets
- [ ] Navigation menu

### Phase 4: E2E Tests (Priority: Low)

**User Journeys:**
- [ ] Complete login flow
- [ ] Create and complete a trip
- [ ] Generate and download invoice
- [ ] User management workflow

---

## Test Data

### Test Users

| Email | Password | Role |
|-------|----------|------|
| admin@test.com | Admin123! | Admin |
| dispatcher@test.com | Dispatcher123! | Dispatcher |
| driver@test.com | Driver123! | Driver |
| user@test.com | User123! | User |

### Test Vehicles

| Name | Plate | Type |
|------|-------|------|
| Ambulance 1 | ABC-123 | Ambulance |
| Van 1 | XYZ-789 | Medical Van |

### Test Locations

| Name | Latitude | Longitude |
|------|----------|-----------|
| Hospital A | -6.7924 | 39.2083 |
| Clinic B | -6.8000 | 39.2500 |

---

## Known Testing Gaps

### Critical Gaps

| Area | Gap | Risk |
|------|-----|------|
| Authentication | No token expiry tests | Security |
| Authorization | Role permission tests missing | Security |
| Input Validation | No boundary testing | Data integrity |
| Error Handling | No exception tests | Stability |
| Concurrency | No race condition tests | Data corruption |

### Medium Priority Gaps

| Area | Gap | Risk |
|------|-----|------|
| Performance | No load testing | Scalability |
| Database | No migration rollback tests | Deployment |
| File Upload | No size/type validation tests | Security |
| Email | No email delivery tests | Functionality |

---

## Continuous Integration

### Planned CI Pipeline

```yaml
# .github/workflows/test.yml (planned)
name: Test Pipeline

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '9.0.x'
      - name: Restore dependencies
        run: dotnet restore
      - name: Build
        run: dotnet build --no-restore
      - name: Run Unit Tests
        run: dotnet test --no-build --verbosity normal
      - name: Upload Coverage
        uses: codecov/codecov-action@v3
```

---

## Test Reporting

### Metrics to Track

| Metric | Target | Current |
|--------|--------|---------|
| Code Coverage | 70% | 0% |
| Test Pass Rate | 100% | N/A |
| Test Execution Time | < 5 min | N/A |
| Flaky Test Rate | < 1% | N/A |

### Reporting Tools (Planned)
- Code coverage: Coverlet + Codecov
- Test results: xUnit XML reports
- Dashboard: GitHub Actions summary

---

## Security Testing Checklist

| Test | Status | Tool |
|------|--------|------|
| SQL Injection | ❌ Pending | Manual/OWASP ZAP |
| XSS | ❌ Pending | Manual/OWASP ZAP |
| CSRF | ❌ Pending | Manual |
| Authentication Bypass | ❌ Pending | Manual |
| Authorization Bypass | ❌ Pending | Manual |
| Sensitive Data Exposure | ❌ Pending | Manual |
| Rate Limiting | ❌ Pending | Manual |
| JWT Vulnerabilities | ❌ Pending | jwt.io, Manual |

---

## Performance Testing Baseline

### Target Metrics

| Endpoint | Target Response Time | Target Throughput |
|----------|---------------------|-------------------|
| GET /api/trips | < 200ms | 100 req/s |
| POST /api/trips | < 500ms | 50 req/s |
| GET /api/auth/login | < 300ms | 50 req/s |
| GET /api/invoices/{id}/pdf | < 2s | 10 req/s |

### Load Test Scenarios (Planned)

| Scenario | Users | Duration | Goal |
|----------|-------|----------|------|
| Smoke Test | 5 | 1 min | Verify basic functionality |
| Load Test | 50 | 10 min | Normal load handling |
| Stress Test | 200 | 5 min | Find breaking point |
| Soak Test | 30 | 1 hour | Memory leak detection |

---

## Recommendations

### Immediate Actions
1. Create `AmbulanceRider.Tests` project
2. Add xUnit and Moq packages
3. Write tests for critical services (Auth, Trip, Invoice)
4. Set up code coverage reporting

### Short-term Goals
1. Achieve 50% code coverage
2. Add integration tests for all controllers
3. Set up CI pipeline with automated testing

### Long-term Goals
1. Achieve 70%+ code coverage
2. Add E2E tests with Playwright
3. Implement performance testing
4. Add security scanning to CI pipeline

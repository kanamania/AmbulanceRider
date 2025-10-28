# AmbulanceRider API - Swagger Documentation Guide

## Overview

The AmbulanceRider API now includes comprehensive Swagger/OpenAPI documentation accessible at the root URL when running in development mode.

## Accessing Swagger UI

1. **Start the API**:
   ```bash
   cd AmbulanceRider.API
   dotnet run
   ```

2. **Open Swagger UI**:
   - Navigate to: `http://localhost:5000` or `https://localhost:5001`
   - The Swagger UI will load automatically at the root path

## Features Implemented

### 1. Enhanced API Information
- **Comprehensive Description**: Detailed overview of all API capabilities
- **Contact Information**: Support team details
- **License Information**: API usage terms
- **Version Information**: API version tracking

### 2. JWT Authentication Integration
- **Authorize Button**: Click the green "Authorize" button in the top-right
- **Token Input**: Enter `Bearer {your-token}` after logging in
- **Automatic Headers**: All subsequent requests include authentication
- **Token Format**: JWT Bearer tokens with proper validation

### 3. Detailed Endpoint Documentation
Each endpoint now includes:
- **Summary**: Brief description of the endpoint
- **Detailed Remarks**: Extended information about usage
- **Parameters**: Full parameter documentation with types
- **Request Examples**: Sample request bodies with realistic data
- **Response Codes**: All possible HTTP status codes
- **Response Examples**: Sample responses for each status code
- **Authorization Requirements**: Which roles can access the endpoint

### 4. Request/Response Examples
Automatic examples for common DTOs:
- **LoginDto**: Sample login credentials
- **RegisterDto**: User registration example
- **CreateTripDto**: Trip creation with all fields
- **CreateVehicleDto**: Vehicle registration
- **CreateLocationDto**: Location with coordinates
- **UpdateTripStatusDto**: Status update example

### 5. Enhanced UI Features
- **Custom Styling**: Professional red/white theme matching ambulance services
- **Filter Functionality**: Search/filter endpoints
- **Deep Linking**: Direct links to specific endpoints
- **Request Duration**: See how long each request takes
- **Collapsible Sections**: Organized by controller tags
- **Model Schemas**: Expandable DTO definitions

## How to Use Swagger UI

### Step 1: Authentication
1. Scroll to the **Authentication** section
2. Expand `POST /api/auth/login`
3. Click "Try it out"
4. Use test credentials:
   ```json
   {
     "email": "admin@ambulancerider.com",
     "password": "Admin@123"
   }
   ```
5. Click "Execute"
6. Copy the `accessToken` from the response
7. Click the green "Authorize" button at the top
8. Enter: `Bearer {paste-your-token-here}`
9. Click "Authorize" then "Close"

### Step 2: Test Endpoints
1. Navigate to any endpoint (e.g., `GET /api/trips`)
2. Click "Try it out"
3. Fill in any required parameters
4. Click "Execute"
5. View the response below

### Step 3: Explore Models
1. Scroll to the bottom of the page
2. Click on "Schemas" section
3. Expand any model to see its structure
4. View required fields, data types, and examples

## API Sections

### Authentication (`/api/auth`)
- **POST /register**: Create new user account
- **POST /login**: Authenticate and get JWT tokens
- **POST /refresh**: Refresh expired access token
- **GET /me**: Get current user profile
- **POST /logout**: Invalidate refresh token
- **POST /forgot-password**: Request password reset
- **POST /reset-password**: Reset password with token

### Trips (`/api/trips`)
- **GET /trips**: List all trips
- **GET /trips/{id}**: Get trip details
- **POST /trips**: Create new trip
- **PUT /trips/{id}**: Update trip
- **DELETE /trips/{id}**: Delete trip
- **GET /trips/status/{status}**: Filter by status
- **GET /trips/pending**: Get pending approvals
- **POST /trips/{id}/approve**: Approve trip
- **POST /trips/{id}/start**: Start trip
- **POST /trips/{id}/complete**: Complete trip
- **PUT /trips/{id}/status**: Update trip status
- **GET /trips/{id}/status-logs**: Get status history

### Vehicles (`/api/vehicles`)
- **GET /vehicles**: List all vehicles
- **GET /vehicles/{id}**: Get vehicle details
- **POST /vehicles**: Register new vehicle
- **PUT /vehicles/{id}**: Update vehicle
- **DELETE /vehicles/{id}**: Remove vehicle
- **GET /vehicles/types**: Get vehicle types
- **GET /vehicles/available**: Get available vehicles

### Locations (`/api/locations`)
- **GET /locations**: List all locations
- **GET /locations/{id}**: Get location details
- **POST /locations**: Create new location
- **PUT /locations/{id}**: Update location
- **DELETE /locations/{id}**: Remove location

### Users (`/api/users`)
- **GET /users**: List all users
- **GET /users/{id}**: Get user details
- **POST /users**: Create new user
- **PUT /users/{id}**: Update user
- **DELETE /users/{id}**: Remove user
- **GET /users/drivers**: Get all drivers
- **GET /users/role/{role}**: Filter by role

### Analytics (`/api/analytics`)
- **GET /analytics/dashboard**: Dashboard statistics
- **GET /analytics/vehicles/utilization**: Vehicle usage stats
- **GET /analytics/drivers/performance**: Driver performance metrics
- **GET /analytics/trips/summary**: Trip summary reports

### Telemetry (`/api/telemetry`)
- **POST /telemetry**: Log telemetry data
- **GET /telemetry/user/{userId}**: Get user telemetry
- **GET /telemetry/trip/{tripId}**: Get trip telemetry
- **GET /telemetry/analytics**: Telemetry analytics

### Trip Types (`/api/triptypes`)
- **GET /triptypes**: List all trip types
- **GET /triptypes/active**: Get active trip types
- **GET /triptypes/{id}**: Get trip type details
- **POST /triptypes**: Create trip type
- **PUT /triptypes/{id}**: Update trip type
- **DELETE /triptypes/{id}**: Remove trip type

## Default Test Accounts

The system includes pre-seeded test accounts:

| Role | Email | Password |
|------|-------|----------|
| Admin | admin@ambulancerider.com | Admin@123 |
| Dispatcher | dispatcher@ambulancerider.com | Dispatcher@123 |
| Driver | driver@ambulancerider.com | Driver@123 |
| User | user@ambulancerider.com | User@123 |

## Response Codes

### Success Codes
- **200 OK**: Request successful
- **201 Created**: Resource created successfully
- **204 No Content**: Successful deletion

### Client Error Codes
- **400 Bad Request**: Invalid input data
- **401 Unauthorized**: Missing or invalid authentication
- **403 Forbidden**: Insufficient permissions
- **404 Not Found**: Resource doesn't exist
- **409 Conflict**: Resource conflict (e.g., duplicate)

### Server Error Codes
- **500 Internal Server Error**: Server-side error

## Advanced Features

### 1. XML Documentation
All controllers and DTOs include XML comments that appear in Swagger UI:
```csharp
/// <summary>
/// Brief description
/// </summary>
/// <remarks>
/// Detailed information with examples
/// </remarks>
/// <param name="paramName">Parameter description</param>
/// <returns>Return value description</returns>
/// <response code="200">Success description</response>
```

### 2. Swagger Annotations
Enhanced documentation using Swashbuckle annotations:
```csharp
[SwaggerOperation(
    Summary = "Short summary",
    Description = "Detailed description",
    OperationId = "Unique_Operation_Id",
    Tags = new[] { "Tag Name" }
)]
```

### 3. Custom Filters
- **SwaggerDefaultValues**: Adds default values to parameters
- **SwaggerExampleSchemaFilter**: Provides realistic examples for DTOs

### 4. Custom Styling
Custom CSS provides:
- Professional color scheme (red/white for emergency services)
- Improved readability
- Better visual hierarchy
- Enhanced button styling
- Responsive design

## Exporting API Documentation

### Export OpenAPI Specification
1. Navigate to Swagger UI
2. Click on `/swagger/v1/swagger.json` link at the top
3. Save the JSON file
4. Use with tools like Postman, Insomnia, or API clients

### Generate Client Code
Use the OpenAPI spec to generate client libraries:
```bash
# Install OpenAPI Generator
npm install @openapitools/openapi-generator-cli -g

# Generate C# client
openapi-generator-cli generate -i swagger.json -g csharp -o ./client

# Generate TypeScript client
openapi-generator-cli generate -i swagger.json -g typescript-axios -o ./client
```

## Best Practices

### For API Developers
1. **Always add XML comments** to controllers and actions
2. **Use ProducesResponseType** attributes for all response codes
3. **Add SwaggerOperation** for enhanced documentation
4. **Provide examples** in DTO classes
5. **Document all parameters** with clear descriptions
6. **Keep summaries concise** and remarks detailed

### For API Consumers
1. **Read the remarks** for detailed usage information
2. **Check response codes** to handle all scenarios
3. **Use the examples** as templates for requests
4. **Test with Try it out** before implementing
5. **Copy the OpenAPI spec** for client generation
6. **Check authorization requirements** for each endpoint

## Troubleshooting

### Swagger UI Not Loading
- Ensure you're running in Development mode
- Check that the API is running on the correct port
- Clear browser cache and reload

### Authentication Not Working
- Ensure you're using the format: `Bearer {token}`
- Check that the token hasn't expired (24 hours)
- Verify you copied the entire token

### Examples Not Showing
- Ensure XML documentation file is generated (check .csproj)
- Verify the XML file exists in the output directory
- Restart the API after adding new documentation

### Custom CSS Not Applied
- Check that wwwroot/swagger-ui/custom.css exists
- Verify the file is included in the build output
- Clear browser cache

## Production Considerations

### Security
- **Disable Swagger in Production**: Remove Swagger middleware in production
- **API Keys**: Consider additional API key authentication
- **Rate Limiting**: Implement rate limiting for public endpoints
- **CORS**: Configure appropriate CORS policies

### Performance
- **Caching**: Enable response caching where appropriate
- **Compression**: Enable response compression
- **Pagination**: Use pagination for large datasets

### Monitoring
- **Logging**: Log all API requests and errors
- **Metrics**: Track API performance metrics
- **Alerts**: Set up alerts for errors and performance issues

## Additional Resources

- [Swashbuckle Documentation](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)
- [OpenAPI Specification](https://swagger.io/specification/)
- [ASP.NET Core API Documentation](https://docs.microsoft.com/en-us/aspnet/core/tutorials/web-api-help-pages-using-swagger)

## Support

For issues or questions about the API documentation:
- Email: support@ambulancerider.com
- Check the API_DOCUMENTATION.md file for endpoint details
- Review the IMPLEMENTATION_SUMMARY.md for feature information

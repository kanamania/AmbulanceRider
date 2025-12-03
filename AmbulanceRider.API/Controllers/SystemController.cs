using System.Security.Claims;
using AmbulanceRider.API.DTOs;
using AmbulanceRider.API.Models;
using AmbulanceRider.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AmbulanceRider.API.Controllers;

/// <summary>
/// System-wide data endpoints
/// </summary>
/// <remarks>
/// Provides endpoints for retrieving system-wide data including trips, locations, trip types, and vehicles.
/// Access is role-based with Admin/Dispatcher getting all data and Driver/User getting filtered data.
/// </remarks>
[ApiController]
[Route("api/system")]
[Produces("application/json")]
[SwaggerTag("Retrieve system-wide data with role-based access control")]
public class SystemController : ControllerBase
{
    private readonly ITripService _tripService;
    private readonly ILocationService _locationService;
    private readonly ITripTypeService _tripTypeService;
    private readonly IVehicleService _vehicleService;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<SystemController> _logger;

    public SystemController(
        ITripService tripService,
        ILocationService locationService,
        ITripTypeService tripTypeService,
        IVehicleService vehicleService,
        UserManager<User> userManager,
        ILogger<SystemController> logger)
    {
        _tripService = tripService;
        _locationService = locationService;
        _tripTypeService = tripTypeService;
        _vehicleService = vehicleService;
        _userManager = userManager;
        _logger = logger;
    }

    /// <summary>
    /// Get system data with optional filtering (trips, locations, trip types, vehicles)
    /// </summary>
    /// <remarks>
    /// Returns comprehensive system data based on user role:
    /// 
    /// **Admin/Dispatcher:**
    /// - All trips in the system
    /// - All locations
    /// - All trip types
    /// - All vehicles
    /// 
    /// **Driver/User:**
    /// - Only trips created by or assigned to them
    /// - All locations
    /// - All trip types
    /// - All vehicles
    /// 
    /// **Filtering:**
    /// Use query parameters to request only specific data types:
    /// - `includeTrips=true` - Include trips data
    /// - `includeLocations=true` - Include locations data
    /// - `includeTripTypes=true` - Include trip types data
    /// - `includeVehicles=true` - Include vehicles data
    /// 
    /// If no filters are specified, all data is returned.
    /// 
    /// This endpoint is useful for initial data loading, synchronization, and partial updates.
    /// </remarks>
    /// <param name="includeTrips">Include trips in response</param>
    /// <param name="includeLocations">Include locations in response</param>
    /// <param name="includeTripTypes">Include trip types in response</param>
    /// <param name="includeVehicles">Include vehicles in response</param>
    /// <param name="includeDrivers">Include drivers in response</param>
    /// <returns>System data including requested data types</returns>
    /// <response code="200">Data retrieved successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="500">Server error occurred</response>
    [Authorize]
    [HttpGet("data")]
    [ProducesResponseType(typeof(SystemDataResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Get system data with filtering",
        Description = "Retrieve system data with optional filtering for specific data types",
        OperationId = "System_GetData",
        Tags = new[] { "System" }
    )]
    public async Task<ActionResult<SystemDataResponseDto>> GetSystemData(
        [FromQuery] bool? includeTrips = null,
        [FromQuery] bool? includeLocations = null,
        [FromQuery] bool? includeTripTypes = null,
        [FromQuery] bool? includeVehicles = null,
        [FromQuery] bool? includeDrivers = null)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Unauthorized(new { message = "User not found" });
            }

            var roles = await _userManager.GetRolesAsync(user);
            var isAdminOrDispatcher = roles.Any(r => 
                r.Equals("Admin", StringComparison.OrdinalIgnoreCase) || 
                r.Equals("Dispatcher", StringComparison.OrdinalIgnoreCase));

            // If no filters specified, include all data
            var shouldIncludeTrips = includeTrips ?? true;
            var shouldIncludeLocations = includeLocations ?? true;
            var shouldIncludeTripTypes = includeTripTypes ?? true;
            var shouldIncludeVehicles = includeVehicles ?? true;
            var shouldIncludeDrivers = includeDrivers ?? true;

            var response = new SystemDataResponseDto();

            // Get trips based on role (if requested)
            if (shouldIncludeTrips)
            {
                var trips = await _tripService.GetAllTripsAsync(userId, isAdminOrDispatcher);
                response.Trips = trips.ToList();
                _logger.LogInformation("Retrieved {Count} trips for user {UserId}", response.Trips.Count, userId);
            }
            
            // Get all locations (if requested)
            if (shouldIncludeLocations)
            {
                var locations = await _locationService.GetAllLocationsAsync();
                response.Locations = locations.ToList();
                _logger.LogInformation("Retrieved {Count} locations", response.Locations.Count);
            }
            
            // Get all trip types (if requested)
            if (shouldIncludeTripTypes)
            {
                var tripTypes = await _tripTypeService.GetAllTripTypesAsync();
                response.TripTypes = tripTypes.ToList();
                _logger.LogInformation("Retrieved {Count} trip types", response.TripTypes.Count);
            }
            
            // Get all vehicles (if requested)
            if (shouldIncludeVehicles)
            {
                var vehicles = await _vehicleService.GetAllVehiclesAsync();
                response.Vehicles = vehicles.ToList();
                _logger.LogInformation("Retrieved {Count} vehicles", response.Vehicles.Count);
            }

            // Get all drivers (if requested)
            if (shouldIncludeDrivers)
            {
                var drivers = await _userManager.GetUsersInRoleAsync("Driver");
                response.Drivers = drivers.Select(u => new UserDto
                {
                    Id = u.Id.ToString(),
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Name = u.FirstName + " " + u.LastName,
                    Email = u.Email!,
                    PhoneNumber = u.PhoneNumber,
                    ImagePath = u.ImagePath,
                    ImageUrl = u.ImageUrl
                }).ToList();
                _logger.LogInformation("Retrieved {Count} drivers", response.Drivers.Count);
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving system data for user");
            return StatusCode(500, new { message = "An error occurred while retrieving system data" });
        }
    }
}

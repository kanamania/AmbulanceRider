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
    /// Get all system data (trips, locations, trip types, vehicles)
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
    /// This endpoint is useful for initial data loading and synchronization.
    /// </remarks>
    /// <returns>System data including trips, locations, trip types, and vehicles</returns>
    /// <response code="200">Data retrieved successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="500">Server error occurred</response>
    [Authorize]
    [HttpGet("data")]
    [ProducesResponseType(typeof(SystemDataResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Get system data",
        Description = "Retrieve all system data with role-based filtering",
        OperationId = "System_GetData",
        Tags = new[] { "System" }
    )]
    public async Task<ActionResult<SystemDataResponseDto>> GetSystemData()
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

            // Get trips based on role
            var trips = await _tripService.GetAllTripsAsync(userId, isAdminOrDispatcher);
            
            // Get all locations (available to all authenticated users)
            var locations = await _locationService.GetAllLocationsAsync();
            
            // Get all trip types (available to all authenticated users)
            var tripTypes = await _tripTypeService.GetAllTripTypesAsync();
            
            // Get all vehicles (available to all authenticated users)
            var vehicles = await _vehicleService.GetAllVehiclesAsync();

            var response = new SystemDataResponseDto
            {
                Trips = trips.ToList(),
                Locations = locations.ToList(),
                TripTypes = tripTypes.ToList(),
                Vehicles = vehicles.ToList()
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving system data for user");
            return StatusCode(500, new { message = "An error occurred while retrieving system data" });
        }
    }
}

using System.Security.Claims;
using AmbulanceRider.API.DTOs;
using AmbulanceRider.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AmbulanceRider.API.Controllers;

[ApiController]
[Route("api/telemetry")]
[Produces("application/json")]
public class TelemetryController : ControllerBase
{
    private readonly ITelemetryService _telemetryService;
    private readonly ILogger<TelemetryController> _logger;

    public TelemetryController(ITelemetryService telemetryService, ILogger<TelemetryController> logger)
    {
        _telemetryService = telemetryService;
        _logger = logger;
    }

    /// <summary>
    /// Log a single telemetry event
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> LogTelemetry([FromBody] TelemetryEventDto telemetryEvent)
    {
        try
        {
            var userId = GetCurrentUserId();
            
            await _telemetryService.LogTelemetryAsync(
                telemetryEvent.EventType,
                telemetryEvent.Telemetry,
                userId,
                telemetryEvent.EventDetails
            );

            return Ok(new { message = "Telemetry logged successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log telemetry");
            return BadRequest(new { message = "Failed to log telemetry" });
        }
    }

    /// <summary>
    /// Log multiple telemetry events in a batch (for timeseries data)
    /// </summary>
    [HttpPost("batch")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> LogBatchTelemetry([FromBody] BatchTelemetryDto batchTelemetry)
    {
        try
        {
            var userId = GetCurrentUserId();
            
            var telemetryBatch = batchTelemetry.Events.Select(e => 
                (e.EventType, e.Telemetry, userId, e.EventDetails)
            );

            var count = await _telemetryService.LogBatchTelemetryAsync(telemetryBatch);

            return Ok(new { message = $"Batch telemetry logged successfully", count });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log batch telemetry");
            return BadRequest(new { message = "Failed to log batch telemetry" });
        }
    }

    /// <summary>
    /// Get timeseries telemetry data within a time range
    /// </summary>
    [Authorize(Roles = "Admin,Dispatcher")]
    [HttpPost("timeseries")]
    [ProducesResponseType(typeof(List<TelemetryTimeseriesDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<TelemetryTimeseriesDto>>> GetTimeseries([FromBody] TimeseriesQueryDto query)
    {
        try
        {
            var telemetries = await _telemetryService.GetTimeseriesDataAsync(
                query.StartTime,
                query.EndTime,
                query.EventType,
                query.Limit
            );

            var result = telemetries.Select(t => new TelemetryTimeseriesDto
            {
                Id = t.Id,
                EventType = t.EventType,
                EventDetails = t.EventDetails,
                UserId = t.UserId,
                UserName = t.User != null ? $"{t.User.FirstName} {t.User.LastName}" : null,
                Latitude = t.Latitude,
                Longitude = t.Longitude,
                Speed = t.Speed,
                BatteryLevel = t.BatteryLevel,
                CreatedAt = t.CreatedAt,
                EventTimestamp = t.EventTimestamp
            }).ToList();

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve timeseries data");
            return BadRequest(new { message = "Failed to retrieve timeseries data" });
        }
    }

    /// <summary>
    /// Get timeseries telemetry data for a specific user
    /// </summary>
    [Authorize]
    [HttpGet("user/{userId}/timeseries")]
    [ProducesResponseType(typeof(List<TelemetryTimeseriesDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<TelemetryTimeseriesDto>>> GetUserTimeseries(
        Guid userId,
        [FromQuery] DateTime startTime,
        [FromQuery] DateTime endTime,
        [FromQuery] string? eventType = null)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("Dispatcher");

            // Users can only view their own data unless they're admin/dispatcher
            if (currentUserId != userId && !isAdmin)
            {
                return Forbid();
            }

            var telemetries = await _telemetryService.GetUserTimeseriesAsync(
                userId,
                startTime,
                endTime,
                eventType
            );

            var result = telemetries.Select(t => new TelemetryTimeseriesDto
            {
                Id = t.Id,
                EventType = t.EventType,
                EventDetails = t.EventDetails,
                UserId = t.UserId,
                UserName = t.User != null ? $"{t.User.FirstName} {t.User.LastName}" : null,
                Latitude = t.Latitude,
                Longitude = t.Longitude,
                Speed = t.Speed,
                BatteryLevel = t.BatteryLevel,
                CreatedAt = t.CreatedAt,
                EventTimestamp = t.EventTimestamp
            }).ToList();

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve user timeseries data for user {UserId}", userId);
            return BadRequest(new { message = "Failed to retrieve user timeseries data" });
        }
    }

    /// <summary>
    /// Get current user's telemetry timeseries
    /// </summary>
    [Authorize]
    [HttpGet("me/timeseries")]
    [ProducesResponseType(typeof(List<TelemetryTimeseriesDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<TelemetryTimeseriesDto>>> GetMyTimeseries(
        [FromQuery] DateTime startTime,
        [FromQuery] DateTime endTime,
        [FromQuery] string? eventType = null)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized();
            }

            var telemetries = await _telemetryService.GetUserTimeseriesAsync(
                userId.Value,
                startTime,
                endTime,
                eventType
            );

            var result = telemetries.Select(t => new TelemetryTimeseriesDto
            {
                Id = t.Id,
                EventType = t.EventType,
                EventDetails = t.EventDetails,
                UserId = t.UserId,
                Latitude = t.Latitude,
                Longitude = t.Longitude,
                Speed = t.Speed,
                BatteryLevel = t.BatteryLevel,
                CreatedAt = t.CreatedAt,
                EventTimestamp = t.EventTimestamp
            }).ToList();

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve current user timeseries data");
            return BadRequest(new { message = "Failed to retrieve timeseries data" });
        }
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            return null;
        }

        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}

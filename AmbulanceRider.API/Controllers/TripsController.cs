using AmbulanceRider.API.DTOs;
using AmbulanceRider.API.Models;
using AmbulanceRider.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AmbulanceRider.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TripsController : ControllerBase
{
    private readonly ITripService _tripService;

    public TripsController(ITripService tripService)
    {
        _tripService = tripService;
    }

    /// <summary>
    /// Get all trips
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TripDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TripDto>>> GetAll()
    {
        var trips = await _tripService.GetAllTripsAsync();
        return Ok(trips);
    }

    /// <summary>
    /// Get trip by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TripDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TripDto>> GetById(int id)
    {
        var trip = await _tripService.GetTripByIdAsync(id);
        if (trip == null)
            return NotFound();

        return Ok(trip);
    }

    /// <summary>
    /// Get trips by status
    /// </summary>
    [HttpGet("status/{status}")]
    [ProducesResponseType(typeof(IEnumerable<TripDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TripDto>>> GetByStatus(TripStatus status)
    {
        var trips = await _tripService.GetTripsByStatusAsync(status);
        return Ok(trips);
    }

    /// <summary>
    /// Get pending trips (awaiting approval)
    /// </summary>
    [HttpGet("pending")]
    [Authorize(Roles = "Admin,Dispatcher")]
    [ProducesResponseType(typeof(IEnumerable<TripDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TripDto>>> GetPending()
    {
        var trips = await _tripService.GetPendingTripsAsync();
        return Ok(trips);
    }

    /// <summary>
    /// Get trips by driver
    /// </summary>
    [HttpGet("driver/{driverId}")]
    [ProducesResponseType(typeof(IEnumerable<TripDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TripDto>>> GetByDriver(Guid driverId)
    {
        var trips = await _tripService.GetTripsByDriverAsync(driverId);
        return Ok(trips);
    }

    /// <summary>
    /// Create a new trip
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Dispatcher")]
    [ProducesResponseType(typeof(TripDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TripDto>> Create([FromBody] CreateTripDto createTripDto)
    {
        try
        {
            var trip = await _tripService.CreateTripAsync(createTripDto);
            return CreatedAtAction(nameof(GetById), new { id = trip.Id }, trip);
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing trip (only pending trips)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Dispatcher")]
    [ProducesResponseType(typeof(TripDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TripDto>> Update(int id, [FromBody] UpdateTripDto updateTripDto)
    {
        try
        {
            var trip = await _tripService.UpdateTripAsync(id, updateTripDto);
            return Ok(trip);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Approve or reject a trip
    /// </summary>
    [HttpPost("{id}/approve")]
    [Authorize(Roles = "Admin,Dispatcher")]
    [ProducesResponseType(typeof(TripDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TripDto>> Approve(int id, [FromBody] ApproveTripDto approveTripDto)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid approverId))
            {
                return Unauthorized(new { message = "Invalid user token" });
            }

            var trip = await _tripService.ApproveTripAsync(id, approveTripDto, approverId);
            return Ok(trip);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Start a trip (change status to in-progress)
    /// </summary>
    [HttpPost("{id}/start")]
    [ProducesResponseType(typeof(TripDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TripDto>> Start(int id, [FromBody] StartTripDto startTripDto)
    {
        try
        {
            var trip = await _tripService.StartTripAsync(id, startTripDto);
            return Ok(trip);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Complete a trip
    /// </summary>
    [HttpPost("{id}/complete")]
    [ProducesResponseType(typeof(TripDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TripDto>> Complete(int id, [FromBody] CompleteTripDto completeTripDto)
    {
        try
        {
            var trip = await _tripService.CompleteTripAsync(id, completeTripDto);
            return Ok(trip);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Update trip status (unified endpoint for all status updates)
    /// </summary>
    [HttpPut("{id}/status")]
    [ProducesResponseType(typeof(TripDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<TripDto>> UpdateStatus(int id, [FromBody] UpdateTripStatusDto updateDto)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Invalid user token" });
            }

            var isAdminOrDispatcher = User.IsInRole("Admin") || User.IsInRole("Dispatcher");
            var trip = await _tripService.UpdateTripStatusAsync(id, updateDto, userId, isAdminOrDispatcher);
            return Ok(trip);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
    }

    /// <summary>
    /// Cancel a trip
    /// </summary>
    [HttpPost("{id}/cancel")]
    [Authorize(Roles = "Admin,Dispatcher")]
    [ProducesResponseType(typeof(TripDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TripDto>> Cancel(int id)
    {
        try
        {
            var trip = await _tripService.CancelTripAsync(id);
            return Ok(trip);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Delete a trip (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _tripService.DeleteTripAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Get status change history for a trip
    /// </summary>
    [HttpGet("{id}/status-logs")]
    [ProducesResponseType(typeof(IEnumerable<TripStatusLogDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<TripStatusLogDto>>> GetStatusLogs(int id)
    {
        try
        {
            var logs = await _tripService.GetTripStatusLogsAsync(id);
            return Ok(logs);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}

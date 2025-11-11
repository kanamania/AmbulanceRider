using AmbulanceRider.API.Data;
using AmbulanceRider.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AmbulanceRider.API.Controllers;

/// <summary>
/// Controller for handling notification requests
/// Triggers SignalR broadcasts to connected clients
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(
        INotificationService notificationService,
        ApplicationDbContext context,
        ILogger<NotificationsController> logger)
    {
        _notificationService = notificationService;
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Notify about trip creation
    /// Broadcasts to all admins and dispatchers via SignalR
    /// </summary>
    /// <param name="request">Trip creation notification request</param>
    [HttpPost("trip-created")]
    public async Task<IActionResult> NotifyTripCreated([FromBody] TripNotificationRequest request)
    {
        try
        {
            // Get trip details
            var trip = await _context.Trips
                .Include(t => t.User)
                .Include(t => t.TripType)
                .FirstOrDefaultAsync(t => t.Id == request.TripId);

            if (trip == null)
            {
                return NotFound(new { message = "Trip not found" });
            }

            // Broadcast to admins and dispatchers
            await _notificationService.SendNotificationToGroupAsync(
                "admins",
                "New Trip Created",
                $"Trip #{trip.Id} has been created by {trip.User?.UserName ?? "Unknown"}",
                new
                {
                    tripId = trip.Id,
                    userId = trip.UserId,
                    fromAddress = trip.FromAddress,
                    toAddress = trip.ToAddress,
                    patientName = trip.PatientName,
                    tripType = trip.TripType?.Name,
                    status = trip.Status
                }
            );

            await _notificationService.SendNotificationToGroupAsync(
                "dispatchers",
                "New Trip Created",
                $"Trip #{trip.Id} requires assignment",
                new
                {
                    tripId = trip.Id,
                    userId = trip.UserId,
                    fromAddress = trip.FromAddress,
                    toAddress = trip.ToAddress,
                    patientName = trip.PatientName,
                    tripType = trip.TripType?.Name,
                    status = trip.Status
                }
            );

            // Also send via TripHub
            await _notificationService.SendTripUpdateAsync(
                trip.Id,
                "created",
                new
                {
                    trip.Id,
                    trip.Status,
                    trip.FromAddress,
                    trip.ToAddress,
                    trip.PatientName,
                    CreatedAt = trip.CreatedAt
                }
            );

            _logger.LogInformation("Trip creation notification sent for trip {TripId}", trip.Id);

            return Ok(new { message = "Notification sent successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending trip creation notification for trip {TripId}", request.TripId);
            return StatusCode(500, new { message = "Failed to send notification", error = ex.Message });
        }
    }

    /// <summary>
    /// Notify about trip status change
    /// Broadcasts to relevant users via SignalR
    /// </summary>
    /// <param name="request">Trip status change notification request</param>
    [HttpPost("trip-status-changed")]
    public async Task<IActionResult> NotifyTripStatusChanged([FromBody] TripStatusChangeRequest request)
    {
        try
        {
            // Get trip details
            var trip = await _context.Trips
                .Include(t => t.User)
                .Include(t => t.Vehicle)
                .Include(t => t.TripType)
                .FirstOrDefaultAsync(t => t.Id == request.TripId);

            if (trip == null)
            {
                return NotFound(new { message = "Trip not found" });
            }

            var statusMessage = GetStatusMessage(request.Status, trip);

            // Notify the trip creator
            if (trip.User != null)
            {
                await _notificationService.SendNotificationToUserAsync(
                    trip.UserId.ToString(),
                    "Trip Status Update",
                    statusMessage,
                    new
                    {
                        tripId = trip.Id,
                        status = request.Status,
                        vehicleInfo = trip.Vehicle != null ? $"{trip.Vehicle.Make} {trip.Vehicle.Model} - {trip.Vehicle.LicensePlate}" : null
                    }
                );
            }

            // Notify admins and dispatchers
            await _notificationService.SendNotificationToGroupAsync(
                "admins",
                "Trip Status Changed",
                $"Trip #{trip.Id} status changed to {request.Status}",
                new
                {
                    tripId = trip.Id,
                    status = request.Status,
                    patientName = trip.PatientName
                }
            );

            await _notificationService.SendNotificationToGroupAsync(
                "dispatchers",
                "Trip Status Changed",
                $"Trip #{trip.Id} status changed to {request.Status}",
                new
                {
                    tripId = trip.Id,
                    status = request.Status,
                    patientName = trip.PatientName
                }
            );

            // Send via TripHub
            await _notificationService.SendTripStatusChangeAsync(
                trip.Id,
                trip.Status,
                request.Status,
                new
                {
                    trip.Id,
                    NewStatus = request.Status,
                    Message = statusMessage,
                    trip.PatientName,
                    VehicleInfo = trip.Vehicle != null ? $"{trip.Vehicle.Make} {trip.Vehicle.Model}" : null
                }
            );

            _logger.LogInformation(
                "Trip status change notification sent for trip {TripId}, status: {Status}",
                trip.Id,
                request.Status);

            return Ok(new { message = "Notification sent successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error sending trip status change notification for trip {TripId}",
                request.TripId);
            return StatusCode(500, new { message = "Failed to send notification", error = ex.Message });
        }
    }

    private static string GetStatusMessage(string status, dynamic trip)
    {
        return status.ToLower() switch
        {
            "accepted" or "approved" => $"Your trip has been accepted and assigned to a vehicle",
            "in_progress" => $"Your trip is now in progress",
            "completed" => $"Your trip has been completed",
            "cancelled" or "rejected" => $"Your trip has been cancelled",
            _ => $"Your trip status has been updated to {status}"
        };
    }
}

/// <summary>
/// Request model for trip creation notification
/// </summary>
public class TripNotificationRequest
{
    public int TripId { get; set; }
}

/// <summary>
/// Request model for trip status change notification
/// </summary>
public class TripStatusChangeRequest
{
    public int TripId { get; set; }
    public string Status { get; set; } = string.Empty;
}

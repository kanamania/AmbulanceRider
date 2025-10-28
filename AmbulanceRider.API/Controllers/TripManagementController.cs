using AmbulanceRider.API.DTOs;
using AmbulanceRider.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AmbulanceRider.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TripManagementController : ControllerBase
{
    private readonly ITripManagementService _tripManagementService;
    private readonly ILogger<TripManagementController> _logger;

    public TripManagementController(
        ITripManagementService tripManagementService,
        ILogger<TripManagementController> logger)
    {
        _tripManagementService = tripManagementService;
        _logger = logger;
    }

    #region Bulk Operations

    [HttpPost("bulk-update")]
    [Authorize(Roles = "Admin,Dispatcher")]
    public async Task<IActionResult> BulkUpdateTrips([FromBody] BulkUpdateTripsDto updateDto)
    {
        if (updateDto == null || !updateDto.TripIds.Any())
        {
            return BadRequest("No trip IDs provided");
        }

        var userId = GetCurrentUserId();
        var result = await _tripManagementService.BulkUpdateTripsAsync(updateDto, userId);
        
        if (!result.Success)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { 
                result.Errors, 
                result.UpdatedCount, 
                result.FailedCount 
            });
        }

        return Ok(new { 
            Message = result.Message,
            UpdatedCount = result.UpdatedCount,
            FailedCount = result.FailedCount
        });
    }

    #endregion

    #region GPS Verification

    [HttpPost("{tripId}/verify-location")]
    public async Task<IActionResult> VerifyTripCompletionLocation(int tripId, [FromBody] GpsVerificationDto verificationDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userId = GetCurrentUserId();
        var isVerified = await _tripManagementService.VerifyTripCompletionLocationAsync(tripId, verificationDto, userId);
        
        return Ok(new { IsVerified = isVerified });
    }

    #endregion

    #region Media Uploads

    [HttpPost("{tripId}/upload-media")]
    [RequestSizeLimit(10 * 1024 * 1024)] // 10MB limit
    public async Task<IActionResult> UploadTripMedia(int tripId, [FromForm] TripMediaUploadDto mediaUpload)
    {
        if (mediaUpload?.File == null || mediaUpload.File.Length == 0)
        {
            return BadRequest("No file uploaded");
        }

        if (mediaUpload.MediaType != "photo" && mediaUpload.MediaType != "signature")
        {
            return BadRequest("Invalid media type. Must be 'photo' or 'signature'");
        }

        try
        {
            var userId = GetCurrentUserId();
            var fileUrl = await _tripManagementService.UploadTripMediaAsync(tripId, mediaUpload, userId);
            return Ok(new { FileUrl = fileUrl });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error uploading {mediaUpload.MediaType} for trip {tripId}");
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error uploading {mediaUpload.MediaType}");
        }
    }

    #endregion

    #region Auto-Approval

    [HttpGet("auto-approve/{tripId}")]
    [Authorize(Roles = "Admin,Dispatcher")]
    public async Task<IActionResult> CheckAndAutoApproveTrip(int tripId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var wasAutoApproved = await _tripManagementService.CheckAndAutoApproveTripAsync(tripId, userId);
            return Ok(new { AutoApproved = wasAutoApproved });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error checking auto-approval for trip {tripId}");
            return StatusCode(StatusCodes.Status500InternalServerError, "Error checking auto-approval");
        }
    }

    [HttpGet("auto-approve/criteria")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAutoApproveCriteria()
    {
        try
        {
            var criteria = await _tripManagementService.GetAutoApproveCriteriaAsync();
            return Ok(criteria);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting auto-approval criteria");
            return StatusCode(StatusCodes.Status500InternalServerError, "Error getting auto-approval criteria");
        }
    }

    [HttpPut("auto-approve/criteria")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateAutoApproveCriteria([FromBody] AutoApproveCriteriaDto criteria)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var userId = GetCurrentUserId();
            var success = await _tripManagementService.UpdateAutoApproveCriteriaAsync(criteria, userId);
            
            if (!success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to update auto-approval criteria");
            }
            
            return Ok(criteria);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating auto-approval criteria");
            return StatusCode(StatusCodes.Status500InternalServerError, "Error updating auto-approval criteria");
        }
    }

    #endregion

    #region Route Optimization

    [HttpPost("{tripId}/optimize-route")]
    public async Task<IActionResult> OptimizeRoute(int tripId, [FromBody] RouteOptimizationRequestDto? request = null)
    {
        try
        {
            var optimizedRoute = await _tripManagementService.GetOptimizedRouteForTripAsync(tripId, request);
            return Ok(optimizedRoute);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error optimizing route for trip {tripId}");
            return StatusCode(StatusCodes.Status500InternalServerError, "Error optimizing route");
        }
    }

    [HttpPost("batch-optimize-routes")]
    [Authorize(Roles = "Admin,Dispatcher")]
    public async Task<IActionResult> BatchOptimizeRoutes([FromBody] List<int> tripIds, [FromQuery] bool optimizeForTime = true)
    {
        if (tripIds == null || !tripIds.Any())
        {
            return BadRequest("No trip IDs provided");
        }

        try
        {
            var request = new RouteOptimizationRequestDto
            {
                OptimizeForTime = optimizeForTime
            };
            
            var results = await _tripManagementService.BatchOptimizeRoutesForTripsAsync(tripIds, request);
            return Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error batch optimizing routes for {tripIds.Count} trips");
            return StatusCode(StatusCodes.Status500InternalServerError, "Error optimizing routes");
        }
    }

    #endregion

    #region Helper Methods

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user ID in token");
        }
        return userId;
    }

    #endregion
}

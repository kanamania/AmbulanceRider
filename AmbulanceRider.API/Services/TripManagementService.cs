using System.Transactions;
using AmbulanceRider.API.DTOs;
using AmbulanceRider.API.Models;
using AmbulanceRider.API.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AmbulanceRider.API.Services;

public class TripManagementService : ITripManagementService
{
    private readonly ITripRepository _tripRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAuditService _auditService;
    private readonly IFileStorageService _fileStorageService;
    private readonly IRouteOptimizationService _routeOptimizationService;
    private readonly ILogger<TripManagementService> _logger;
    private readonly IConfiguration _configuration;

    public TripManagementService(
        ITripRepository tripRepository,
        IUserRepository userRepository,
        IAuditService auditService,
        IFileStorageService fileStorageService,
        IRouteOptimizationService routeOptimizationService,
        ILogger<TripManagementService> logger,
        IConfiguration configuration)
    {
        _tripRepository = tripRepository;
        _userRepository = userRepository;
        _auditService = auditService;
        _fileStorageService = fileStorageService;
        _routeOptimizationService = routeOptimizationService;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<BulkOperationResult> BulkUpdateTripsAsync(BulkUpdateTripsDto updateDto, Guid userId)
    {
        var result = new BulkOperationResult();
        
        if (updateDto.TripIds == null || !updateDto.TripIds.Any())
        {
            result.Errors.Add("No trip IDs provided");
            return result;
        }

        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        
        try
        {
            foreach (var tripId in updateDto.TripIds)
            {
                try
                {
                    var trip = await _tripRepository.GetByIdAsync(tripId);
                    if (trip == null)
                    {
                        result.Errors.Add($"Trip {tripId} not found");
                        result.FailedCount++;
                        continue;
                    }

                    var changes = new List<string>();
                    
                    if (!string.IsNullOrEmpty(updateDto.Status) && 
                        Enum.TryParse<TripStatus>(updateDto.Status, out var newStatus))
                    {
                        changes.Add($"Status: {trip.Status} -> {newStatus}");
                        trip.Status = newStatus;
                    }
                    
                    if (updateDto.VehicleId.HasValue)
                    {
                        changes.Add($"Vehicle: {trip.VehicleId} -> {updateDto.VehicleId}");
                        trip.VehicleId = updateDto.VehicleId;
                    }
                    
                    if (updateDto.DriverId.HasValue)
                    {
                        changes.Add($"Driver: {trip.DriverId} -> {updateDto.DriverId}");
                        trip.DriverId = updateDto.DriverId;
                    }
                    
                    if (changes.Any())
                    {
                        trip.UpdatedAt = DateTime.UtcNow;
                        await _tripRepository.UpdateAsync(trip);
                        
                        // Log the changes
                        await _auditService.LogAsync(
                            "Trip",
                            trip.Id,
                            "BulkUpdate",
                            null,
                            string.Join(", ", changes),
                            "Updated in bulk operation",
                            updateDto.Notes
                        );
                        
                        result.UpdatedCount++;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error updating trip {tripId}");
                    result.Errors.Add($"Error updating trip {tripId}: {ex.Message}");
                    result.FailedCount++;
                }
            }
            
            transaction.Complete();
            result.Success = true;
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in bulk update operation");
            result.Errors.Add($"Bulk update failed: {ex.Message}");
            return result;
        }
    }

    public async Task<bool> VerifyTripCompletionLocationAsync(int tripId, GpsVerificationDto verification, Guid userId)
    {
        try
        {
            var trip = await _tripRepository.GetByIdAsync(tripId);
            if (trip == null)
            {
                _logger.LogWarning($"Trip {tripId} not found for GPS verification");
                return false;
            }

            // Store the verification data
            trip.CompletionLatitude = verification.Latitude;
            trip.CompletionLongitude = verification.Longitude;
            trip.CompletionAccuracy = verification.Accuracy;
            trip.UpdatedAt = DateTime.UtcNow;
            
            // Calculate distance from destination
            var distanceFromDestination = CalculateDistance(
                verification.Latitude, verification.Longitude,
                trip.ToLatitude, trip.ToLongitude);
            
            var isWithinAcceptableRange = distanceFromDestination <= (trip.CompletionAccuracy ?? 100) * 2; // Within 2x accuracy
            
            await _tripRepository.UpdateAsync(trip);
            
            // Log the verification
            await _auditService.LogAsync(
                "Trip",
                trip.Id,
                "LocationVerification",
                null,
                $"Verified at {verification.Latitude}, {verification.Longitude} (Accuracy: {verification.Accuracy}m, Distance from destination: {distanceFromDestination}m)",
                isWithinAcceptableRange ? "Within acceptable range" : "Outside acceptable range",
                verification.Notes
            );
            
            return isWithinAcceptableRange;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error verifying trip {tripId} completion location");
            throw new ApplicationException("Failed to verify trip completion location", ex);
        }
    }

    public async Task<string> UploadTripMediaAsync(int tripId, TripMediaUploadDto mediaUpload, Guid userId)
    {
        try
        {
            var trip = await _tripRepository.GetByIdAsync(tripId);
            if (trip == null)
            {
                throw new ArgumentException($"Trip {tripId} not found");
            }
            
            // Validate media type
            if (mediaUpload.MediaType != "photo" && mediaUpload.MediaType != "signature")
            {
                throw new ArgumentException("Invalid media type. Must be 'photo' or 'signature'");
            }
            
            // Generate a unique filename
            var fileExtension = Path.GetExtension(mediaUpload.File.FileName).ToLowerInvariant();
            var fileName = $"trip-{tripId}-{mediaUpload.MediaType}-{DateTime.UtcNow:yyyyMMddHHmmss}{fileExtension}";
            
            // Save the file
            var fileUrl = await _fileStorageService.SaveFileAsync(mediaUpload.File, "trip-media", fileName);
            
            // Update the trip with the media URL
            if (mediaUpload.MediaType == "photo")
            {
                trip.PhotoUrl = fileUrl;
            }
            else if (mediaUpload.MediaType == "signature")
            {
                trip.SignatureUrl = fileUrl;
            }
            
            trip.UpdatedAt = DateTime.UtcNow;
            await _tripRepository.UpdateAsync(trip);
            
            // Log the upload
            await _auditService.LogAsync(
                "Trip",
                trip.Id,
                $"{mediaUpload.MediaType}Upload",
                null,
                $"Uploaded {mediaUpload.MediaType}",
                fileUrl,
                mediaUpload.Notes
            );
            
            return fileUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error uploading {mediaUpload.MediaType} for trip {tripId}");
            throw new ApplicationException($"Failed to upload {mediaUpload.MediaType}", ex);
        }
    }

    public async Task<bool> CheckAndAutoApproveTripAsync(int tripId, Guid userId)
    {
        try
        {
            var trip = await _tripRepository.GetByIdAsync(tripId);
            if (trip == null || trip.Status != TripStatus.Pending)
            {
                return false;
            }
            
            var criteria = await GetAutoApproveCriteriaAsync();
            if (!criteria.EnableForAllTrips && 
                !criteria.AllowedTripTypeIds.Contains(trip.TripTypeId ?? 0))
            {
                return false;
            }
            
            // Check if the trip meets the auto-approval criteria
            var shouldAutoApprove = true;
            var reasons = new List<string>();
            
            // Check if vehicle type is allowed
            if (trip.VehicleId.HasValue && criteria.AllowedVehicleTypeIds.Any())
            {
                var vehicle = await _tripRepository.GetVehicleAsync(trip.VehicleId.Value);
                if (vehicle != null && !criteria.AllowedVehicleTypeIds.Contains(vehicle.VehicleTypeId))
                {
                    shouldAutoApprove = false;
                    reasons.Add("Vehicle type not allowed for auto-approval");
                }
            }
            
            // Check distance criteria if specified
            if (criteria.MaxDistanceMeters.HasValue && 
                trip.EstimatedDistance > criteria.MaxDistanceMeters.Value)
            {
                shouldAutoApprove = false;
                reasons.Add($"Distance ({trip.EstimatedDistance}m) exceeds maximum allowed ({criteria.MaxDistanceMeters}m)");
            }
            
            // Check duration criteria if specified
            if (criteria.MaxDuration.HasValue && 
                trip.EstimatedDuration > criteria.MaxDuration.Value.TotalSeconds)
            {
                shouldAutoApprove = false;
                reasons.Add($"Duration ({trip.EstimatedDuration}s) exceeds maximum allowed ({criteria.MaxDuration.Value.TotalSeconds}s)");
            }
            
            // If all checks pass, auto-approve the trip
            if (shouldAutoApprove)
            {
                trip.Status = TripStatus.Approved;
                trip.ApprovedBy = userId;
                trip.ApprovedAt = DateTime.UtcNow;
                trip.AutoApproved = true;
                trip.UpdatedAt = DateTime.UtcNow;
                
                await _tripRepository.UpdateAsync(trip);
                
                // Log the auto-approval
                await _auditService.LogAsync(
                    "Trip",
                    trip.Id,
                    "AutoApproved",
                    null,
                    "Trip auto-approved based on criteria",
                    string.Join(", ", reasons)
                );
                
                return true;
            }
            
            // Log why auto-approval was not granted
            if (reasons.Any())
            {
                await _auditService.LogAsync(
                    "Trip",
                    trip.Id,
                    "AutoApproveSkipped",
                    null,
                    "Trip did not meet auto-approval criteria",
                    string.Join("; ", reasons)
                );
            }
            
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error checking auto-approval for trip {tripId}");
            return false;
        }
    }

    public async Task<AutoApproveCriteriaDto> GetAutoApproveCriteriaAsync()
    {
        try
        {
            // In a real implementation, this would load from a database or configuration
            return _configuration.GetSection("AutoApproveCriteria").Get<AutoApproveCriteriaDto>() ?? 
                   new AutoApproveCriteriaDto();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting auto-approval criteria");
            return new AutoApproveCriteriaDto();
        }
    }

    public async Task<bool> UpdateAutoApproveCriteriaAsync(AutoApproveCriteriaDto criteria, Guid userId)
    {
        try
        {
            // In a real implementation, this would save to a database or configuration
            _configuration.GetSection("AutoApproveCriteria").Bind(criteria);
            
            // Log the update
            await _auditService.LogAsync(
                "System",
                0, // No specific entity ID for system settings
                "AutoApproveCriteriaUpdated",
                null,
                "Auto-approval criteria updated",
                System.Text.Json.JsonSerializer.Serialize(criteria)
            );
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating auto-approval criteria");
            return false;
        }
    }

    public async Task CheckAndStartScheduledTripsAsync()
    {
        try
        {
            var now = DateTime.UtcNow;
            var timeWindow = TimeSpan.FromMinutes(5); // Start trips up to 5 minutes before scheduled time
            
            // Find trips that are scheduled to start soon and haven't been started yet
            var tripsToStart = await _tripRepository.GetTripsToAutoStartAsync(now, timeWindow);
            
            foreach (var trip in tripsToStart)
            {
                try
                {
                    trip.Status = TripStatus.InProgress;
                    trip.ActualStartTime = now;
                    trip.AutoStarted = true;
                    trip.UpdatedAt = now;
                    
                    await _tripRepository.UpdateAsync(trip);
                    
                    // Log the auto-start
                    await _auditService.LogAsync(
                        "Trip",
                        trip.Id,
                        "AutoStarted",
                        null,
                        "Trip automatically started at scheduled time",
                        $"Scheduled: {trip.ScheduledStartTime:u}"
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error auto-starting trip {trip.Id}");
                    
                    // Log the failure
                    await _auditService.LogAsync(
                        "Trip",
                        trip.Id,
                        "AutoStartFailed",
                        null,
                        "Failed to auto-start trip",
                        ex.Message
                    );
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in CheckAndStartScheduledTripsAsync");
        }
    }

    public async Task<OptimizedRouteDto> GetOptimizedRouteForTripAsync(int tripId, RouteOptimizationRequestDto? request = null)
    {
        try
        {
            var trip = await _tripRepository.GetByIdAsync(tripId);
            if (trip == null)
            {
                throw new ArgumentException($"Trip {tripId} not found");
            }
            
            // If no custom request is provided, create one based on the trip
            request ??= new RouteOptimizationRequestDto
            {
                Waypoints = new List<RouteWaypointDto>
                {
                    new()
                    {
                        Latitude = trip.FromLatitude,
                        Longitude = trip.FromLongitude,
                        IsPickup = true,
                        LocationName = trip.FromLocationName
                    },
                    new()
                    {
                        Latitude = trip.ToLatitude,
                        Longitude = trip.ToLongitude,
                        IsDropoff = true,
                        LocationName = trip.ToLocationName
                    }
                }
            };
            
            // Get the optimized route
            var optimizedRoute = await _routeOptimizationService.GetOptimizedRouteAsync(request);
            
            // Update the trip with the optimized route information
            trip.OptimizedRoute = optimizedRoute.Polyline;
            trip.EstimatedDistance = optimizedRoute.DistanceMeters;
            trip.EstimatedDuration = optimizedRoute.DurationSeconds;
            trip.UpdatedAt = DateTime.UtcNow;
            
            await _tripRepository.UpdateAsync(trip);
            
            // Log the route optimization
            await _auditService.LogAsync(
                "Trip",
                trip.Id,
                "RouteOptimized",
                null,
                "Route optimized for trip",
                $"Distance: {optimizedRoute.DistanceMeters}m, Duration: {optimizedRoute.DurationSeconds}s"
            );
            
            return optimizedRoute;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error optimizing route for trip {tripId}");
            throw new ApplicationException("Failed to optimize route", ex);
        }
    }

    public async Task<Dictionary<int, OptimizedRouteDto>> BatchOptimizeRoutesForTripsAsync(
        List<int> tripIds, 
        RouteOptimizationRequestDto? request = null)
    {
        var results = new Dictionary<int, OptimizedRouteDto>();
        
        // Process each trip in parallel
        var tasks = tripIds.Select(async tripId =>
        {
            try
            {
                var result = await GetOptimizedRouteForTripAsync(tripId, request);
                results[tripId] = result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error optimizing route for trip {tripId}");
            }
        });
        
        await Task.WhenAll(tasks);
        return results;
    }
    
    // Helper method to calculate distance between two points using Haversine formula
    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double earthRadiusMeters = 6371000; // Earth's radius in meters
        
        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);
        
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) * 
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
                
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return earthRadiusMeters * c;
    }
    
    private double ToRadians(double degrees)
    {
        return degrees * (Math.PI / 180);
    }
}

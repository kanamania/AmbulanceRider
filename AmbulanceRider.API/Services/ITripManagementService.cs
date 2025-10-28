using AmbulanceRider.API.DTOs;
using AmbulanceRider.API.Models;

namespace AmbulanceRider.API.Services;

public interface ITripManagementService
{
    // Bulk operations
    Task<BulkOperationResult> BulkUpdateTripsAsync(BulkUpdateTripsDto updateDto, Guid userId);
    
    // GPS verification
    Task<bool> VerifyTripCompletionLocationAsync(int tripId, GpsVerificationDto verification, Guid userId);
    
    // Media handling
    Task<string> UploadTripMediaAsync(int tripId, TripMediaUploadDto mediaUpload, Guid userId);
    
    // Auto-approval
    Task<bool> CheckAndAutoApproveTripAsync(int tripId, Guid userId);
    Task<AutoApproveCriteriaDto> GetAutoApproveCriteriaAsync();
    Task<bool> UpdateAutoApproveCriteriaAsync(AutoApproveCriteriaDto criteria, Guid userId);
    
    // Auto-start
    Task CheckAndStartScheduledTripsAsync();
    
    // Route optimization
    Task<OptimizedRouteDto> GetOptimizedRouteForTripAsync(int tripId, RouteOptimizationRequestDto? request = null);
    Task<Dictionary<int, OptimizedRouteDto>> BatchOptimizeRoutesForTripsAsync(List<int> tripIds, RouteOptimizationRequestDto? request = null);
}

public class BulkOperationResult
{
    public bool Success { get; set; }
    public int UpdatedCount { get; set; }
    public int FailedCount { get; set; }
    public List<string> Errors { get; set; } = new();
    public string? Message => $"Updated {UpdatedCount} trips. {FailedCount} failed.";
}

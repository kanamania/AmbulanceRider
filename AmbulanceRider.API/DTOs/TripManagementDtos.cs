using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace AmbulanceRider.API.DTOs;

public class BulkOperationResult
{
    public bool Success { get; set; }
    public int UpdatedCount { get; set; }
    public int FailedCount { get; set; }
    public List<string> Errors { get; set; } = new();
    public string Message => $"Updated {UpdatedCount} trips, {FailedCount} failed";
}

public class BulkUpdateTripsDto
{
    [Required]
    public List<int> TripIds { get; set; } = new();
    
    public string? Status { get; set; }
    public int? VehicleId { get; set; }
    public Guid? DriverId { get; set; }
    public string? Notes { get; set; }
}

public class GpsVerificationDto
{
    [Required]
    public double Latitude { get; set; }
    
    [Required]
    public double Longitude { get; set; }
    
    [Required]
    public double Accuracy { get; set; }
    
    public string? Notes { get; set; }
}

public class TripMediaUploadDto
{
    [Required]
    public IFormFile File { get; set; } = null!;
    
    [Required]
    public string MediaType { get; set; } = null!; // "photo" or "signature"
    
    public string? Notes { get; set; }
}

public class AutoApproveCriteriaDto
{
    public bool EnableForAllTrips { get; set; }
    public double? MaxDistanceMeters { get; set; } // Auto-approve if within this distance
    public TimeSpan? MaxDuration { get; set; } // Auto-approve if within this duration
    public List<int> AllowedTripTypeIds { get; set; } = new();
    public List<int> AllowedVehicleTypeIds { get; set; } = new();
    public bool RequireDriverConfirmation { get; set; }
}

public class RouteOptimizationRequestDto
{
    [Required]
    public List<RouteWaypointDto> Waypoints { get; set; } = new();
    
    public bool OptimizeForTime { get; set; } = true;
    public bool AvoidTolls { get; set; }
    public bool AvoidHighways { get; set; }
    public string? VehicleType { get; set; }
}

public class RouteWaypointDto
{
    [Required]
    public double Latitude { get; set; }
    
    [Required]
    public double Longitude { get; set; }
    
    public bool IsPickup { get; set; }
    public bool IsDropoff { get; set; }
    public string? LocationName { get; set; }
    public int? Sequence { get; set; }
}

public class OptimizedRouteDto
{
    public string? Polyline { get; set; }
    public double DistanceMeters { get; set; }
    public int DurationSeconds { get; set; }
    public List<RouteLegDto> Legs { get; set; } = new();
    public Dictionary<string, object>? Metadata { get; set; }
}

public class RouteLegDto
{
    public double StartLat { get; set; }
    public double StartLng { get; set; }
    public double EndLat { get; set; }
    public double EndLng { get; set; }
    public double DistanceMeters { get; set; }
    public int DurationSeconds { get; set; }
    public string? Instructions { get; set; }
    public string? Maneuver { get; set; }
}

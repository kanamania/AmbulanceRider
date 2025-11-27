using AmbulanceRider.API.DTOs;
using AmbulanceRider.API.Models;
using AmbulanceRider.API.Repositories;

namespace AmbulanceRider.API.Services;

public class TripService : ITripService
{
    private readonly ITripRepository _tripRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITripStatusLogRepository _tripStatusLogRepository;
    private readonly ITripAttributeValueRepository _tripAttributeValueRepository;
    private readonly INotificationService _notificationService;
    private readonly IRouteOptimizationService _routeOptimizationService;
    private readonly IPricingMatrixRepository _pricingRepo;

    public TripService(
        ITripRepository tripRepository,
        IVehicleRepository vehicleRepository,
        IUserRepository userRepository,
        ITripStatusLogRepository tripStatusLogRepository,
        ITripAttributeValueRepository tripAttributeValueRepository,
        INotificationService notificationService,
        IRouteOptimizationService routeOptimizationService,
        IPricingMatrixRepository pricingRepo)
    {
        _tripRepository = tripRepository;
        _vehicleRepository = vehicleRepository;
        _userRepository = userRepository;
        _tripStatusLogRepository = tripStatusLogRepository;
        _tripAttributeValueRepository = tripAttributeValueRepository;
        _notificationService = notificationService;
        _routeOptimizationService = routeOptimizationService;
        _pricingRepo = pricingRepo;
    }

    public async Task<IEnumerable<TripDto>> GetAllTripsAsync()
    {
        var trips = await _tripRepository.GetAllAsync();
        return trips.Select(MapToDto);
    }

    public async Task<IEnumerable<TripDto>> GetAllTripsAsync(string userId, bool isAdminOrDispatcher)
    {
        var trips = await _tripRepository.GetAllAsync();
        
        if (!isAdminOrDispatcher)
        {
            var userGuid = Guid.Parse(userId);
            trips = trips.Where(t => t.CreatedBy == userGuid || t.DriverId == userGuid).ToList();
        }
        
        return trips.Select(MapToDto);
    }

    public async Task<TripDto?> GetTripByIdAsync(int id)
    {
        var trip = await _tripRepository.GetByIdAsync(id);
        return trip == null ? null : await MapToDtoAsync(trip);
    }

    public async Task<IEnumerable<TripDto>> GetTripsByStatusAsync(TripStatus status)
    {
        var trips = await _tripRepository.GetTripsByStatusAsync(status);
        return trips.Select(MapToDto);
    }

    public async Task<IEnumerable<TripDto>> GetPendingTripsAsync()
    {
        var trips = await _tripRepository.GetPendingTripsAsync();
        return trips.Select(MapToDto);
    }

    public async Task<IEnumerable<TripDto>> GetTripsByDriverAsync(Guid driverId)
    {
        var trips = await _tripRepository.GetTripsByDriverAsync(driverId);
        return trips.Select(MapToDto);
    }

    public async Task<TripDto> CreateTripAsync(CreateTripDto createTripDto, Guid createdBy)
    {
        // Validate vehicle if provided
        if (createTripDto.VehicleId.HasValue)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(createTripDto.VehicleId.Value);
            if (vehicle == null)
            {
                throw new KeyNotFoundException("Vehicle not found");
            }
        }

        // Validate driver if provided
        if (createTripDto.DriverId.HasValue)
        {
            var driver = await _userRepository.GetByIdAsync(createTripDto.DriverId.Value);
            if (driver == null)
            {
                throw new KeyNotFoundException("Driver not found");
            }
        }

        var trip = new Trip
        {
            Name = createTripDto.Name,
            Description = createTripDto.Description,
            ScheduledStartTime = createTripDto.ScheduledStartTime ?? DateTime.UtcNow,
            FromLatitude = createTripDto.FromLatitude,
            FromLongitude = createTripDto.FromLongitude,
            ToLatitude = createTripDto.ToLatitude,
            ToLongitude = createTripDto.ToLongitude,
            FromLocationName = createTripDto.FromLocationName,
            ToLocationName = createTripDto.ToLocationName,
            VehicleId = createTripDto.VehicleId,
            DriverId = createTripDto.DriverId,
            TripTypeId = createTripDto.TripTypeId,
            Status = TripStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        // Calculate pricing
        var pricing = (await _pricingRepo.GetByDimensionsAsync(
            createTripDto.Weight, 
            createTripDto.Height,
            createTripDto.Length,
            createTripDto.Width))
            .FirstOrDefault();

        if (pricing != null)
        {
            trip.PricingMatrixId = pricing.Id;
            trip.BasePrice = pricing.BasePrice;
            trip.TaxAmount = pricing.BasePrice * pricing.TaxRate;
            trip.TotalPrice = pricing.TotalPrice;
        }

        try
        {
            var routeRequest = new RouteOptimizationRequestDto
            {
                Waypoints = new List<RouteWaypointDto>
                {
                    new RouteWaypointDto
                    {
                        Latitude = createTripDto.FromLatitude,
                        Longitude = createTripDto.FromLongitude,
                        Sequence = 0
                    },
                    new RouteWaypointDto
                    {
                        Latitude = createTripDto.ToLatitude,
                        Longitude = createTripDto.ToLongitude,
                        Sequence = 1
                    }
                }
            };

            var optimized = await _routeOptimizationService.GetOptimizedRouteAsync(routeRequest);
            trip.OptimizedRoute = optimized.Polyline;
            trip.RoutePolyline = optimized.Polyline;
            trip.EstimatedDistance = optimized.DistanceMeters;
            trip.EstimatedDuration = optimized.DurationSeconds;
        }
        catch
        {
        }

        await _tripRepository.AddAsync(trip);
        
        // Add attribute values if provided
        if (createTripDto.AttributeValues != null && createTripDto.AttributeValues.Any())
        {
            var attributeValues = createTripDto.AttributeValues.Select(av => new TripAttributeValue
            {
                TripId = trip.Id,
                TripTypeAttributeId = av.TripTypeAttributeId,
                Value = av.Value,
                CreatedAt = DateTime.UtcNow
            });
            
            await _tripAttributeValueRepository.AddRangeAsync(attributeValues);
        }
        
        // Reload to get navigation properties
        var createdTrip = await _tripRepository.GetByIdAsync(trip.Id);
        return await MapToDtoAsync(createdTrip!);
    }

    public async Task<TripDto> UpdateTripAsync(int id, UpdateTripDto updateTripDto)
    {
        var trip = await _tripRepository.GetByIdAsync(id);
        if (trip == null)
        {
            throw new KeyNotFoundException("Trip not found");
        }

        // Only allow updates for pending trips
        if (trip.Status != TripStatus.Pending)
        {
            throw new InvalidOperationException("Only pending trips can be updated");
        }

        if (!string.IsNullOrEmpty(updateTripDto.Name))
            trip.Name = updateTripDto.Name;

        if (updateTripDto.Description != null)
            trip.Description = updateTripDto.Description;

        if (updateTripDto.ScheduledStartTimeSpecified)
        {
            trip.ScheduledStartTime = updateTripDto.ScheduledStartTime ?? DateTime.UtcNow;
        }

        // Update coordinates if provided
        var coordsChanged = false;
        if (updateTripDto.FromLatitude.HasValue && updateTripDto.FromLatitude.Value != trip.FromLatitude)
        {
            trip.FromLatitude = updateTripDto.FromLatitude.Value;
            coordsChanged = true;
        }
        
        if (updateTripDto.FromLongitude.HasValue && updateTripDto.FromLongitude.Value != trip.FromLongitude)
        {
            trip.FromLongitude = updateTripDto.FromLongitude.Value;
            coordsChanged = true;
        }
        
        if (updateTripDto.ToLatitude.HasValue && updateTripDto.ToLatitude.Value != trip.ToLatitude)
        {
            trip.ToLatitude = updateTripDto.ToLatitude.Value;
            coordsChanged = true;
        }
        
        if (updateTripDto.ToLongitude.HasValue && updateTripDto.ToLongitude.Value != trip.ToLongitude)
        {
            trip.ToLongitude = updateTripDto.ToLongitude.Value;
            coordsChanged = true;
        }
        
        if (updateTripDto.FromLocationName != null)
            trip.FromLocationName = updateTripDto.FromLocationName;
        
        if (updateTripDto.ToLocationName != null)
            trip.ToLocationName = updateTripDto.ToLocationName;

        if (updateTripDto.VehicleId.HasValue)
        {
            if (updateTripDto.VehicleId.Value != 0)
            {
                var vehicle = await _vehicleRepository.GetByIdAsync(updateTripDto.VehicleId.Value);
                if (vehicle == null)
                {
                    throw new KeyNotFoundException("Vehicle not found");
                }
            }
            trip.VehicleId = updateTripDto.VehicleId.Value;
        }

        if (updateTripDto.DriverId.HasValue)
        {
            if (updateTripDto.DriverId.Value != Guid.Empty)
            {
                var driver = await _userRepository.GetByIdAsync(updateTripDto.DriverId.Value);
                if (driver == null)
                {
                    throw new KeyNotFoundException("Driver not found");
                }
            }
            trip.DriverId = updateTripDto.DriverId.Value;
        }

        if (updateTripDto.TripTypeId.HasValue)
        {
            trip.TripTypeId = updateTripDto.TripTypeId.Value;
        }

        if (updateTripDto.AttributeValues != null)
        {
            await _tripAttributeValueRepository.DeleteByTripIdAsync(trip.Id);
            var newValues = updateTripDto.AttributeValues.Select(av => new TripAttributeValue
            {
                TripId = trip.Id,
                TripTypeAttributeId = av.TripTypeAttributeId,
                Value = av.Value,
                CreatedAt = DateTime.UtcNow
            });
            await _tripAttributeValueRepository.AddRangeAsync(newValues);
        }

        // If coordinates changed, recompute route estimates
        if (coordsChanged)
        {
            try
            {
                var routeRequest = new RouteOptimizationRequestDto
                {
                    Waypoints = new List<RouteWaypointDto>
                    {
                        new RouteWaypointDto
                        {
                            Latitude = trip.FromLatitude,
                            Longitude = trip.FromLongitude,
                            Sequence = 0
                        },
                        new RouteWaypointDto
                        {
                            Latitude = trip.ToLatitude,
                            Longitude = trip.ToLongitude,
                            Sequence = 1
                        }
                    }
                };

                var optimized = await _routeOptimizationService.GetOptimizedRouteAsync(routeRequest);
                trip.OptimizedRoute = optimized.Polyline;
                trip.RoutePolyline = optimized.Polyline;
                trip.EstimatedDistance = optimized.DistanceMeters;
                trip.EstimatedDuration = optimized.DurationSeconds;
            }
            catch
            {
            }
        }

        trip.UpdatedAt = DateTime.UtcNow;
        await _tripRepository.UpdateAsync(trip);
        
        // Reload to get navigation properties
        var updatedTrip = await _tripRepository.GetByIdAsync(trip.Id);
        return await MapToDtoAsync(updatedTrip!);
    }

    public async Task<TripDto> ApproveTripAsync(int id, ApproveTripDto approveTripDto, Guid approverId)
    {
        var trip = await _tripRepository.GetByIdAsync(id);
        if (trip == null)
        {
            throw new KeyNotFoundException("Trip not found");
        }

        if (trip.Status != TripStatus.Pending)
        {
            throw new InvalidOperationException("Only pending trips can be approved or rejected");
        }

        if (approveTripDto.Approve)
        {
            if (!approveTripDto.VehicleId.HasValue)
            {
                throw new InvalidOperationException("Vehicle ID is required when approving a trip");
            }
            
            // Verify the vehicle exists
            var vehicle = await _vehicleRepository.GetByIdAsync(approveTripDto.VehicleId.Value);
            if (vehicle == null)
            {
                throw new KeyNotFoundException("Specified vehicle not found");
            }
            
            trip.Status = TripStatus.Approved;
            trip.VehicleId = approveTripDto.VehicleId.Value;
            trip.ApprovedBy = approverId;
            trip.ApprovedAt = DateTime.UtcNow;
            trip.RejectionReason = null;
        }
        else
        {
            if (string.IsNullOrWhiteSpace(approveTripDto.RejectionReason))
            {
                throw new InvalidOperationException("Rejection reason is required when rejecting a trip");
            }
            trip.Status = TripStatus.Rejected;
            trip.RejectionReason = approveTripDto.RejectionReason;
            trip.ApprovedBy = approverId;
            trip.ApprovedAt = DateTime.UtcNow;
        }

        trip.UpdatedAt = DateTime.UtcNow;
        await _tripRepository.UpdateAsync(trip);
        
        // Reload to get navigation properties
        var updatedTrip = await _tripRepository.GetByIdAsync(trip.Id);
        return MapToDto(updatedTrip!);
    }

    public async Task<TripDto> StartTripAsync(int id, StartTripDto startTripDto)
    {
        var trip = await _tripRepository.GetByIdAsync(id);
        if (trip == null)
        {
            throw new KeyNotFoundException("Trip not found");
        }

        if (trip.Status != TripStatus.Approved)
        {
            throw new InvalidOperationException("Only approved trips can be started");
        }

        trip.Status = TripStatus.InProgress;
        trip.ActualStartTime = startTripDto.ActualStartTime;
        trip.UpdatedAt = DateTime.UtcNow;

        await _tripRepository.UpdateAsync(trip);
        
        // Reload to get navigation properties
        var updatedTrip = await _tripRepository.GetByIdAsync(trip.Id);
        return MapToDto(updatedTrip!);
    }

    public async Task<TripDto> CompleteTripAsync(int id, CompleteTripDto completeTripDto)
    {
        var trip = await _tripRepository.GetByIdAsync(id);
        if (trip == null)
        {
            throw new KeyNotFoundException("Trip not found");
        }

        if (trip.Status != TripStatus.InProgress)
        {
            throw new InvalidOperationException("Only in-progress trips can be completed");
        }

        trip.Status = TripStatus.Completed;
        trip.ActualEndTime = completeTripDto.ActualEndTime;
        trip.UpdatedAt = DateTime.UtcNow;

        await _tripRepository.UpdateAsync(trip);
        
        // Reload to get navigation properties
        var updatedTrip = await _tripRepository.GetByIdAsync(trip.Id);
        return MapToDto(updatedTrip!);
    }

    public async Task<TripDto> CancelTripAsync(int id, string? reason = null)
    {
        var trip = await _tripRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("Trip not found");

        if (trip.Status == TripStatus.Cancelled)
        {
            return MapToDto(trip);
        }

        if (trip.Status == TripStatus.Completed)
        {
            throw new InvalidOperationException("Cannot cancel a completed trip");
        }

        trip.Status = TripStatus.Cancelled;
        trip.UpdatedAt = DateTime.UtcNow;
        trip.RejectionReason = reason;

        await _tripRepository.UpdateAsync(trip);
        
        // Reload to get navigation properties
        var updatedTrip = await _tripRepository.GetByIdAsync(trip.Id);
        return MapToDto(updatedTrip!);
    }

    public async Task<TripDto> UpdateTripStatusAsync(int tripId, UpdateTripStatusDto updateDto, Guid userId, bool isAdminOrDispatcher)
    {
        var trip = await _tripRepository.GetByIdAsync(tripId) ?? throw new KeyNotFoundException("Trip not found");
        var oldStatus = trip.Status;
        
        // Check if user is the creator of the trip
        var isCreator = userId == trip.CreatedBy;
        
        // Validate status transition
        if (!IsValidStatusTransition(trip.Status, updateDto.Status, isAdminOrDispatcher, isCreator, updateDto.ForceComplete))
        {
            throw new InvalidOperationException("Invalid status transition");
        }

        // Apply status-specific logic
        switch (updateDto.Status)
        {
            case TripStatus.Approved:
                if (!isAdminOrDispatcher)
                {
                    throw new UnauthorizedAccessException("Only admins and dispatchers can approve trips");
                }
                trip.ApprovedBy = userId;
                trip.ApprovedAt = DateTime.UtcNow;
                break;

            case TripStatus.Rejected:
                if (!isAdminOrDispatcher)
                {
                    throw new UnauthorizedAccessException("Only admins and dispatchers can reject trips");
                }
                trip.RejectionReason = updateDto.RejectionReason ?? "No reason provided";
                break;

            case TripStatus.InProgress:
                trip.ActualStartTime = DateTime.UtcNow;
                break;

            case TripStatus.Completed:
                trip.ActualEndTime = DateTime.UtcNow;
                break;

            case TripStatus.Cancelled:
                if (!isAdminOrDispatcher && userId != trip.CreatedBy)
                {
                    throw new UnauthorizedAccessException("Only admins, dispatchers, or the trip creator can cancel a trip");
                }
                trip.RejectionReason = updateDto.Notes;
                break;
        }

        // Update the status and save
        trip.Status = updateDto.Status;
        trip.UpdatedAt = DateTime.UtcNow;
        
        // Add a note to the description if provided
        if (!string.IsNullOrWhiteSpace(updateDto.Notes))
        {
            trip.Description = string.IsNullOrEmpty(trip.Description) 
                ? updateDto.Notes 
                : $"{trip.Description}\n\nStatus Update: {updateDto.Notes}";
        }
        
        trip.UpdatedAt = DateTime.UtcNow;
        await _tripRepository.UpdateAsync(trip);
        
        // Log status change
        await LogStatusChangeAsync(trip, oldStatus, trip.Status, userId, isAdminOrDispatcher, updateDto);
        
        // Send notification about status change
        await _notificationService.SendTripStatusChangeAsync(trip.Id, oldStatus.ToString(), trip.Status.ToString());
        
        // Reload to get navigation properties
        var updatedTrip = await _tripRepository.GetByIdAsync(trip.Id);
        return MapToDto(updatedTrip!);
    }

    private async Task LogStatusChangeAsync(Trip trip, TripStatus fromStatus, TripStatus toStatus, Guid userId, bool isAdminOrDispatcher, UpdateTripStatusDto updateDto)
    {
        // Get user information
        var user = await _userRepository.GetByIdAsync(userId);
        var userRole = isAdminOrDispatcher ? "Admin/Dispatcher" : "Driver";
        
        var statusLog = new TripStatusLog
        {
            TripId = trip.Id,
            FromStatus = fromStatus,
            ToStatus = toStatus,
            ChangedBy = userId,
            ChangedAt = DateTime.UtcNow,
            Notes = updateDto.Notes,
            RejectionReason = updateDto.RejectionReason,
            IsForceComplete = updateDto.ForceComplete,
            UserRole = userRole,
            UserName = user != null ? $"{user.FirstName} {user.LastName}" : "Unknown User",
            CreatedAt = DateTime.UtcNow
        };

        await _tripStatusLogRepository.AddAsync(statusLog);
    }

    private bool IsValidStatusTransition(TripStatus currentStatus, TripStatus newStatus, bool isAdminOrDispatcher, bool isCreator, bool forceComplete = false)
    {
        // If forcing completion, only check if the target status is Completed
        if (forceComplete)
        {
            return newStatus == TripStatus.Completed;
        }

        // Define valid transitions
        var validTransitions = new Dictionary<TripStatus, List<(TripStatus, bool, bool)>>
        {
            [TripStatus.Pending] = new()
            {
                (TripStatus.Approved, true, false),    // Admin/Dispatcher can approve
                (TripStatus.Rejected, true, false),    // Admin/Dispatcher can reject
                (TripStatus.Cancelled, true, true),    // Creator or Admin can cancel a pending trip
            },
            [TripStatus.Approved] = new()
            {
                (TripStatus.InProgress, true, true),   // Creator or Admin can start the trip
                (TripStatus.Cancelled, true, true),    // Creator or Admin can cancel an approved trip
            },
            [TripStatus.InProgress] = new()
            {
                (TripStatus.Completed, true, true),    // Creator or Admin can complete the trip
                (TripStatus.Cancelled, true, true),    // Creator or Admin can cancel an in-progress trip
            },
            [TripStatus.Rejected] = new()
            {
                (TripStatus.Pending, true, false),     // Admin/Dispatcher can move back to pending
            },
            [TripStatus.Completed] = new()
            {
                // No valid transitions from completed
            },
            [TripStatus.Cancelled] = new()
            {
                (TripStatus.Pending, true, false),     // Admin/Dispatcher can move back to pending
            }
        };

        // Check if the transition is valid
        if (validTransitions.TryGetValue(currentStatus, out var allowedTransitions))
        {
            return allowedTransitions.Any(t => 
                t.Item1 == newStatus && 
                (t.Item2 || !isAdminOrDispatcher) &&   // If transition requires admin/dispatcher, check if user is one
                (t.Item3 || isCreator)                 // If transition requires creator, check if user is the creator
            );
        }

        return false;
    }

    public async Task DeleteTripAsync(int id)
    {
        var trip = await _tripRepository.GetByIdAsync(id);
        if (trip == null)
        {
            throw new KeyNotFoundException("Trip not found");
        }

        trip.DeletedAt = DateTime.UtcNow;
        await _tripRepository.UpdateAsync(trip);
    }

    public async Task<IEnumerable<TripStatusLogDto>> GetTripStatusLogsAsync(int tripId)
    {
        var logs = await _tripStatusLogRepository.GetLogsByTripIdAsync(tripId);
        return logs.Select(MapStatusLogToDto);
    }

    private static TripStatusLogDto MapStatusLogToDto(TripStatusLog log)
    {
        return new TripStatusLogDto
        {
            Id = log.Id,
            TripId = log.TripId,
            FromStatus = log.FromStatus.ToString(),
            ToStatus = log.ToStatus.ToString(),
            ChangedBy = log.ChangedBy,
            ChangedAt = log.ChangedAt,
            Notes = log.Notes,
            RejectionReason = log.RejectionReason,
            IsForceComplete = log.IsForceComplete,
            UserRole = log.UserRole,
            UserName = log.UserName,
            User = log.User != null ? new UserDto
            {
                Id = log.User.Id.ToString(),
                FirstName = log.User.FirstName,
                LastName = log.User.LastName,
                Email = log.User.Email ?? string.Empty,
                PhoneNumber = log.User.PhoneNumber,
                Roles = new List<string>()
            } : null
        };
    }

    private static TripDto MapToDto(Trip trip)
    {
        return new TripDto
        {
            Id = trip.Id,
            Name = trip.Name,
            Description = trip.Description,
            ScheduledStartTime = trip.ScheduledStartTime,
            ActualStartTime = trip.ActualStartTime,
            ActualEndTime = trip.ActualEndTime,
            Status = trip.Status.ToString(),
            RejectionReason = trip.RejectionReason,
            FromLatitude = trip.FromLatitude,
            FromLongitude = trip.FromLongitude,
            ToLatitude = trip.ToLatitude,
            ToLongitude = trip.ToLongitude,
            FromLocationName = trip.FromLocationName,
            ToLocationName = trip.ToLocationName,
            VehicleId = trip.VehicleId,
            Vehicle = trip.Vehicle != null ? new VehicleDto
            {
                Id = trip.Vehicle.Id,
                Name = trip.Vehicle.Name,
                ImagePath = trip.Vehicle.Image,
                ImageUrl = trip.Vehicle.Image,
                CreatedAt = trip.Vehicle.CreatedAt
            } : null,
            DriverId = trip.DriverId,
            Driver = trip.Driver != null ? new UserDto
            {
                Id = trip.Driver.Id.ToString(),
                Name = $"{trip.Driver.FirstName} {trip.Driver.LastName}",
                FirstName = trip.Driver.FirstName,
                LastName = trip.Driver.LastName,
                Email = trip.Driver.Email!,
                PhoneNumber = trip.Driver.PhoneNumber,
                ImagePath = trip.Driver.ImagePath,
                ImageUrl = trip.Driver.ImageUrl,
                Roles = new List<string>(),
                CreatedAt = trip.Driver.CreatedAt,
                UpdatedAt = trip.Driver.UpdatedAt
            } : null,
            ApprovedBy = trip.ApprovedBy,
            Approver = trip.Approver != null ? new UserDto
            {
                Id = trip.Approver.Id.ToString(),
                Name = $"{trip.Approver.FirstName} {trip.Approver.LastName}",
                FirstName = trip.Approver.FirstName,
                LastName = trip.Approver.LastName,
                Email = trip.Approver.Email!,
                PhoneNumber = trip.Approver.PhoneNumber,
                ImagePath = trip.Approver.ImagePath,
                ImageUrl = trip.Approver.ImageUrl,
                Roles = new List<string>(),
                CreatedAt = trip.Approver.CreatedAt,
                UpdatedAt = trip.Approver.UpdatedAt
            } : null,
            ApprovedAt = trip.ApprovedAt,
            CreatedAt = trip.CreatedAt,
            TripTypeId = trip.TripTypeId,
            TripType = trip.TripType != null ? new TripTypeDto
            {
                Id = trip.TripType.Id,
                Name = trip.TripType.Name,
                Description = trip.TripType.Description,
                Color = trip.TripType.Color,
                Icon = trip.TripType.Icon,
                IsActive = trip.TripType.IsActive,
                DisplayOrder = trip.TripType.DisplayOrder,
                CreatedAt = trip.TripType.CreatedAt,
                Attributes = new List<TripTypeAttributeDto>()
            } : null,
            OptimizedRoute = trip.OptimizedRoute,
            RoutePolyline = trip.RoutePolyline,
            EstimatedDistance = trip.EstimatedDistance,
            EstimatedDuration = trip.EstimatedDuration,
            PricingMatrixId = trip.PricingMatrixId,
            BasePrice = trip.BasePrice,
            TaxAmount = trip.TaxAmount,
            TotalPrice = trip.TotalPrice
        };
    }
    
    private async Task<TripDto> MapToDtoAsync(Trip trip)
    {
        var dto = MapToDto(trip);
        
        // Load attribute values
        if (trip.TripTypeId.HasValue)
        {
            var attributeValues = await _tripAttributeValueRepository.GetByTripIdAsync(trip.Id);
            dto.AttributeValues = attributeValues.Select(av => new TripAttributeValueDto
            {
                Id = av.Id,
                TripId = av.TripId,
                TripTypeAttributeId = av.TripTypeAttributeId,
                Value = av.Value,
                AttributeName = av.TripTypeAttribute?.Name,
                AttributeLabel = av.TripTypeAttribute?.Label,
                AttributeDataType = av.TripTypeAttribute?.DataType
            }).ToList();
        }
        
        return dto;
    }
}

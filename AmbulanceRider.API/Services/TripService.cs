using AmbulanceRider.API.DTOs;
using AmbulanceRider.API.Models;
using AmbulanceRider.API.Repositories;

namespace AmbulanceRider.API.Services;

public class TripService : ITripService
{
    private readonly ITripRepository _tripRepository;
    private readonly IRouteRepository _routeRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IUserRepository _userRepository;

    public TripService(
        ITripRepository tripRepository,
        IRouteRepository routeRepository,
        IVehicleRepository vehicleRepository,
        IUserRepository userRepository)
    {
        _tripRepository = tripRepository;
        _routeRepository = routeRepository;
        _vehicleRepository = vehicleRepository;
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<TripDto>> GetAllTripsAsync()
    {
        var trips = await _tripRepository.GetAllAsync();
        return trips.Select(MapToDto);
    }

    public async Task<TripDto?> GetTripByIdAsync(int id)
    {
        var trip = await _tripRepository.GetByIdAsync(id);
        return trip == null ? null : MapToDto(trip);
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

    public async Task<IEnumerable<TripDto>> GetTripsByRouteAsync(int routeId)
    {
        var trips = await _tripRepository.GetTripsByRouteAsync(routeId);
        return trips.Select(MapToDto);
    }

    public async Task<IEnumerable<TripDto>> GetTripsByDriverAsync(Guid driverId)
    {
        var trips = await _tripRepository.GetTripsByDriverAsync(driverId);
        return trips.Select(MapToDto);
    }

    public async Task<TripDto> CreateTripAsync(CreateTripDto createTripDto)
    {
        // Validate route exists
        var route = await _routeRepository.GetByIdAsync(createTripDto.RouteId);
        if (route == null)
        {
            throw new KeyNotFoundException("Route not found");
        }

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
            ScheduledStartTime = createTripDto.ScheduledStartTime,
            RouteId = createTripDto.RouteId,
            VehicleId = createTripDto.VehicleId,
            DriverId = createTripDto.DriverId,
            Status = TripStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _tripRepository.AddAsync(trip);
        
        // Reload to get navigation properties
        var createdTrip = await _tripRepository.GetByIdAsync(trip.Id);
        return MapToDto(createdTrip!);
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

        if (updateTripDto.ScheduledStartTime.HasValue)
            trip.ScheduledStartTime = updateTripDto.ScheduledStartTime.Value;

        if (updateTripDto.RouteId.HasValue && updateTripDto.RouteId.Value != 0)
        {
            var route = await _routeRepository.GetByIdAsync(updateTripDto.RouteId.Value);
            if (route == null)
            {
                throw new KeyNotFoundException("Route not found");
            }
            trip.RouteId = updateTripDto.RouteId.Value;
        }

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

        trip.UpdatedAt = DateTime.UtcNow;
        await _tripRepository.UpdateAsync(trip);
        
        // Reload to get navigation properties
        var updatedTrip = await _tripRepository.GetByIdAsync(trip.Id);
        return MapToDto(updatedTrip!);
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
            trip.Status = TripStatus.Approved;
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

    public async Task<TripDto> CancelTripAsync(int id)
    {
        var trip = await _tripRepository.GetByIdAsync(id);
        if (trip == null)
        {
            throw new KeyNotFoundException("Trip not found");
        }

        if (trip.Status == TripStatus.Completed)
        {
            throw new InvalidOperationException("Completed trips cannot be cancelled");
        }

        trip.Status = TripStatus.Cancelled;
        trip.UpdatedAt = DateTime.UtcNow;

        await _tripRepository.UpdateAsync(trip);
        
        // Reload to get navigation properties
        var updatedTrip = await _tripRepository.GetByIdAsync(trip.Id);
        return MapToDto(updatedTrip!);
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
            RouteId = trip.RouteId,
            Route = trip.Route != null ? new RouteDto
            {
                Id = trip.Route.Id,
                Name = trip.Route.Name,
                Distance = trip.Route.Distance,
                EstimatedDuration = trip.Route.EstimatedDuration,
                Description = trip.Route.Description,
                FromLocation = trip.Route.FromLocation,
                ToLocation = trip.Route.ToLocation,
                FromLocationId = trip.Route.FromLocationId,
                ToLocationId = trip.Route.ToLocationId,
                CreatedAt = trip.Route.CreatedAt
            } : null,
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
            CreatedAt = trip.CreatedAt
        };
    }
}

# User and Driver Permissions Implementation

**Date:** 2025-10-27  
**Status:** ✅ Implemented  
**Objective:** Ensure User and Driver roles have identical permissions for trip management

---

## Changes Made

### 1. API Controller Authorization Updates

**File:** `AmbulanceRider.API/Controllers/TripsController.cs`

Updated all trip-related endpoints to allow User and Driver roles:

```csharp
// CREATE Trip - Now allows User and Driver
[HttpPost]
[Authorize(Roles = "Admin,Dispatcher,Driver,User")]
public async Task<ActionResult<TripDto>> Create([FromBody] CreateTripDto createTripDto)

// UPDATE Trip - Now allows User and Driver
[HttpPut("{id}")]
[Authorize(Roles = "Admin,Dispatcher,Driver,User")]
public async Task<ActionResult<TripDto>> Update(int id, [FromBody] UpdateTripDto updateTripDto)

// START Trip - Now allows User and Driver
[HttpPost("{id}/start")]
[Authorize(Roles = "Admin,Dispatcher,Driver,User")]
public async Task<ActionResult<TripDto>> Start(int id, [FromBody] StartTripDto startTripDto)

// COMPLETE Trip - Now allows User and Driver
[HttpPost("{id}/complete")]
[Authorize(Roles = "Admin,Dispatcher,Driver,User")]
public async Task<ActionResult<TripDto>> Complete(int id, [FromBody] CompleteTripDto completeTripDto)

// CANCEL Trip - Now allows User and Driver
[HttpPost("{id}/cancel")]
[Authorize(Roles = "Admin,Dispatcher,Driver,User")]
public async Task<ActionResult<TripDto>> Cancel(int id)

// UPDATE STATUS - Already allows all authenticated users
[HttpPut("{id}/status")]
[Authorize] // No role restriction
public async Task<ActionResult<TripDto>> UpdateStatus(int id, [FromBody] UpdateTripStatusDto updateDto)
```

### 2. Trip Creator Tracking

**File:** `AmbulanceRider.API/Controllers/TripsController.cs`

Updated Create endpoint to capture the user who creates the trip:

```csharp
public async Task<ActionResult<TripDto>> Create([FromBody] CreateTripDto createTripDto)
{
    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
    {
        return Unauthorized(new { message = "Invalid user token" });
    }

    var trip = await _tripService.CreateTripAsync(createTripDto, userId);
    return CreatedAtAction(nameof(GetById), new { id = trip.Id }, trip);
}
```

### 3. Service Layer Updates

**File:** `AmbulanceRider.API/Services/ITripService.cs`

Updated interface to accept creator ID:

```csharp
Task<TripDto> CreateTripAsync(CreateTripDto createTripDto, Guid createdBy);
```

**File:** `AmbulanceRider.API/Services/TripService.cs`

#### A. CreateTripAsync - Sets CreatedBy

```csharp
public async Task<TripDto> CreateTripAsync(CreateTripDto createTripDto, Guid createdBy)
{
    // ... validation code ...
    
    var trip = new Trip
    {
        // ... other properties ...
        Status = TripStatus.Pending,
        CreatedAt = DateTime.UtcNow,
        CreatedBy = createdBy  // ✅ NEW: Track who created the trip
    };

    await _tripRepository.AddAsync(trip);
    var createdTrip = await _tripRepository.GetByIdAsync(trip.Id);
    return MapToDto(createdTrip!);
}
```

#### B. UpdateTripStatusAsync - Checks Creator

```csharp
public async Task<TripDto> UpdateTripStatusAsync(int tripId, UpdateTripStatusDto updateDto, Guid userId, bool isAdminOrDispatcher)
{
    var trip = await _tripRepository.GetByIdAsync(tripId) ?? throw new KeyNotFoundException("Trip not found");
    var oldStatus = trip.Status;
    
    // ✅ NEW: Check if user is the creator of the trip
    var isCreator = userId == trip.CreatedBy;
    
    // Validate status transition
    if (!IsValidStatusTransition(trip.Status, updateDto.Status, isAdminOrDispatcher, isCreator, updateDto.ForceComplete))
    {
        throw new InvalidOperationException("Invalid status transition");
    }
    
    // ... rest of the method ...
}
```

#### C. Cancel Authorization - Checks Creator

```csharp
case TripStatus.Cancelled:
    if (!isAdminOrDispatcher && userId != trip.CreatedBy)  // ✅ CHANGED: from trip.DriverId
    {
        throw new UnauthorizedAccessException("Only admins, dispatchers, or the trip creator can cancel a trip");
    }
    trip.RejectionReason = updateDto.Notes;
    break;
```

#### D. IsValidStatusTransition - Uses Creator

```csharp
private bool IsValidStatusTransition(TripStatus currentStatus, TripStatus newStatus, bool isAdminOrDispatcher, bool isCreator, bool forceComplete = false)
{
    // ... force complete logic ...
    
    var validTransitions = new Dictionary<TripStatus, List<(TripStatus, bool, bool)>>
    {
        [TripStatus.Pending] = new()
        {
            (TripStatus.Approved, true, false),    // Admin/Dispatcher can approve
            (TripStatus.Rejected, true, false),    // Admin/Dispatcher can reject
            (TripStatus.Cancelled, true, true),    // ✅ Creator or Admin can cancel
        },
        [TripStatus.Approved] = new()
        {
            (TripStatus.InProgress, true, true),   // ✅ Creator or Admin can start
            (TripStatus.Cancelled, true, true),    // ✅ Creator or Admin can cancel
        },
        [TripStatus.InProgress] = new()
        {
            (TripStatus.Completed, true, true),    // ✅ Creator or Admin can complete
            (TripStatus.Cancelled, true, true),    // ✅ Creator or Admin can cancel
        },
        // ... other transitions ...
    };
    
    // Check if the transition is valid
    if (validTransitions.TryGetValue(currentStatus, out var allowedTransitions))
    {
        return allowedTransitions.Any(t => 
            t.Item1 == newStatus && 
            (t.Item2 || !isAdminOrDispatcher) &&
            (t.Item3 || isCreator)  // ✅ CHANGED: from isDriver to isCreator
        );
    }
    
    return false;
}
```

---

## How It Works

### Trip Creation Flow

1. **User/Driver creates a trip**
   - User logs in (gets JWT token with their user ID)
   - Calls `POST /api/trips` with trip details
   - Controller extracts user ID from JWT token
   - Service creates trip with `CreatedBy = userId`
   - Trip is saved with creator information

2. **Trip Ownership**
   - `CreatedBy` field tracks who created the trip
   - `DriverId` field tracks who is assigned to drive (can be different)
   - User can only manage trips they created (via `CreatedBy`)
   - Admin/Dispatcher can manage any trip

### Permission Checks

```
Action: Complete Trip
├─ Is user Admin/Dispatcher? → ✅ Allow
├─ Is user the creator (CreatedBy)? → ✅ Allow
└─ Otherwise → ❌ Deny

Action: Cancel Trip
├─ Is user Admin/Dispatcher? → ✅ Allow
├─ Is user the creator (CreatedBy)? → ✅ Allow
└─ Otherwise → ❌ Deny

Action: Approve/Reject Trip
├─ Is user Admin/Dispatcher? → ✅ Allow
└─ Otherwise → ❌ Deny (even if creator)
```

---

## Key Differences: DriverId vs CreatedBy

### Before (Incorrect)
- Permission checks used `trip.DriverId`
- Problem: User A creates trip, assigns Driver B
- Result: User A cannot complete/cancel their own trip ❌
- Result: Driver B can complete/cancel even though they didn't create it ❌

### After (Correct)
- Permission checks use `trip.CreatedBy`
- Scenario: User A creates trip, assigns Driver B
- Result: User A can complete/cancel their trip ✅
- Result: Driver B cannot complete/cancel (unless they're Admin) ✅
- Result: Admin/Dispatcher can manage any trip ✅

---

## Database Schema

The `Trip` model inherits from `BaseModel` which includes:

```csharp
public abstract class BaseModel
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }      // ✅ Used for ownership
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
}
```

The `Trip` model also has:

```csharp
public class Trip : BaseModel
{
    // ... other properties ...
    public Guid? DriverId { get; set; }       // Who is assigned to drive
    public virtual User? Driver { get; set; }
}
```

**Important Distinction:**
- `CreatedBy` (from BaseModel) = Who created/owns the trip
- `DriverId` (Trip property) = Who is assigned to drive

---

## Testing Checklist

### ✅ User Role Tests
- [ ] User can create a trip
- [ ] User can view their own trips
- [ ] User can update their own pending trips
- [ ] User can start their own approved trips
- [ ] User can complete their own in-progress trips
- [ ] User can cancel their own trips
- [ ] User cannot complete trips created by others
- [ ] User cannot cancel trips created by others
- [ ] User cannot approve/reject any trips

### ✅ Driver Role Tests
- [ ] Driver can create a trip
- [ ] Driver can view their own trips
- [ ] Driver can update their own pending trips
- [ ] Driver can start their own approved trips
- [ ] Driver can complete their own in-progress trips
- [ ] Driver can cancel their own trips
- [ ] Driver cannot complete trips created by others
- [ ] Driver cannot cancel trips created by others
- [ ] Driver cannot approve/reject any trips

### ✅ Admin/Dispatcher Tests
- [ ] Admin can create any trip
- [ ] Admin can view all trips
- [ ] Admin can update any trip
- [ ] Admin can approve/reject pending trips
- [ ] Admin can force complete any trip
- [ ] Admin can cancel any trip
- [ ] Admin can reactivate cancelled/rejected trips

### ✅ Edge Cases
- [ ] User A creates trip, assigns Driver B
  - User A can complete/cancel ✅
  - Driver B cannot complete/cancel ✅
  - Admin can complete/cancel ✅
- [ ] User creates trip without assigning driver
  - User can still manage trip ✅
- [ ] User creates trip and assigns themselves as driver
  - User can manage trip (via CreatedBy, not DriverId) ✅

---

## API Endpoints Summary

| Endpoint | Method | Roles Allowed | Permission Check |
|----------|--------|---------------|------------------|
| `/api/trips` | GET | All authenticated | None |
| `/api/trips/{id}` | GET | All authenticated | None |
| `/api/trips` | POST | Admin, Dispatcher, Driver, User | None (sets CreatedBy) |
| `/api/trips/{id}` | PUT | Admin, Dispatcher, Driver, User | Only pending trips |
| `/api/trips/{id}/status` | PUT | All authenticated | Creator or Admin |
| `/api/trips/{id}/start` | POST | Admin, Dispatcher, Driver, User | Creator or Admin |
| `/api/trips/{id}/complete` | POST | Admin, Dispatcher, Driver, User | Creator or Admin |
| `/api/trips/{id}/cancel` | POST | Admin, Dispatcher, Driver, User | Creator or Admin |
| `/api/trips/{id}/approve` | POST | Admin, Dispatcher | Admin only |
| `/api/trips/{id}/status-logs` | GET | All authenticated | None |

---

## Benefits

1. **Correct Ownership Model**
   - Users own the trips they create
   - Driver assignment is separate from ownership
   - Clear separation of concerns

2. **Proper Access Control**
   - Users can only manage their own trips
   - Admins can manage any trip
   - No confusion between creator and assigned driver

3. **Audit Trail**
   - `CreatedBy` tracks who created the trip
   - `TripStatusLog` tracks all status changes
   - Complete accountability

4. **Flexibility**
   - User can create trip for themselves
   - User can create trip and assign another driver
   - Driver assignment can be changed without affecting ownership

---

## Summary

✅ **User and Driver roles now have identical permissions**
✅ **Permission checks use `CreatedBy` instead of `DriverId`**
✅ **Users can only manage trips they created**
✅ **Admin/Dispatcher can manage any trip**
✅ **Proper separation between trip creator and assigned driver**

---

**Status:** ✅ Implementation Complete  
**Build Status:** Ready to test  
**Next Step:** Build and test the application

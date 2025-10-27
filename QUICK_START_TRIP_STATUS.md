# Quick Start: Trip Status Management

## Overview
This guide shows you how to use the new trip status management features.

## For Drivers

### How to Complete a Trip
1. Navigate to **Edit Trip** page for your assigned trip
2. Ensure trip status is **InProgress**
3. Click **"Complete Trip"** button in the Trip Actions card
4. (Optional) Add notes about the trip completion
5. Click **"Complete Trip"** in the modal
6. ✅ Trip status updates to **Completed**

### How to Cancel a Trip
1. Navigate to **Edit Trip** page for your assigned trip
2. Click **"Cancel Trip"** button in the Trip Actions card
3. (Optional) Provide a reason for cancellation
4. Click **"Confirm Cancellation"** in the modal
5. ✅ Trip status updates to **Cancelled**

**Note**: You cannot cancel completed trips.

## For Admins/Dispatchers

### How to Approve a Trip
1. Navigate to **Edit Trip** page for a pending trip
2. Click **"Approve"** button in the Admin Actions section
3. (Optional) Add approval notes
4. Click **"Approve Trip"** in the modal
5. ✅ Trip status updates to **Approved**

### How to Reject a Trip
1. Navigate to **Edit Trip** page for a pending trip
2. Click **"Reject"** button in the Admin Actions section
3. **Required**: Provide a rejection reason
4. Click **"Reject Trip"** in the modal (enabled only when reason is provided)
5. ✅ Trip status updates to **Rejected**

### How to Force Complete a Trip
1. Navigate to **Edit Trip** page for any trip
2. If trip is **InProgress**, click **"Force Complete"** button
3. Confirm the action
4. ✅ Trip status updates to **Completed** (bypasses normal workflow)

**Use Cases for Force Complete:**
- Driver forgot to complete the trip
- Emergency situations requiring immediate closure
- System issues preventing normal completion

## Trip Status Flow

```
┌─────────┐
│ Pending │ ◄─────────────────┐
└────┬────┘                   │
     │                        │
     ├─► Approve (Admin) ─────┤
     │                        │
     ├─► Reject (Admin) ──────┤
     │                        │
     └─► Cancel (Anyone) ─────┤
                              │
┌──────────┐                  │
│ Approved │                  │
└────┬─────┘                  │
     │                        │
     ├─► Start (Driver) ──────┤
     │                        │
     └─► Cancel (Anyone) ─────┤
                              │
┌────────────┐                │
│ InProgress │                │
└─────┬──────┘                │
      │                       │
      ├─► Complete (Driver) ──┤
      │                       │
      ├─► Cancel (Anyone) ────┤
      │                       │
      └─► Force Complete ─────┤
          (Admin)             │
                              │
┌───────────┐                 │
│ Completed │ (Final State)   │
└───────────┘                 │
                              │
┌───────────┐                 │
│ Cancelled │ ────────────────┘
└───────────┘
     │
     └─► Reactivate to Pending (Admin)

┌──────────┐
│ Rejected │ ────────────────┐
└──────────┘                 │
     │                       │
     └─► Reactivate to Pending (Admin)
```

## Button Visibility

### Driver View
- **Complete Trip**: Visible only when trip is InProgress
- **Cancel Trip**: Visible for all statuses except Completed

### Admin/Dispatcher View
- **Approve**: Visible only when trip is Pending
- **Reject**: Visible only when trip is Pending
- **Force Complete**: Visible only when trip is InProgress

## API Endpoint (for developers)

```http
PUT /api/trips/{id}/status
Authorization: Bearer <token>
Content-Type: application/json

{
  "id": 1,
  "status": 4,  // 0=Pending, 1=Approved, 2=Rejected, 3=InProgress, 4=Completed, 5=Cancelled
  "notes": "Optional notes",
  "rejectionReason": "Required for rejections",
  "forceComplete": false
}
```

## Troubleshooting

### "Invalid status transition" error
- Check that you're transitioning to a valid status
- Verify you have the correct permissions
- Review the status flow diagram above

### "Insufficient permissions" error
- Ensure you're logged in with the correct role
- Drivers can only complete/cancel their assigned trips
- Admins/Dispatchers have full access

### Button is disabled
- Check the current trip status
- Some actions are only available for specific statuses
- Example: Can't complete a Pending trip (must be InProgress)

## Tips

1. **Always add notes**: While optional, notes help maintain a clear audit trail
2. **Rejection reasons are required**: This ensures transparency in decision-making
3. **Force complete sparingly**: Use only when necessary, as it bypasses normal workflow
4. **Check trip details**: Review all trip information before approving or completing

## Support

For issues or questions:
1. Check the detailed documentation in `TRIP_STATUS_WORKFLOW_SUMMARY.md`
2. Review the implementation details in `IMPLEMENTATION_COMPLETE.md`
3. Contact your system administrator

# Trip Types Seed Data

**Date:** 2025-10-27  
**Status:** ✅ Implemented  
**Auto-Seed:** Yes (runs on application startup)

---

## Overview

The application automatically seeds 4 predefined trip types with custom attributes when the database is first initialized. This provides a ready-to-use set of trip categories for common medical transport scenarios.

---

## Seed Data Details

### 1. Emergency 🚨
**Color:** Red (#DC2626)  
**Icon:** alert-circle  
**Use Case:** Emergency medical transport requiring immediate attention

**Attributes (5):**
1. **Patient Age** (number, required)
   - Validation: 0-120 years
   - Placeholder: "Enter patient age"

2. **Patient Name** (text, required)
   - Validation: 2-100 characters
   - Placeholder: "Enter patient name"

3. **Emergency Type** (select, required)
   - Options: Cardiac, Trauma, Respiratory, Stroke, Seizure, Allergic Reaction, Other
   - Placeholder: "Select emergency type"

4. **Priority Level** (select, required)
   - Options: Critical, High, Medium
   - Default: High
   - Placeholder: "Select priority level"

5. **Special Equipment Needed** (textarea, optional)
   - Placeholder: "List any special equipment needed (e.g., defibrillator, oxygen)"

---

### 2. Routine 📅
**Color:** Blue (#2563EB)  
**Icon:** calendar  
**Use Case:** Scheduled routine medical appointments and check-ups

**Attributes (5):**
1. **Patient Name** (text, required)
   - Placeholder: "Enter patient name"

2. **Appointment Time** (date, required)
   - No placeholder (date picker)

3. **Mobility Status** (select, required)
   - Options: Ambulatory, Wheelchair, Stretcher, Assisted Walking
   - Placeholder: "Select mobility status"

4. **Appointment Type** (select, optional)
   - Options: Check-up, Dialysis, Physical Therapy, Specialist Visit, Lab Work, Other
   - Placeholder: "Select appointment type"

5. **Special Instructions** (textarea, optional)
   - Placeholder: "Enter any special instructions"

---

### 3. Transfer 🔄
**Color:** Green (#059669)  
**Icon:** arrow-right-left  
**Use Case:** Patient transfer between medical facilities

**Attributes (6):**
1. **Patient Name** (text, required)
   - Placeholder: "Enter patient name"

2. **From Facility** (text, required)
   - Placeholder: "Enter facility name"

3. **To Facility** (text, required)
   - Placeholder: "Enter facility name"

4. **Medical Records Attached** (boolean, required)
   - Default: false

5. **Medical Escort Required** (boolean, optional)
   - Default: false

6. **Transfer Reason** (textarea, required)
   - Placeholder: "Enter reason for transfer"

---

### 4. Discharge 🏠
**Color:** Purple (#7C3AED)  
**Icon:** home  
**Use Case:** Patient discharge and transport home

**Attributes (4):**
1. **Patient Name** (text, required)
   - Placeholder: "Enter patient name"

2. **Discharge Facility** (text, required)
   - Placeholder: "Enter facility name"

3. **Mobility Assistance** (select, required)
   - Options: None, Wheelchair, Walker, Stretcher, Full Assistance
   - Placeholder: "Select assistance level"

4. **Discharge Instructions** (textarea, optional)
   - Placeholder: "Enter any discharge instructions"

---

## How Seeding Works

### Automatic Seeding
The seed data is automatically applied when:
1. The application starts
2. The database has been migrated
3. No trip types exist in the database

### Seed Logic
```csharp
// In Program.cs
await TripTypeSeedData.SeedTripTypesAsync(context);
```

The seeding method:
1. Checks if any trip types exist
2. If none exist, creates 4 trip types with all attributes
3. Saves everything to the database in one transaction

### Idempotent
The seeding is **idempotent** - it can be run multiple times safely:
- If trip types already exist, it does nothing
- No duplicate data will be created
- Safe to run on every application startup

---

## Customization

### Modifying Seed Data
Edit `AmbulanceRider.API/Data/TripTypeSeedData.cs` to:
- Add more trip types
- Modify existing attributes
- Change colors or icons
- Add/remove options in select fields

### Adding New Trip Type
```csharp
new TripType
{
    Name = "Your Type Name",
    Description = "Description here",
    Color = "#HEX_COLOR",
    Icon = "icon-name",
    IsActive = true,
    DisplayOrder = 5,
    CreatedAt = DateTime.UtcNow,
    Attributes = new List<TripTypeAttribute>
    {
        // Add attributes here
    }
}
```

### Disabling Auto-Seed
To disable automatic seeding, comment out this line in `Program.cs`:
```csharp
// await TripTypeSeedData.SeedTripTypesAsync(context);
```

---

## Testing Seed Data

### 1. Check if Seeded
```bash
# Get all trip types
curl http://localhost:5000/api/triptypes \
  -H "Authorization: Bearer YOUR_TOKEN"
```

### 2. Expected Result
You should see 4 trip types:
- Emergency (Red)
- Routine (Blue)
- Transfer (Green)
- Discharge (Purple)

Each with their respective attributes.

### 3. Verify Attributes
```bash
# Get specific trip type with attributes
curl http://localhost:5000/api/triptypes/1 \
  -H "Authorization: Bearer YOUR_TOKEN"
```

---

## Seed Data Statistics

| Trip Type | Attributes | Required Fields | Optional Fields | Select Fields | Boolean Fields |
|-----------|------------|-----------------|-----------------|---------------|----------------|
| Emergency | 5 | 4 | 1 | 2 | 0 |
| Routine | 5 | 3 | 2 | 2 | 0 |
| Transfer | 6 | 4 | 2 | 0 | 2 |
| Discharge | 4 | 3 | 1 | 1 | 0 |
| **Total** | **20** | **14** | **6** | **5** | **2** |

---

## Benefits

✅ **Ready to Use** - Application comes with working trip types  
✅ **Best Practices** - Examples of proper attribute configuration  
✅ **Common Scenarios** - Covers most medical transport needs  
✅ **Customizable** - Easy to modify or extend  
✅ **Safe** - Idempotent seeding prevents duplicates  
✅ **Automatic** - No manual setup required  

---

## Resetting Seed Data

To reset and re-seed trip types:

### Option 1: Delete and Restart
```sql
-- Delete all trip types (cascade will delete attributes and values)
DELETE FROM trip_types;

-- Restart application to re-seed
```

### Option 2: Database Reset
```bash
# Drop and recreate database
dotnet ef database drop
dotnet ef database update

# Application will auto-seed on next start
```

---

## Example Usage

### Creating a Trip with Seeded Type

```bash
curl -X POST http://localhost:5000/api/trips \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Emergency Transport - Cardiac Event",
    "scheduledStartTime": "2025-10-27T15:00:00Z",
    "fromLatitude": 40.7128,
    "fromLongitude": -74.0060,
    "toLatitude": 40.7589,
    "toLongitude": -73.9851,
    "tripTypeId": 1,
    "attributeValues": [
      {"tripTypeAttributeId": 1, "value": "67"},
      {"tripTypeAttributeId": 2, "value": "John Doe"},
      {"tripTypeAttributeId": 3, "value": "Cardiac"},
      {"tripTypeAttributeId": 4, "value": "Critical"}
    ]
  }'
```

---

## Files

- **Seed Class:** `AmbulanceRider.API/Data/TripTypeSeedData.cs`
- **Program.cs:** Calls seed method on startup
- **Documentation:** This file

---

## Summary

The Trip Types seed data provides a production-ready set of trip categories with well-configured attributes. It automatically seeds on first run and is safe to run multiple times. You can customize it by editing the seed file or add new trip types through the API after initial seeding.

**Status:** ✅ Implemented and Active  
**Trip Types:** 4 predefined  
**Total Attributes:** 20  
**Auto-Seed:** Enabled

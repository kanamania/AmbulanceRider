# Trip Types & Dynamic Attributes Implementation

**Date:** 2025-10-27  
**Status:** ✅ Implemented  
**Objective:** Add flexible trip types with dynamic custom attributes

---

## Overview

This feature allows administrators to define different types of trips (e.g., Emergency, Routine, Transfer, Patient Transport) with custom attributes that can be dynamically added to trips. Each trip type can have its own set of fields with different data types, validation rules, and UI configurations.

---

## Architecture

### Database Schema

```
trip_types
├── id (PK)
├── name (string, required)
├── description (string, optional)
├── color (string, optional) - For UI display
├── icon (string, optional) - Icon name
├── is_active (boolean)
├── display_order (int)
└── BaseModel fields (created_at, updated_at, deleted_at, etc.)

trip_type_attributes
├── id (PK)
├── trip_type_id (FK)
├── name (string, required) - Field name (e.g., "patient_age")
├── label (string, required) - Display label (e.g., "Patient Age")
├── description (string, optional)
├── data_type (string, required) - text, number, date, boolean, select, textarea
├── is_required (boolean)
├── display_order (int)
├── options (JSON string) - For select fields: ["Option1", "Option2"]
├── default_value (string, optional)
├── validation_rules (JSON string, optional)
├── placeholder (string, optional)
├── is_active (boolean)
└── BaseModel fields

trip_attribute_values
├── id (PK)
├── trip_id (FK)
├── trip_type_attribute_id (FK)
├── value (string) - Stores the actual value
└── BaseModel fields

trips
└── trip_type_id (FK, optional) - Links trip to a type
```

---

## Models Created

### 1. TripType
**File:** `AmbulanceRider.API/Models/TripType.cs`

Defines a category of trips with custom attributes.

**Properties:**
- `Name` - Trip type name (e.g., "Emergency", "Routine")
- `Description` - Optional description
- `Color` - Hex color for UI display (e.g., "#FF5733")
- `Icon` - Icon name for UI
- `IsActive` - Whether this type is currently active
- `DisplayOrder` - Sort order in UI
- `Attributes` - Collection of custom attributes
- `Trips` - Collection of trips using this type

### 2. TripTypeAttribute
**File:** `AmbulanceRider.API/Models/TripTypeAttribute.cs`

Defines a custom field/attribute for a trip type.

**Properties:**
- `TripTypeId` - Parent trip type
- `Name` - Field name (e.g., "patient_age")
- `Label` - Display label (e.g., "Patient Age")
- `Description` - Help text
- `DataType` - Field type: text, number, date, boolean, select, textarea
- `IsRequired` - Whether field is mandatory
- `DisplayOrder` - Sort order in form
- `Options` - JSON array for select options
- `DefaultValue` - Default value
- `ValidationRules` - JSON validation rules
- `Placeholder` - Placeholder text
- `IsActive` - Whether field is active

### 3. TripAttributeValue
**File:** `AmbulanceRider.API/Models/TripAttributeValue.cs`

Stores the actual value of a custom attribute for a specific trip.

**Properties:**
- `TripId` - The trip this value belongs to
- `TripTypeAttributeId` - The attribute definition
- `Value` - The actual value (stored as string)

---

## API Endpoints

### Trip Types

#### GET `/api/triptypes`
Get all trip types with their attributes.

**Authorization:** Authenticated  
**Response:** `200 OK`
```json
[
  {
    "id": 1,
    "name": "Emergency",
    "description": "Emergency medical transport",
    "color": "#FF0000",
    "icon": "alert-circle",
    "isActive": true,
    "displayOrder": 1,
    "createdAt": "2025-10-27T20:00:00Z",
    "attributes": [
      {
        "id": 1,
        "tripTypeId": 1,
        "name": "patient_age",
        "label": "Patient Age",
        "description": "Age of the patient",
        "dataType": "number",
        "isRequired": true,
        "displayOrder": 1,
        "options": null,
        "defaultValue": null,
        "validationRules": "{\"min\": 0, \"max\": 120}",
        "placeholder": "Enter patient age",
        "isActive": true
      }
    ]
  }
]
```

#### GET `/api/triptypes/active`
Get only active trip types.

**Authorization:** Authenticated  
**Response:** `200 OK` - Same format as above

#### GET `/api/triptypes/{id}`
Get a specific trip type with all attributes.

**Authorization:** Authenticated  
**Response:** `200 OK` - Single trip type object

#### POST `/api/triptypes`
Create a new trip type.

**Authorization:** Admin, Dispatcher  
**Request Body:**
```json
{
  "name": "Emergency",
  "description": "Emergency medical transport",
  "color": "#FF0000",
  "icon": "alert-circle",
  "isActive": true,
  "displayOrder": 1
}
```

**Response:** `201 Created` - Created trip type object

#### PUT `/api/triptypes/{id}`
Update an existing trip type.

**Authorization:** Admin, Dispatcher  
**Request Body:** Same as POST (all fields optional)  
**Response:** `200 OK` - Updated trip type object

#### DELETE `/api/triptypes/{id}`
Delete a trip type (soft delete).

**Authorization:** Admin  
**Response:** `204 No Content`

### Trip Type Attributes

#### POST `/api/triptypes/attributes`
Create a new attribute for a trip type.

**Authorization:** Admin, Dispatcher  
**Request Body:**
```json
{
  "tripTypeId": 1,
  "name": "patient_age",
  "label": "Patient Age",
  "description": "Age of the patient",
  "dataType": "number",
  "isRequired": true,
  "displayOrder": 1,
  "options": null,
  "defaultValue": null,
  "validationRules": "{\"min\": 0, \"max\": 120}",
  "placeholder": "Enter patient age",
  "isActive": true
}
```

**Data Types:**
- `text` - Single line text input
- `textarea` - Multi-line text input
- `number` - Numeric input
- `date` - Date picker
- `boolean` - Checkbox
- `select` - Dropdown (requires `options` JSON array)

**Response:** `201 Created` - Created attribute object

#### PUT `/api/triptypes/attributes/{id}`
Update an existing attribute.

**Authorization:** Admin, Dispatcher  
**Request Body:** Same as POST (all fields optional)  
**Response:** `200 OK` - Updated attribute object

#### DELETE `/api/triptypes/attributes/{id}`
Delete an attribute (soft delete).

**Authorization:** Admin  
**Response:** `204 No Content`

---

## Trip Integration

### Creating a Trip with Custom Attributes

**Endpoint:** `POST /api/trips`

**Request Body:**
```json
{
  "name": "Emergency Transport",
  "description": "Patient transport to hospital",
  "scheduledStartTime": "2025-10-27T15:00:00Z",
  "fromLatitude": 40.7128,
  "fromLongitude": -74.0060,
  "toLatitude": 40.7589,
  "toLongitude": -73.9851,
  "fromLocationName": "Home",
  "toLocationName": "Hospital",
  "vehicleId": 1,
  "driverId": "guid",
  "tripTypeId": 1,
  "attributeValues": [
    {
      "tripTypeAttributeId": 1,
      "value": "45"
    },
    {
      "tripTypeAttributeId": 2,
      "value": "John Doe"
    }
  ]
}
```

### Trip Response with Attributes

```json
{
  "id": 1,
  "name": "Emergency Transport",
  "tripTypeId": 1,
  "tripType": {
    "id": 1,
    "name": "Emergency",
    "color": "#FF0000",
    "icon": "alert-circle"
  },
  "attributeValues": [
    {
      "id": 1,
      "tripId": 1,
      "tripTypeAttributeId": 1,
      "value": "45",
      "attributeName": "patient_age",
      "attributeLabel": "Patient Age",
      "attributeDataType": "number"
    },
    {
      "id": 2,
      "tripId": 1,
      "tripTypeAttributeId": 2,
      "value": "John Doe",
      "attributeName": "patient_name",
      "attributeLabel": "Patient Name",
      "attributeDataType": "text"
    }
  ]
}
```

---

## Use Cases

### Example 1: Emergency Transport

**Trip Type:** Emergency  
**Color:** Red (#FF0000)  
**Icon:** alert-circle

**Attributes:**
1. Patient Name (text, required)
2. Patient Age (number, required, min: 0, max: 120)
3. Emergency Type (select, required)
   - Options: ["Cardiac", "Trauma", "Respiratory", "Other"]
4. Priority Level (select, required)
   - Options: ["Critical", "High", "Medium"]
5. Special Equipment Needed (textarea, optional)

### Example 2: Routine Transport

**Trip Type:** Routine  
**Color:** Blue (#0000FF)  
**Icon:** calendar

**Attributes:**
1. Patient Name (text, required)
2. Appointment Time (date, required)
3. Mobility Status (select, required)
   - Options: ["Ambulatory", "Wheelchair", "Stretcher"]
4. Special Instructions (textarea, optional)

### Example 3: Patient Transfer

**Trip Type:** Inter-Facility Transfer  
**Color:** Green (#00FF00)  
**Icon:** arrow-right-left

**Attributes:**
1. Patient Name (text, required)
2. From Facility (text, required)
3. To Facility (text, required)
4. Medical Records Attached (boolean, required)
5. Escort Required (boolean, required)
6. Transfer Reason (textarea, required)

---

## UI Implementation Guide

### 1. Trip Type Management Page

**Features:**
- List all trip types with color indicators
- Add/Edit/Delete trip types
- Drag-and-drop to reorder (using `displayOrder`)
- Toggle active/inactive status

### 2. Attribute Configuration

**Features:**
- List attributes for selected trip type
- Add/Edit/Delete attributes
- Configure data type and validation
- Set display order
- Preview how field will look

### 3. Dynamic Trip Form

**Features:**
- Select trip type from dropdown
- Dynamically render form fields based on selected type
- Apply validation rules
- Show/hide fields based on `isActive`
- Display fields in `displayOrder`

**Example Blazor Component:**
```razor
@foreach (var attribute in TripType.Attributes.OrderBy(a => a.DisplayOrder))
{
    @switch (attribute.DataType)
    {
        case "text":
            <InputText @bind-Value="@GetAttributeValue(attribute.Id)" 
                       placeholder="@attribute.Placeholder" />
            break;
        case "number":
            <InputNumber @bind-Value="@GetAttributeValueAsNumber(attribute.Id)" 
                         placeholder="@attribute.Placeholder" />
            break;
        case "select":
            <InputSelect @bind-Value="@GetAttributeValue(attribute.Id)">
                @foreach (var option in ParseOptions(attribute.Options))
                {
                    <option value="@option">@option</option>
                }
            </InputSelect>
            break;
        case "boolean":
            <InputCheckbox @bind-Value="@GetAttributeValueAsBool(attribute.Id)" />
            break;
        case "date":
            <InputDate @bind-Value="@GetAttributeValueAsDate(attribute.Id)" />
            break;
        case "textarea":
            <InputTextArea @bind-Value="@GetAttributeValue(attribute.Id)" 
                           placeholder="@attribute.Placeholder" 
                           rows="4" />
            break;
    }
}
```

---

## Files Created

### Models (3 files)
1. `AmbulanceRider.API/Models/TripType.cs`
2. `AmbulanceRider.API/Models/TripTypeAttribute.cs`
3. `AmbulanceRider.API/Models/TripAttributeValue.cs`

### DTOs (3 files)
1. `AmbulanceRider.API/DTOs/TripTypeDto.cs`
2. `AmbulanceRider.API/DTOs/TripTypeAttributeDto.cs`
3. `AmbulanceRider.API/DTOs/TripAttributeValueDto.cs`

### Repositories (6 files)
1. `AmbulanceRider.API/Repositories/ITripTypeRepository.cs`
2. `AmbulanceRider.API/Repositories/TripTypeRepository.cs`
3. `AmbulanceRider.API/Repositories/ITripTypeAttributeRepository.cs`
4. `AmbulanceRider.API/Repositories/TripTypeAttributeRepository.cs`
5. `AmbulanceRider.API/Repositories/ITripAttributeValueRepository.cs`
6. `AmbulanceRider.API/Repositories/TripAttributeValueRepository.cs`

### Services (2 files)
1. `AmbulanceRider.API/Services/ITripTypeService.cs`
2. `AmbulanceRider.API/Services/TripTypeService.cs`

### Controllers (1 file)
1. `AmbulanceRider.API/Controllers/TripTypesController.cs`

### Modified Files (5 files)
1. `AmbulanceRider.API/Models/Trip.cs` - Added TripTypeId and AttributeValues
2. `AmbulanceRider.API/DTOs/TripDto.cs` - Added TripType and AttributeValues
3. `AmbulanceRider.API/Data/ApplicationDbContext.cs` - Added DbSets and configurations
4. `AmbulanceRider.API/Services/TripService.cs` - Added attribute value handling
5. `AmbulanceRider.API/Program.cs` - Registered new services and repositories

---

## Next Steps

### 1. Create Database Migration

```bash
cd AmbulanceRider.API
dotnet ef migrations add AddTripTypesAndAttributes
dotnet ef database update
```

### 2. Seed Initial Data (Optional)

Create some default trip types:
- Emergency (Red, high priority)
- Routine (Blue, standard)
- Transfer (Green, inter-facility)

### 3. Build UI Components

Create Blazor components for:
- Trip type management
- Attribute configuration
- Dynamic trip form
- Trip type selector

### 4. Add Validation

Implement client-side and server-side validation based on `validationRules` JSON.

---

## Benefits

✅ **Flexibility** - Add new trip types without code changes  
✅ **Customization** - Each trip type can have unique fields  
✅ **Scalability** - Easy to add new attributes as needs evolve  
✅ **Type Safety** - Strongly typed models with validation  
✅ **UI Friendly** - Includes display metadata (labels, placeholders, colors)  
✅ **Soft Delete** - All entities support soft delete  
✅ **Audit Trail** - BaseModel tracks creation and updates  

---

## Summary

The Trip Types & Dynamic Attributes system provides a flexible, extensible way to categorize trips and collect custom data. Administrators can define trip types with custom fields through the API, and the UI can dynamically render forms based on these configurations. This eliminates the need for code changes when business requirements evolve.

**Status:** ✅ Backend Implementation Complete  
**Next:** Create database migration and build UI components

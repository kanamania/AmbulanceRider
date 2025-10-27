# Trip Types Quick Start Guide

**Date:** 2025-10-27  
**Feature:** Dynamic Trip Types with Custom Attributes

---

## What Are Trip Types?

Trip Types allow you to categorize trips and add custom fields specific to each type. For example:
- **Emergency** trips might need patient age, emergency type, and priority level
- **Routine** trips might need appointment time and mobility status
- **Transfer** trips might need facility names and medical records info

---

## Quick Setup (5 Steps)

### Step 1: Create a Trip Type

**Endpoint:** `POST /api/triptypes`

```bash
curl -X POST http://localhost:5000/api/triptypes \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Emergency",
    "description": "Emergency medical transport",
    "color": "#FF0000",
    "icon": "alert-circle",
    "isActive": true,
    "displayOrder": 1
  }'
```

**Response:**
```json
{
  "id": 1,
  "name": "Emergency",
  "color": "#FF0000",
  "attributes": []
}
```

### Step 2: Add Custom Attributes

**Endpoint:** `POST /api/triptypes/attributes`

**Example 1: Patient Age (Number Field)**
```bash
curl -X POST http://localhost:5000/api/triptypes/attributes \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "tripTypeId": 1,
    "name": "patient_age",
    "label": "Patient Age",
    "description": "Age of the patient",
    "dataType": "number",
    "isRequired": true,
    "displayOrder": 1,
    "validationRules": "{\"min\": 0, \"max\": 120}",
    "placeholder": "Enter patient age",
    "isActive": true
  }'
```

**Example 2: Emergency Type (Select Field)**
```bash
curl -X POST http://localhost:5000/api/triptypes/attributes \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "tripTypeId": 1,
    "name": "emergency_type",
    "label": "Emergency Type",
    "dataType": "select",
    "isRequired": true,
    "displayOrder": 2,
    "options": "[\"Cardiac\", \"Trauma\", \"Respiratory\", \"Stroke\", \"Other\"]",
    "placeholder": "Select emergency type",
    "isActive": true
  }'
```

**Example 3: Special Instructions (Text Area)**
```bash
curl -X POST http://localhost:5000/api/triptypes/attributes \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "tripTypeId": 1,
    "name": "special_instructions",
    "label": "Special Instructions",
    "dataType": "textarea",
    "isRequired": false,
    "displayOrder": 3,
    "placeholder": "Any special instructions or notes",
    "isActive": true
  }'
```

### Step 3: View Trip Types

**Endpoint:** `GET /api/triptypes/active`

```bash
curl http://localhost:5000/api/triptypes/active \
  -H "Authorization: Bearer YOUR_TOKEN"
```

**Response:**
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
    "attributes": [
      {
        "id": 1,
        "name": "patient_age",
        "label": "Patient Age",
        "dataType": "number",
        "isRequired": true,
        "displayOrder": 1
      },
      {
        "id": 2,
        "name": "emergency_type",
        "label": "Emergency Type",
        "dataType": "select",
        "options": "[\"Cardiac\", \"Trauma\", \"Respiratory\", \"Stroke\", \"Other\"]",
        "isRequired": true,
        "displayOrder": 2
      }
    ]
  }
]
```

### Step 4: Create a Trip with Custom Attributes

**Endpoint:** `POST /api/trips`

```bash
curl -X POST http://localhost:5000/api/trips \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Emergency Transport - Cardiac Event",
    "description": "Patient experiencing chest pain",
    "scheduledStartTime": "2025-10-27T15:00:00Z",
    "fromLatitude": 40.7128,
    "fromLongitude": -74.0060,
    "toLatitude": 40.7589,
    "toLongitude": -73.9851,
    "fromLocationName": "Patient Home",
    "toLocationName": "City Hospital ER",
    "vehicleId": 1,
    "driverId": "guid-here",
    "tripTypeId": 1,
    "attributeValues": [
      {
        "tripTypeAttributeId": 1,
        "value": "67"
      },
      {
        "tripTypeAttributeId": 2,
        "value": "Cardiac"
      },
      {
        "tripTypeAttributeId": 3,
        "value": "Patient has history of heart disease. Bring defibrillator."
      }
    ]
  }'
```

### Step 5: View Trip with Attributes

**Endpoint:** `GET /api/trips/{id}`

**Response:**
```json
{
  "id": 1,
  "name": "Emergency Transport - Cardiac Event",
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
      "value": "67",
      "attributeName": "patient_age",
      "attributeLabel": "Patient Age",
      "attributeDataType": "number"
    },
    {
      "id": 2,
      "tripId": 1,
      "tripTypeAttributeId": 2,
      "value": "Cardiac",
      "attributeName": "emergency_type",
      "attributeLabel": "Emergency Type",
      "attributeDataType": "select"
    }
  ]
}
```

---

## Data Types Reference

### 1. Text
Single-line text input
```json
{
  "dataType": "text",
  "placeholder": "Enter patient name"
}
```

### 2. Textarea
Multi-line text input
```json
{
  "dataType": "textarea",
  "placeholder": "Enter detailed notes"
}
```

### 3. Number
Numeric input with optional validation
```json
{
  "dataType": "number",
  "validationRules": "{\"min\": 0, \"max\": 120}",
  "placeholder": "Enter age"
}
```

### 4. Date
Date picker
```json
{
  "dataType": "date",
  "defaultValue": "2025-10-27"
}
```

### 5. Boolean
Checkbox
```json
{
  "dataType": "boolean",
  "defaultValue": "false"
}
```

### 6. Select
Dropdown with predefined options
```json
{
  "dataType": "select",
  "options": "[\"Option 1\", \"Option 2\", \"Option 3\"]"
}
```

---

## Common Use Cases

### Emergency Transport
```json
{
  "name": "Emergency",
  "color": "#FF0000",
  "attributes": [
    {"name": "patient_age", "dataType": "number", "isRequired": true},
    {"name": "patient_name", "dataType": "text", "isRequired": true},
    {"name": "emergency_type", "dataType": "select", "isRequired": true},
    {"name": "priority_level", "dataType": "select", "isRequired": true},
    {"name": "special_equipment", "dataType": "textarea", "isRequired": false}
  ]
}
```

### Routine Appointment
```json
{
  "name": "Routine",
  "color": "#0000FF",
  "attributes": [
    {"name": "patient_name", "dataType": "text", "isRequired": true},
    {"name": "appointment_time", "dataType": "date", "isRequired": true},
    {"name": "mobility_status", "dataType": "select", "isRequired": true},
    {"name": "special_instructions", "dataType": "textarea", "isRequired": false}
  ]
}
```

### Inter-Facility Transfer
```json
{
  "name": "Transfer",
  "color": "#00FF00",
  "attributes": [
    {"name": "patient_name", "dataType": "text", "isRequired": true},
    {"name": "from_facility", "dataType": "text", "isRequired": true},
    {"name": "to_facility", "dataType": "text", "isRequired": true},
    {"name": "medical_records_attached", "dataType": "boolean", "isRequired": true},
    {"name": "escort_required", "dataType": "boolean", "isRequired": false},
    {"name": "transfer_reason", "dataType": "textarea", "isRequired": true}
  ]
}
```

---

## Management Operations

### Update Trip Type
```bash
curl -X PUT http://localhost:5000/api/triptypes/1 \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Emergency - Updated",
    "color": "#CC0000",
    "isActive": true
  }'
```

### Update Attribute
```bash
curl -X PUT http://localhost:5000/api/triptypes/attributes/1 \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "label": "Patient Age (Years)",
    "isRequired": false
  }'
```

### Deactivate Trip Type
```bash
curl -X PUT http://localhost:5000/api/triptypes/1 \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "isActive": false
  }'
```

### Delete Trip Type
```bash
curl -X DELETE http://localhost:5000/api/triptypes/1 \
  -H "Authorization: Bearer YOUR_TOKEN"
```

---

## Validation Rules Format

Validation rules are stored as JSON strings. Examples:

### Number Validation
```json
{
  "min": 0,
  "max": 120,
  "step": 1
}
```

### Text Validation
```json
{
  "minLength": 3,
  "maxLength": 100,
  "pattern": "^[A-Za-z ]+$"
}
```

### Date Validation
```json
{
  "minDate": "2025-01-01",
  "maxDate": "2025-12-31"
}
```

---

## Best Practices

### 1. Naming Conventions
- Use snake_case for attribute names: `patient_age`, `emergency_type`
- Use descriptive labels: "Patient Age", "Emergency Type"

### 2. Display Order
- Order fields logically (most important first)
- Group related fields together
- Use increments of 10 (10, 20, 30) to allow easy reordering

### 3. Required Fields
- Only mark truly essential fields as required
- Provide clear descriptions for required fields

### 4. Select Options
- Keep option lists concise (< 10 options)
- Use clear, unambiguous option names
- Consider adding an "Other" option

### 5. Colors and Icons
- Use consistent color coding:
  - Red (#FF0000) - Emergency/Critical
  - Blue (#0000FF) - Routine/Standard
  - Green (#00FF00) - Transfer/Non-urgent
  - Yellow (#FFFF00) - Scheduled/Planned

---

## Troubleshooting

### Issue: Attribute not showing in trip response
**Solution:** Ensure the attribute `isActive` is set to `true`

### Issue: Validation not working
**Solution:** Check that `validationRules` is valid JSON format

### Issue: Select dropdown empty
**Solution:** Verify `options` field contains valid JSON array: `["Option1", "Option2"]`

### Issue: Can't create trip with attributes
**Solution:** Ensure `tripTypeAttributeId` matches existing attribute IDs

---

## Summary

✅ **Step 1:** Create trip type  
✅ **Step 2:** Add custom attributes  
✅ **Step 3:** View available types  
✅ **Step 4:** Create trips with attribute values  
✅ **Step 5:** View trips with all custom data  

The Trip Types system is now ready to use! You can create as many trip types as needed, each with its own custom fields, without any code changes.

**Next Steps:**
1. Run database migration: `dotnet ef database update`
2. Create your first trip type via API
3. Build UI components to manage trip types
4. Integrate dynamic forms in trip creation UI

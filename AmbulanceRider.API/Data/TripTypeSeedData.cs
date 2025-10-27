using AmbulanceRider.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AmbulanceRider.API.Data;

public static class TripTypeSeedData
{
    public static async Task SeedTripTypesAsync(ApplicationDbContext context)
    {
        // Check if trip types already exist
        if (await context.TripTypes.AnyAsync())
        {
            return; // Database has been seeded
        }

        var tripTypes = new List<TripType>
        {
            // 1. Emergency Transport
            new TripType
            {
                Name = "Emergency",
                Description = "Emergency medical transport requiring immediate attention",
                Color = "#DC2626", // Red
                Icon = "alert-circle",
                IsActive = true,
                DisplayOrder = 1,
                CreatedAt = DateTime.UtcNow,
                Attributes = new List<TripTypeAttribute>
                {
                    new TripTypeAttribute
                    {
                        Name = "patient_age",
                        Label = "Patient Age",
                        Description = "Age of the patient in years",
                        DataType = "number",
                        IsRequired = true,
                        DisplayOrder = 1,
                        ValidationRules = "{\"min\": 0, \"max\": 120}",
                        Placeholder = "Enter patient age",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new TripTypeAttribute
                    {
                        Name = "patient_name",
                        Label = "Patient Name",
                        Description = "Full name of the patient",
                        DataType = "text",
                        IsRequired = true,
                        DisplayOrder = 2,
                        ValidationRules = "{\"minLength\": 2, \"maxLength\": 100}",
                        Placeholder = "Enter patient name",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new TripTypeAttribute
                    {
                        Name = "emergency_type",
                        Label = "Emergency Type",
                        Description = "Type of medical emergency",
                        DataType = "select",
                        IsRequired = true,
                        DisplayOrder = 3,
                        Options = "[\"Cardiac\", \"Trauma\", \"Respiratory\", \"Stroke\", \"Seizure\", \"Allergic Reaction\", \"Other\"]",
                        Placeholder = "Select emergency type",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new TripTypeAttribute
                    {
                        Name = "priority_level",
                        Label = "Priority Level",
                        Description = "Urgency level of the emergency",
                        DataType = "select",
                        IsRequired = true,
                        DisplayOrder = 4,
                        Options = "[\"Critical\", \"High\", \"Medium\"]",
                        DefaultValue = "High",
                        Placeholder = "Select priority level",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new TripTypeAttribute
                    {
                        Name = "special_equipment",
                        Label = "Special Equipment Needed",
                        Description = "Any special medical equipment required",
                        DataType = "textarea",
                        IsRequired = false,
                        DisplayOrder = 5,
                        Placeholder = "List any special equipment needed (e.g., defibrillator, oxygen)",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    }
                }
            },

            // 2. Routine Appointment
            new TripType
            {
                Name = "Routine",
                Description = "Scheduled routine medical appointments and check-ups",
                Color = "#2563EB", // Blue
                Icon = "calendar",
                IsActive = true,
                DisplayOrder = 2,
                CreatedAt = DateTime.UtcNow,
                Attributes = new List<TripTypeAttribute>
                {
                    new TripTypeAttribute
                    {
                        Name = "patient_name",
                        Label = "Patient Name",
                        Description = "Full name of the patient",
                        DataType = "text",
                        IsRequired = true,
                        DisplayOrder = 1,
                        Placeholder = "Enter patient name",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new TripTypeAttribute
                    {
                        Name = "appointment_time",
                        Label = "Appointment Time",
                        Description = "Scheduled appointment date and time",
                        DataType = "date",
                        IsRequired = true,
                        DisplayOrder = 2,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new TripTypeAttribute
                    {
                        Name = "mobility_status",
                        Label = "Mobility Status",
                        Description = "Patient's mobility level",
                        DataType = "select",
                        IsRequired = true,
                        DisplayOrder = 3,
                        Options = "[\"Ambulatory\", \"Wheelchair\", \"Stretcher\", \"Assisted Walking\"]",
                        Placeholder = "Select mobility status",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new TripTypeAttribute
                    {
                        Name = "appointment_type",
                        Label = "Appointment Type",
                        Description = "Type of medical appointment",
                        DataType = "select",
                        IsRequired = false,
                        DisplayOrder = 4,
                        Options = "[\"Check-up\", \"Dialysis\", \"Physical Therapy\", \"Specialist Visit\", \"Lab Work\", \"Other\"]",
                        Placeholder = "Select appointment type",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new TripTypeAttribute
                    {
                        Name = "special_instructions",
                        Label = "Special Instructions",
                        Description = "Any special instructions or notes",
                        DataType = "textarea",
                        IsRequired = false,
                        DisplayOrder = 5,
                        Placeholder = "Enter any special instructions",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    }
                }
            },

            // 3. Inter-Facility Transfer
            new TripType
            {
                Name = "Transfer",
                Description = "Patient transfer between medical facilities",
                Color = "#059669", // Green
                Icon = "arrow-right-left",
                IsActive = true,
                DisplayOrder = 3,
                CreatedAt = DateTime.UtcNow,
                Attributes = new List<TripTypeAttribute>
                {
                    new TripTypeAttribute
                    {
                        Name = "patient_name",
                        Label = "Patient Name",
                        Description = "Full name of the patient",
                        DataType = "text",
                        IsRequired = true,
                        DisplayOrder = 1,
                        Placeholder = "Enter patient name",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new TripTypeAttribute
                    {
                        Name = "from_facility",
                        Label = "From Facility",
                        Description = "Name of the originating facility",
                        DataType = "text",
                        IsRequired = true,
                        DisplayOrder = 2,
                        Placeholder = "Enter facility name",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new TripTypeAttribute
                    {
                        Name = "to_facility",
                        Label = "To Facility",
                        Description = "Name of the destination facility",
                        DataType = "text",
                        IsRequired = true,
                        DisplayOrder = 3,
                        Placeholder = "Enter facility name",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new TripTypeAttribute
                    {
                        Name = "medical_records_attached",
                        Label = "Medical Records Attached",
                        Description = "Are medical records included with transfer?",
                        DataType = "boolean",
                        IsRequired = true,
                        DisplayOrder = 4,
                        DefaultValue = "false",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new TripTypeAttribute
                    {
                        Name = "escort_required",
                        Label = "Medical Escort Required",
                        Description = "Does patient require medical escort?",
                        DataType = "boolean",
                        IsRequired = false,
                        DisplayOrder = 5,
                        DefaultValue = "false",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new TripTypeAttribute
                    {
                        Name = "transfer_reason",
                        Label = "Transfer Reason",
                        Description = "Reason for the facility transfer",
                        DataType = "textarea",
                        IsRequired = true,
                        DisplayOrder = 6,
                        Placeholder = "Enter reason for transfer",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    }
                }
            },

            // 4. Discharge Transport
            new TripType
            {
                Name = "Discharge",
                Description = "Patient discharge and transport home",
                Color = "#7C3AED", // Purple
                Icon = "home",
                IsActive = true,
                DisplayOrder = 4,
                CreatedAt = DateTime.UtcNow,
                Attributes = new List<TripTypeAttribute>
                {
                    new TripTypeAttribute
                    {
                        Name = "patient_name",
                        Label = "Patient Name",
                        Description = "Full name of the patient",
                        DataType = "text",
                        IsRequired = true,
                        DisplayOrder = 1,
                        Placeholder = "Enter patient name",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new TripTypeAttribute
                    {
                        Name = "discharge_facility",
                        Label = "Discharge Facility",
                        Description = "Name of the facility discharging patient",
                        DataType = "text",
                        IsRequired = true,
                        DisplayOrder = 2,
                        Placeholder = "Enter facility name",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new TripTypeAttribute
                    {
                        Name = "mobility_assistance",
                        Label = "Mobility Assistance",
                        Description = "Level of assistance needed",
                        DataType = "select",
                        IsRequired = true,
                        DisplayOrder = 3,
                        Options = "[\"None\", \"Wheelchair\", \"Walker\", \"Stretcher\", \"Full Assistance\"]",
                        Placeholder = "Select assistance level",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new TripTypeAttribute
                    {
                        Name = "discharge_instructions",
                        Label = "Discharge Instructions",
                        Description = "Special care instructions from facility",
                        DataType = "textarea",
                        IsRequired = false,
                        DisplayOrder = 4,
                        Placeholder = "Enter any discharge instructions",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    }
                }
            }
        };

        await context.TripTypes.AddRangeAsync(tripTypes);
        await context.SaveChangesAsync();
    }
}

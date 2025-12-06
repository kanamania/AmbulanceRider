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
            // 1. Medical Assistance
            new TripType
            {
                Name = "Medical Assistance",
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
                        Name = "patient_name",
                        Label = "Patient Name",
                        Description = "Full name of the patient",
                        DataType = "text",
                        IsRequired = true,
                        DisplayOrder = 1,
                        ValidationRules = "{\"minLength\": 2, \"maxLength\": 100}",
                        Placeholder = "Enter patient name",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new TripTypeAttribute
                    {
                        Name = "patient_age",
                        Label = "Patient Age",
                        Description = "Age of the patient in years",
                        DataType = "number",
                        IsRequired = true,
                        DisplayOrder = 2,
                        ValidationRules = "{\"min\": 0, \"max\": 120}",
                        Placeholder = "Enter patient age",
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
                        Options =
                            "[\"Cardiac\", \"Trauma\", \"Respiratory\", \"Stroke\", \"Seizure\", \"Allergic Reaction\", \"Other\"]",
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
                    },
                    new TripTypeAttribute
                    {
                        Name = "contact_person_name",
                        Label = "Contact Person Name",
                        Description = "Contact Person Name",
                        DataType = "text",
                        IsRequired = true,
                        DisplayOrder = 6,
                        Placeholder = "Enter Contact Person Name",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new TripTypeAttribute
                    {
                        Name = "contact_person_phone",
                        Label = "Contact Person Phone Number",
                        Description = "Contact Person Phone Number",
                        DataType = "number",
                        IsRequired = true,
                        DisplayOrder = 7,
                        Placeholder = "Enter Contact Person Phone Number",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new TripTypeAttribute
                    {
                        Name = "contact_person_address",
                        Label = "Contact Person Address",
                        Description = "Contact Person Address",
                        DataType = "text",
                        IsRequired = true,
                        DisplayOrder = 8,
                        Placeholder = "Enter Contact Person Address",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                }
            },

            // 2. Parcel Delivery
            new TripType
            {
                Name = "Parcel Delivery",
                Description = "Parcel Delivery",
                Color = "#2563EB", // Blue
                Icon = "calendar",
                IsActive = true,
                DisplayOrder = 2,
                CreatedAt = DateTime.UtcNow,
                Attributes = new List<TripTypeAttribute>
                {
                    new TripTypeAttribute
                    {
                        Name = "parcel_size",
                        Label = "Parcel Size",
                        Description = "Parcel Size from Pricing Matrix",
                        DataType = "text",
                        IsRequired = true,
                        DisplayOrder = 1,
                        Placeholder = "Enter patient name",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new TripTypeAttribute
                    {
                        Name = "confidential",
                        Label = "Confidential?",
                        Description = "Is Parcel confidential?",
                        DataType = "select",
                        Options = "[\"Yes\", \"No\"]",
                        IsRequired = true,
                        DisplayOrder = 2,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new TripTypeAttribute
                    {
                        Name = "fragile",
                        Label = "Fragile?",
                        Description = "Is Parcel fragile?",
                        DataType = "select",
                        Options = "[\"Yes\", \"No\"]",
                        IsRequired = true,
                        DisplayOrder = 3,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new TripTypeAttribute
                    {
                        Name = "signature",
                        Label = "Signature required?",
                        Description = "Is Signature required?",
                        DataType = "select",
                        Options = "[\"Yes\", \"No\"]",
                        IsRequired = true,
                        DisplayOrder = 4,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new TripTypeAttribute
                    {
                        Name = "receiver_phone",
                        Label = "Parcel Receiver Phone Number",
                        Description = "Parcel Receiver Phone Number",
                        DataType = "number",
                        IsRequired = true,
                        DisplayOrder = 5,
                        Placeholder = "Enter Parcel Receiver Phone Number",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                }
            },
        };

        await context.TripTypes.AddRangeAsync(tripTypes);
        await context.SaveChangesAsync();
    }
}
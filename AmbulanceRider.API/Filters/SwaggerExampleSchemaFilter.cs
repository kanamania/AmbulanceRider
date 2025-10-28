using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AmbulanceRider.API.Filters;

/// <summary>
/// Swagger schema filter to add examples to DTOs
/// </summary>
public class SwaggerExampleSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == null)
            return;

        // Add examples based on type name
        switch (context.Type.Name)
        {
            case "LoginDto":
                schema.Example = new OpenApiObject
                {
                    ["email"] = new OpenApiString("admin@ambulancerider.com"),
                    ["password"] = new OpenApiString("Admin@123"),
                    ["telemetry"] = new OpenApiObject
                    {
                        ["latitude"] = new OpenApiDouble(-1.2921),
                        ["longitude"] = new OpenApiDouble(36.8219),
                        ["accuracy"] = new OpenApiDouble(10.5),
                        ["deviceInfo"] = new OpenApiString("Mozilla/5.0")
                    }
                };
                break;

            case "RegisterDto":
                schema.Example = new OpenApiObject
                {
                    ["email"] = new OpenApiString("newuser@example.com"),
                    ["password"] = new OpenApiString("SecurePass@123"),
                    ["confirmPassword"] = new OpenApiString("SecurePass@123"),
                    ["firstName"] = new OpenApiString("John"),
                    ["lastName"] = new OpenApiString("Doe"),
                    ["phoneNumber"] = new OpenApiString("+254712345678"),
                    ["role"] = new OpenApiString("User"),
                    ["telemetry"] = new OpenApiObject
                    {
                        ["latitude"] = new OpenApiDouble(-1.2921),
                        ["longitude"] = new OpenApiDouble(36.8219)
                    }
                };
                break;

            case "CreateTripDto":
                schema.Example = new OpenApiObject
                {
                    ["pickupLocationId"] = new OpenApiInteger(1),
                    ["dropoffLocationId"] = new OpenApiInteger(2),
                    ["vehicleId"] = new OpenApiInteger(1),
                    ["driverId"] = new OpenApiString("00000000-0000-0000-0000-000000000000"),
                    ["tripTypeId"] = new OpenApiInteger(1),
                    ["patientName"] = new OpenApiString("Jane Smith"),
                    ["patientAge"] = new OpenApiInteger(35),
                    ["patientGender"] = new OpenApiString("Female"),
                    ["emergencyLevel"] = new OpenApiString("High"),
                    ["notes"] = new OpenApiString("Patient experiencing chest pain"),
                    ["scheduledPickupTime"] = new OpenApiString(DateTime.UtcNow.AddHours(1).ToString("o"))
                };
                break;

            case "CreateVehicleDto":
                schema.Example = new OpenApiObject
                {
                    ["plateNumber"] = new OpenApiString("KAA 123B"),
                    ["model"] = new OpenApiString("Toyota Land Cruiser"),
                    ["year"] = new OpenApiInteger(2023),
                    ["vehicleTypeId"] = new OpenApiInteger(1),
                    ["capacity"] = new OpenApiInteger(4),
                    ["status"] = new OpenApiString("Available")
                };
                break;

            case "CreateLocationDto":
                schema.Example = new OpenApiObject
                {
                    ["name"] = new OpenApiString("Kenyatta National Hospital"),
                    ["address"] = new OpenApiString("Hospital Road, Upper Hill"),
                    ["city"] = new OpenApiString("Nairobi"),
                    ["latitude"] = new OpenApiDouble(-1.3018),
                    ["longitude"] = new OpenApiDouble(36.8073),
                    ["locationType"] = new OpenApiString("Hospital")
                };
                break;

            case "UpdateTripStatusDto":
                schema.Example = new OpenApiObject
                {
                    ["status"] = new OpenApiString("InProgress"),
                    ["notes"] = new OpenApiString("Driver en route to pickup location"),
                    ["currentLatitude"] = new OpenApiDouble(-1.2921),
                    ["currentLongitude"] = new OpenApiDouble(36.8219)
                };
                break;

            case "UpdateProfileDto":
                schema.Example = new OpenApiObject
                {
                    ["firstName"] = new OpenApiString("John"),
                    ["lastName"] = new OpenApiString("Doe"),
                    ["phoneNumber"] = new OpenApiString("+254712345678")
                };
                break;

            case "ChangePasswordDto":
                schema.Example = new OpenApiObject
                {
                    ["currentPassword"] = new OpenApiString("OldPassword@123"),
                    ["newPassword"] = new OpenApiString("NewSecurePass@456"),
                    ["confirmPassword"] = new OpenApiString("NewSecurePass@456")
                };
                break;
        }
    }
}

using AmbulanceRider.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AmbulanceRider.API.Data;

public static class DataSeeder
{
    public static async Task SeedData(ApplicationDbContext context)
    {
        // Check if roles already exist
        if (!context.Roles.Any())
        {
            // Add roles
            var roles = new List<Role>
            {
                new() { Id = 1, Name = "Admin" },
                new() { Id = 2, Name = "Driver" },
                new() { Id = 4, Name = "User" }
            };
            await context.Roles.AddRangeAsync(roles);

            // Add admin user
            var adminUser = new User
            {
                Id = 1,
                FirstName = "System",
                LastName = "Admin",
                Email = "demo@gmail.com",
                PhoneNumber = "+1234567890",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await context.Users.AddAsync(adminUser);

            // Assign admin role
            await context.UserRoles.AddAsync(new UserRole { Id = 1, UserId = 1, RoleId = 1 });

            // Add vehicle types
            var vehicleTypes = new List<VehicleType>
            {
                new() { Id = 1, Name = "Ambulance" },
                new() { Id = 2, Name = "Boda Boda" }
            };
            await context.VehicleTypes.AddRangeAsync(vehicleTypes);

            await context.SaveChangesAsync();
        }
    }}

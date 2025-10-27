using AmbulanceRider.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AmbulanceRider.API.Data;

public static class DataSeeder
{
    public static async Task SeedData(ApplicationDbContext context, UserManager<User> userManager,
        RoleManager<Role> roleManager)
    {
        // Check if roles already exist
        if (!await roleManager.Roles.AnyAsync())
        {
            // Add roles
            var roles = new List<string> { "Admin", "Driver", "User" };
            foreach (var roleName in roles)
            {
                await roleManager.CreateAsync(new Role { Name = roleName });
            }

            // Add admin user
            var adminUser = new User
            {
                UserName = "demo@gmail.com",
                FirstName = "System",
                LastName = "Admin",
                Email = "demo@gmail.com",
                PhoneNumber = "+1234567890",
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(adminUser, "Admin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // Add vehicle types
            var vehicleTypes = new List<VehicleType>
            {
                new() { Id = 1, Name = "Ambulance" },
                new() { Id = 2, Name = "Boda Boda" }
            };
            await context.VehicleTypes.AddRangeAsync(vehicleTypes);

            await context.SaveChangesAsync();
        }
    }
}
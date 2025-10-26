using AmbulanceRider.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AmbulanceRider.API.Data;

public static class DataSeeder
{
    public static void SeedData(ModelBuilder modelBuilder)
    {
        // Seed default roles
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Admin" },
            new Role { Id = 2, Name = "Driver" },
            new Role { Id = 4, Name = "User" }
        );

        // Seed default admin user (password: Admin@123)
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                FirstName = "System",
                LastName = "Admin",
                Email = "demo@gmail.com",
                PhoneNumber = "+1234567890",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        );

        // Assign admin role to admin user
        modelBuilder.Entity<UserRole>().HasData(
            new UserRole { Id = 1, UserId = 1, RoleId = 1 } // Admin role
        );

        // Seed vehicle types
        modelBuilder.Entity<VehicleType>().HasData(
            new VehicleType { Id = 1, Name = "Ambulance" },
            new VehicleType { Id = 2, Name = "Boda Boda" }
        );
    }
}

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
            var roles = new List<string> { "Admin", "Dispatcher", "Driver", "User" };
            foreach (var roleName in roles)
            {
                await roleManager.CreateAsync(new Role { Name = roleName });
            }

            // Add test users for each role
            var testUsers = new List<(User User, string Password, string Role)>
            {
                // Admin users
                (new User
                {
                    UserName = "admin@ambulance.com",
                    FirstName = "John",
                    LastName = "Admin",
                    Email = "admin@ambulance.com",
                    PhoneNumber = "+255712345001",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                }, "Admin@123", "Admin"),
                
                (new User
                {
                    UserName = "sarah.admin@ambulance.com",
                    FirstName = "Sarah",
                    LastName = "Johnson",
                    Email = "sarah.admin@ambulance.com",
                    PhoneNumber = "+255712345002",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                }, "Admin@123", "Admin"),
                
                // Driver users
                (new User
                {
                    UserName = "driver1@ambulance.com",
                    FirstName = "Michael",
                    LastName = "Driver",
                    Email = "driver1@ambulance.com",
                    PhoneNumber = "+255712345101",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                }, "Driver@123", "Driver"),
                
                (new User
                {
                    UserName = "driver2@ambulance.com",
                    FirstName = "James",
                    LastName = "Wilson",
                    Email = "driver2@ambulance.com",
                    PhoneNumber = "+255712345102",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                }, "Driver@123", "Driver"),
                
                (new User
                {
                    UserName = "driver3@ambulance.com",
                    FirstName = "David",
                    LastName = "Brown",
                    Email = "driver3@ambulance.com",
                    PhoneNumber = "+255712345103",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                }, "Driver@123", "Driver"),
                
                (new User
                {
                    UserName = "driver4@ambulance.com",
                    FirstName = "Robert",
                    LastName = "Taylor",
                    Email = "driver4@ambulance.com",
                    PhoneNumber = "+255712345104",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                }, "Driver@123", "Driver"),
                
                // Regular users
                (new User
                {
                    UserName = "user1@ambulance.com",
                    FirstName = "Emily",
                    LastName = "User",
                    Email = "user1@ambulance.com",
                    PhoneNumber = "+255712345201",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                }, "User@123", "User"),
                
                (new User
                {
                    UserName = "user2@ambulance.com",
                    FirstName = "Jessica",
                    LastName = "Martinez",
                    Email = "user2@ambulance.com",
                    PhoneNumber = "+255712345202",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                }, "User@123", "User"),
                
                (new User
                {
                    UserName = "user3@ambulance.com",
                    FirstName = "Daniel",
                    LastName = "Anderson",
                    Email = "user3@ambulance.com",
                    PhoneNumber = "+255712345203",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                }, "User@123", "User")
            };

            // Create all test users
            foreach (var (user, password, role) in testUsers)
            {
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }

            // Add vehicle types
            var vehicleTypes = new List<VehicleType>
            {
                new() { Id = 1, Name = "Ambulance" },
                new() { Id = 2, Name = "Boda Boda" },
                new() { Id = 3, Name = "Emergency Van" }
            };
            await context.VehicleTypes.AddRangeAsync(vehicleTypes);
            await context.SaveChangesAsync();

            // Add test vehicles
            var vehicles = new List<Vehicle>
            {
                new()
                {
                    Name = "Ambulance Unit 1",
                    PlateNumber = "T 123 ABC",
                    VehicleTypeId = 1,
                    Image = "/images/ambulance1.jpg",
                    CreatedAt = DateTime.UtcNow
                },
                new()
                {
                    Name = "Ambulance Unit 2",
                    PlateNumber = "T 456 DEF",
                    VehicleTypeId = 1,
                    Image = "/images/ambulance2.jpg",
                    CreatedAt = DateTime.UtcNow
                },
                new()
                {
                    Name = "Ambulance Unit 3",
                    PlateNumber = "T 789 GHI",
                    VehicleTypeId = 1,
                    Image = "/images/ambulance3.jpg",
                    CreatedAt = DateTime.UtcNow
                },
                new()
                {
                    Name = "Emergency Van 1",
                    PlateNumber = "T 321 JKL",
                    VehicleTypeId = 3,
                    Image = "/images/van1.jpg",
                    CreatedAt = DateTime.UtcNow
                },
                new()
                {
                    Name = "Boda Boda 1",
                    PlateNumber = "MC 111 AAA",
                    VehicleTypeId = 2,
                    Image = "/images/boda1.jpg",
                    CreatedAt = DateTime.UtcNow
                },
                new()
                {
                    Name = "Boda Boda 2",
                    PlateNumber = "MC 222 BBB",
                    VehicleTypeId = 2,
                    Image = "/images/boda2.jpg",
                    CreatedAt = DateTime.UtcNow
                },
                new()
                {
                    Name = "Boda Boda 3",
                    PlateNumber = "MC 333 CCC",
                    VehicleTypeId = 2,
                    Image = "/images/boda3.jpg",
                    CreatedAt = DateTime.UtcNow
                }
            };
            await context.Vehicles.AddRangeAsync(vehicles);
            await context.SaveChangesAsync();

            // Add locations in Dar es Salaam
            var locations = new List<Location>
            {
                // Hospitals
                new()
                {
                    Name = "Muhimbili National Hospital",
                    Latitude = -6.8162,
                    Longitude = 39.2803,
                    CreatedAt = DateTime.UtcNow
                },
                new()
                {
                    Name = "Aga Khan Hospital, Ocean Road, Upanga East",
                    Latitude = -6.8145,
                    Longitude = 39.2886,
                    CreatedAt = DateTime.UtcNow
                },
                new()
                {
                    Name = "Mwananyamala Hospital, Kinondoni",
                    Latitude = -6.7731,
                    Longitude = 39.2297,
                    CreatedAt = DateTime.UtcNow
                },
                new()
                {
                    Name = "Temeke Hospital",
                    Latitude = -6.8502,
                    Longitude = 39.2644,
                    CreatedAt = DateTime.UtcNow
                },
                new()
                {
                    Name = "Regency Medical Centre, Ali Hassan Mwinyi Road, Kinondoni",
                    Latitude = -6.7690,
                    Longitude = 39.2472,
                    CreatedAt = DateTime.UtcNow
                },
                
                // Key locations
                new()
                {
                    Name = "Julius Nyerere International Airport, Julius Nyerere Road",
                    Latitude = -6.8781,
                    Longitude = 39.2026,
                    CreatedAt = DateTime.UtcNow
                },
                new()
                {
                    Name = "Kariakoo Market, Ilala",
                    Latitude = -6.8186,
                    Longitude = 39.2742,
                    CreatedAt = DateTime.UtcNow
                },
                new()
                {
                    Name = "Mlimani City Mall, Sam Nujoma Road, Ubungo",
                    Latitude = -6.7704,
                    Longitude = 39.2294,
                    CreatedAt = DateTime.UtcNow
                },
                new()
                {
                    Name = "University of Dar es Salaam, University Road, Ubungo",
                    Latitude = -6.7762,
                    Longitude = 39.2064,
                    CreatedAt = DateTime.UtcNow
                },
                new()
                {
                    Name = "Coco Beach, Msasani Peninsula",
                    Latitude = -6.7583,
                    Longitude = 39.2722,
                    CreatedAt = DateTime.UtcNow
                },
                new()
                {
                    Name = "Posta, Azikiwe Street, City Centre",
                    Latitude = -6.8133,
                    Longitude = 39.2864,
                    CreatedAt = DateTime.UtcNow
                },
                new()
                {
                    Name = "Ubungo Bus Terminal, Morogoro Road, Ubungo",
                    Latitude = -6.7814,
                    Longitude = 39.2536,
                    CreatedAt = DateTime.UtcNow
                },
                new()
                {
                    Name = "Mwenge, Kinondoni",
                    Latitude = -6.7634,
                    Longitude = 39.2294,
                    CreatedAt = DateTime.UtcNow
                },
                new()
                {
                    Name = "Masaki Peninsula, Kinondoni",
                    Latitude = -6.7644,
                    Longitude = 39.2706,
                    CreatedAt = DateTime.UtcNow
                },
                new()
                {
                    Name = "Mikocheni",
                    Latitude = -6.7700,
                    Longitude = 39.2500,
                    CreatedAt = DateTime.UtcNow
                }
            };
            await context.Locations.AddRangeAsync(locations);
            await context.SaveChangesAsync();
        }
    }
}
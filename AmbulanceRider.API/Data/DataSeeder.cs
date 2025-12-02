using AmbulanceRider.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AmbulanceRider.API.Data;

public static class DataSeeder
{
    private static async Task<HashSet<string>> GetExistingEmailsAsync(UserManager<User> userManager)
    {
        return new HashSet<string>(
            await userManager.Users.Select(u => u.Email).ToListAsync(),
            StringComparer.OrdinalIgnoreCase
        );
    }

    private static async Task<HashSet<string>> GetExistingPlateNumbersAsync(ApplicationDbContext context)
    {
        return new HashSet<string>(
            await context.Vehicles.Select(v => v.PlateNumber).ToListAsync(),
            StringComparer.OrdinalIgnoreCase
        );
    }

    private static async Task<HashSet<string>> GetExistingLocationNamesAsync(ApplicationDbContext context)
    {
        return new HashSet<string>(
            await context.Locations.Select(l => l.Name).ToListAsync(),
            StringComparer.OrdinalIgnoreCase
        );
    }

    public static async Task SeedData(ApplicationDbContext context, UserManager<User> userManager,
        RoleManager<Role> roleManager, IConfiguration configuration)
    {
        // Check if seeding is enabled via environment variable
        var enableSeeding = configuration.GetValue<bool>("EnableSeeding", false);
        if (!enableSeeding)
        {
            return;
        }

        // Ensure default company exists
        const string defaultCompanyName = "Default Company";
        var defaultCompany = await context.Companies
            .FirstOrDefaultAsync(c => c.Name == defaultCompanyName && c.DeletedAt == null);

        if (defaultCompany == null)
        {
            defaultCompany = new Company
            {
                Name = defaultCompanyName,
                Description = "Default company",
                ContactEmail = "contact@globalexpress.co.tz",
                ContactPhone = "+255700000001",
                Address = "Dar es Salaam, Tanzania",
                CreatedAt = DateTime.UtcNow
            };

            await context.Companies.AddAsync(defaultCompany);
            await context.SaveChangesAsync();
        }

        // Seed roles if they don't exist
        if (!await roleManager.Roles.AnyAsync())
        {
            var roles = new List<string> { "Admin", "Dispatcher", "Driver", "User" };
            foreach (var roleName in roles)
            {
                await roleManager.CreateAsync(new Role { Name = roleName });
            }

            // Get existing emails to avoid duplicates
            var existingEmails = await GetExistingEmailsAsync(userManager);

            // Add test users for each role
            var testUsers = new List<(User User, string Password, string Role)>
            {
                // Admin users
                (new User
                {
                    UserName = "admin@ambulance.com",
                    FirstName = "John",
                    LastName = "Admin",
                    Email = "demo@gmail.com",
                    PhoneNumber = "+255712345001",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow,
                    CompanyId = defaultCompany.Id
                }, "Admin@123", "Admin"),
                
                (new User
                {
                    UserName = "sarah.admin@ambulance.com",
                    FirstName = "Sarah",
                    LastName = "Johnson",
                    Email = "sarah.admin@ambulance.com",
                    PhoneNumber = "+255712345002",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow,
                    CompanyId = defaultCompany.Id
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
                    CreatedAt = DateTime.UtcNow,
                    CompanyId = defaultCompany.Id
                }, "Driver@123", "Driver"),
                
                (new User
                {
                    UserName = "driver2@ambulance.com",
                    FirstName = "James",
                    LastName = "Wilson",
                    Email = "driver2@ambulance.com",
                    PhoneNumber = "+255712345102",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow,
                    CompanyId = defaultCompany.Id
                }, "Driver@123", "Driver"),
                
                (new User
                {
                    UserName = "driver3@ambulance.com",
                    FirstName = "David",
                    LastName = "Brown",
                    Email = "driver3@ambulance.com",
                    PhoneNumber = "+255712345103",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow,
                    CompanyId = defaultCompany.Id
                }, "Driver@123", "Driver"),
                
                (new User
                {
                    UserName = "driver4@ambulance.com",
                    FirstName = "Robert",
                    LastName = "Taylor",
                    Email = "driver4@ambulance.com",
                    PhoneNumber = "+255712345104",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow,
                    CompanyId = defaultCompany.Id
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
                    CreatedAt = DateTime.UtcNow,
                    CompanyId = defaultCompany.Id
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
                    CreatedAt = DateTime.UtcNow,
                    CompanyId = defaultCompany.Id
                }, "User@123", "User")
            };

            // Create users that don't already exist
            foreach (var (user, password, role) in testUsers.Where(x => !existingEmails.Contains(x.User.Email)))
            {
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }

        // Ensure seeded users are linked to the default company
        var seededUserEmails = new[]
        {
            "demo@gmail.com",
            "sarah.admin@ambulance.com",
            "driver1@ambulance.com",
            "driver2@ambulance.com",
            "driver3@ambulance.com",
            "driver4@ambulance.com",
            "user1@ambulance.com",
            "user2@ambulance.com",
            "user3@ambulance.com",
            "demo@gmail.com"
        };

        var usersNeedingCompany = await userManager.Users
            .Where(u => seededUserEmails.Contains(u.Email) && u.CompanyId != defaultCompany.Id)
            .ToListAsync();

        foreach (var seededUser in usersNeedingCompany)
        {
            seededUser.CompanyId = defaultCompany.Id;
            await userManager.UpdateAsync(seededUser);
        }

        // Seed vehicle types if they don't exist
        if (!await context.VehicleTypes.AnyAsync())
        {
            var vehicleTypes = new List<VehicleType>
            {
                new() { Id = 1, Name = "Ambulance" },
                new() { Id = 2, Name = "Boda Boda" },
                new() { Id = 3, Name = "Emergency Van" }
            };
            await context.VehicleTypes.AddRangeAsync(vehicleTypes);
            await context.SaveChangesAsync();
        }

        // Seed vehicles - check each one individually
        // Ensure vehicle types exist before seeding vehicles
        if (await context.VehicleTypes.AnyAsync())
        {
            // Get vehicle type IDs dynamically
            var ambulanceTypeId = await context.VehicleTypes
                .Where(vt => vt.Name == "Ambulance")
                .Select(vt => vt.Id)
                .FirstOrDefaultAsync();
            
            var bodaBodaTypeId = await context.VehicleTypes
                .Where(vt => vt.Name == "Boda Boda")
                .Select(vt => vt.Id)
                .FirstOrDefaultAsync();
            
            var emergencyVanTypeId = await context.VehicleTypes
                .Where(vt => vt.Name == "Emergency Van")
                .Select(vt => vt.Id)
                .FirstOrDefaultAsync();

            // Only proceed if we have the required vehicle types
            if (ambulanceTypeId > 0 && bodaBodaTypeId > 0)
            {
                // Get existing plate numbers
                var existingPlateNumbers = await GetExistingPlateNumbersAsync(context);

                var vehicles = new List<Vehicle>
                {
                    new()
                    {
                        Name = "Ambulance Unit 1",
                        PlateNumber = "T 123 ABC",
                        VehicleTypeId = ambulanceTypeId,
                        Image = "/images/ambulance1.jpg",
                        CreatedAt = DateTime.UtcNow
                    },
                    new()
                    {
                        Name = "Ambulance Unit 2",
                        PlateNumber = "T 456 DEF",
                        VehicleTypeId = ambulanceTypeId,
                        Image = "/images/ambulance2.jpg",
                        CreatedAt = DateTime.UtcNow
                    },
                    new()
                    {
                        Name = "Ambulance Unit 3",
                        PlateNumber = "T 789 GHI",
                        VehicleTypeId = ambulanceTypeId,
                        Image = "/images/ambulance3.jpg",
                        CreatedAt = DateTime.UtcNow
                    },
                    new()
                    {
                        Name = "Ambulance Unit 4",
                        PlateNumber = "T 114 ABC",
                        VehicleTypeId = ambulanceTypeId,
                        Image = "/images/ambulance4.jpg",
                        CreatedAt = DateTime.UtcNow
                    },
                    new()
                    {
                        Name = "Boda Boda 1",
                        PlateNumber = "MC 111 AAA",
                        VehicleTypeId = bodaBodaTypeId,
                        Image = "/images/boda1.jpg",
                        CreatedAt = DateTime.UtcNow
                    },
                    new()
                    {
                        Name = "Boda Boda 2",
                        PlateNumber = "MC 222 BBB",
                        VehicleTypeId = bodaBodaTypeId,
                        Image = "/images/boda2.jpg",
                        CreatedAt = DateTime.UtcNow
                    },
                    new()
                    {
                        Name = "Boda Boda 3",
                        PlateNumber = "MC 333 CCC",
                        VehicleTypeId = bodaBodaTypeId,
                        Image = "/images/boda3.jpg",
                        CreatedAt = DateTime.UtcNow
                    }
                };

                // Add Emergency Van if the type exists
                if (emergencyVanTypeId > 0)
                {
                    vehicles.Add(new Vehicle
                    {
                        Name = "Emergency Van 1",
                        PlateNumber = "T 321 JKL",
                        VehicleTypeId = emergencyVanTypeId,
                        Image = "/images/van1.jpg",
                        CreatedAt = DateTime.UtcNow
                    });
                }

                // Filter out vehicles that already exist
                var newVehicles = vehicles.Where(v => !existingPlateNumbers.Contains(v.PlateNumber)).ToList();

                if (newVehicles.Any())
                {
                    await context.Vehicles.AddRangeAsync(newVehicles);
                    await context.SaveChangesAsync();
                }

                // Seed driver assignments to vehicles
                var vehicleDriverSeeds = new List<(string PlateNumber, string[] DriverEmails)>
                {
                    ("T 123 ABC", new[] { "driver1@ambulance.com" }),
                    ("T 114 ABC", new[] { "driver2@ambulance.com" }),
                    ("T 456 DEF", new[] { "driver3@ambulance.com" }),
                    ("T 789 GHI", new[] { "driver4@ambulance.com" }),
                    ("MC 111 AAA", new[] { "driver2@ambulance.com" }),
                    ("MC 222 BBB", new[] { "driver3@ambulance.com" }),
                    ("MC 333 CCC", new[] { "driver1@ambulance.com" })
                };

                var platesToSeed = vehicleDriverSeeds
                    .Select(vds => vds.PlateNumber)
                    .Distinct()
                    .ToList();

                var driverEmailsToSeed = vehicleDriverSeeds
                    .SelectMany(vds => vds.DriverEmails)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                var vehiclesByPlate = await context.Vehicles
                    .Where(v => platesToSeed.Contains(v.PlateNumber))
                    .ToDictionaryAsync(v => v.PlateNumber, v => v);

                var driversByEmail = await userManager.Users
                    .Where(u => u.Email != null && driverEmailsToSeed.Contains(u.Email))
                    .ToDictionaryAsync(u => u.Email!, u => u, StringComparer.OrdinalIgnoreCase);

                var vehicleIds = vehiclesByPlate.Values.Select(v => v.Id).ToList();
                var driverIds = driversByEmail.Values.Select(d => d.Id).ToList();

                var existingAssignments = await context.VehicleDrivers
                    .Where(vd => vehicleIds.Contains(vd.VehicleId) && driverIds.Contains(vd.UserId))
                    .Select(vd => new { vd.VehicleId, vd.UserId })
                    .ToListAsync();

                var existingSet = new HashSet<(int VehicleId, Guid UserId)>(
                    existingAssignments.Select(ea => (ea.VehicleId, ea.UserId))
                );

                var assignmentsToAdd = new List<VehicleDriver>();

                foreach (var seed in vehicleDriverSeeds)
                {
                    if (!vehiclesByPlate.TryGetValue(seed.PlateNumber, out var vehicle))
                    {
                        continue;
                    }

                    foreach (var driverEmail in seed.DriverEmails)
                    {
                        if (!driversByEmail.TryGetValue(driverEmail, out var driver))
                        {
                            continue;
                        }

                        var key = (vehicle.Id, driver.Id);
                        if (existingSet.Contains(key))
                        {
                            continue;
                        }

                        assignmentsToAdd.Add(new VehicleDriver
                        {
                            VehicleId = vehicle.Id,
                            UserId = driver.Id,
                            CreatedAt = DateTime.UtcNow,
                            CreatedBy = Guid.Empty
                        });
                        existingSet.Add(key);
                    }
                }

                if (assignmentsToAdd.Any())
                {
                    await context.VehicleDrivers.AddRangeAsync(assignmentsToAdd);
                    await context.SaveChangesAsync();
                }
            }
        }

        // Seed locations - check each one individually
        // Get existing location names
        var existingLocationNames = await GetExistingLocationNamesAsync(context);

        var locations = new List<Location>
            {
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

        // Filter out locations that already exist
        var newLocations = locations.Where(l => !existingLocationNames.Contains(l.Name)).ToList();

        if (newLocations.Any())
        {
            await context.Locations.AddRangeAsync(newLocations);
            await context.SaveChangesAsync();
        }
    }
}
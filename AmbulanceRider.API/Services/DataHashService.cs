using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using AmbulanceRider.API.Data;
using AmbulanceRider.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AmbulanceRider.API.Services
{
    public interface IDataHashService
    {
        Task<string> GenerateUserHashAsync(string userId);
        Task<string> GenerateProfileHashAsync(string userId);
        Task<string> GenerateTripTypesHashAsync();
        Task<string> GenerateLocationsHashAsync();
        Task<string> GenerateTripsHashAsync(string userId);
        Task<string> GenerateDriversHashAsync();
    }

    public class DataHashService : IDataHashService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public DataHashService(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<string> GenerateUserHashAsync(string userId)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Where(u => u.Id == Guid.Parse(userId))
                .Select(u => new
                {
                    u.Id,
                    u.FirstName,
                    u.LastName,
                    u.Email,
                    u.PhoneNumber,
                    u.ImagePath,
                    u.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return string.Empty;

            var json = JsonSerializer.Serialize(user);
            return ComputeHash(json);
        }

        public async Task<string> GenerateProfileHashAsync(string userId)
        {
            // Same as user hash for now, can be extended with additional profile data
            return await GenerateUserHashAsync(userId);
        }

        public async Task<string> GenerateTripTypesHashAsync()
        {
            var tripTypes = await _context.TripTypes
                .AsNoTracking()
                .Include(t => t.Attributes)
                .Where(t => t.IsActive)
                .OrderBy(t => t.Id)
                .Select(t => new
                {
                    t.Id,
                    t.Name,
                    t.Description,
                    t.IsActive,
                    t.UpdatedAt,
                    Attributes = t.Attributes
                        .Where(a => a.IsActive)
                        .OrderBy(a => a.DisplayOrder)
                        .Select(a => new
                        {
                            a.Id,
                            a.Name,
                            a.Label,
                            a.DataType,
                            a.IsRequired,
                            a.DisplayOrder,
                            a.ValidationRules
                        })
                })
                .ToListAsync();

            var json = JsonSerializer.Serialize(tripTypes);
            return ComputeHash(json);
        }

        public async Task<string> GenerateLocationsHashAsync()
        {
            var locations = await _context.Locations
                .AsNoTracking()
                .OrderBy(l => l.Id)
                .Select(l => new
                {
                    l.Id,
                    l.Name,
                    l.Latitude,
                    l.Longitude,
                    l.UpdatedAt
                })
                .ToListAsync();

            var json = JsonSerializer.Serialize(locations);
            return ComputeHash(json);
        }
        public async Task<string> GenerateDriversHashAsync()
        {
            var locations = await _context.Users
                .AsNoTracking()
                .OrderBy(l => l.Id)
                .Select(l => new
                {
                    l.Id,
                    l.FirstName,
                    l.LastName,
                    l.Email,
                    l.PhoneNumber,
                    l.ImagePath,
                    l.ImageUrl
                })
                .ToListAsync();

            var json = JsonSerializer.Serialize(locations);
            return ComputeHash(json);
        }

        public async Task<string> GenerateTripsHashAsync(string userId)
        {
            var userGuid = Guid.Parse(userId);
            var user = await _userManager.FindByIdAsync(userId);
            
            if (user == null)
                return string.Empty;

            var roles = await _userManager.GetRolesAsync(user);
            var isAdminOrDispatcher = roles.Any(r => r.Equals("Admin", StringComparison.OrdinalIgnoreCase) || 
                                                      r.Equals("Dispatcher", StringComparison.OrdinalIgnoreCase));

            // Admin and Dispatcher get all trips, Driver and User get only their trips
            var tripsQuery = _context.Trips.AsNoTracking();
            
            if (!isAdminOrDispatcher)
            {
                // Filter to only trips created by or assigned to this user
                tripsQuery = tripsQuery.Where(t => t.CreatedBy == userGuid || t.DriverId == userGuid);
            }

            var trips = await tripsQuery
                .OrderByDescending(t => t.UpdatedAt ?? t.CreatedAt)
                .Select(t => new
                {
                    t.Id,
                    t.Name,
                    t.Status,
                    t.FromLatitude,
                    t.FromLongitude,
                    t.ToLatitude,
                    t.ToLongitude,
                    t.FromLocationName,
                    t.ToLocationName,
                    t.VehicleId,
                    t.DriverId,
                    t.EstimatedDistance,
                    t.EstimatedDuration,
                    t.CreatedAt,
                    t.UpdatedAt,
                    t.ApprovedAt,
                    t.ActualStartTime,
                    t.ActualEndTime
                })
                .ToListAsync();

            var json = JsonSerializer.Serialize(trips);
            return ComputeHash(json);
        }

        private static string ComputeHash(string input)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}

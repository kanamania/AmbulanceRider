using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace AmbulanceRider.API.Models;

public class User : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    [StringLength(256)]
    public string? ImagePath { get; set; }
    [StringLength(256)]
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    
    // Computed property
    public string FullName => $"{FirstName} {LastName}";
    
    // Navigation properties
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<VehicleDriver> VehicleDrivers { get; set; } = new List<VehicleDriver>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public ICollection<Trip> DriverTrips { get; set; } = new List<Trip>();
}

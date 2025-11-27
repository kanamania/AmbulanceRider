using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
    [Column("CreatedBy")]
    [NotMapped]
    public User? Creator { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    [Column("UpdatedBy")]
    [NotMapped]
    public User? Updater { get; set; }
    public Guid? UpdatedBy { get; set; }
    [Column("DeletedBy")]
    [NotMapped]
    public User? Deleter { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
    
    // Company relationship
    public int? CompanyId { get; set; }
    public virtual Company? Company { get; set; }
    
    // Computed property
    public string FullName => $"{FirstName} {LastName}";
    
    // Navigation properties
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<VehicleDriver> VehicleDrivers { get; set; } = new List<VehicleDriver>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public ICollection<Trip> DriverTrips { get; set; } = new List<Trip>();
}

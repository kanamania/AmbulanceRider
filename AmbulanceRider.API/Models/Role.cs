using Microsoft.AspNetCore.Identity;

namespace AmbulanceRider.API.Models;

public class Role : IdentityRole<Guid>
{
    // Navigation properties
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}

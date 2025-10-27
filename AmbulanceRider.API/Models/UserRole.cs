using Microsoft.AspNetCore.Identity;

namespace AmbulanceRider.API.Models;

public class UserRole : IdentityUserRole<Guid>
{
    // Navigation properties
    public User? User { get; set; }
    public Role? Role { get; set; }
}

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace AmbulanceRider.API.DTOs;

public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? ImagePath { get; set; }
    public string? ImageUrl { get; set; }
    public List<string> Roles { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateUserDto
{
    [Required(ErrorMessage = "First name is required")]
    public string FirstName { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Last name is required")]
    public string LastName { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;
    
    public string PhoneNumber { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
    public string Password { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "At least one role is required")]
    [MinLength(1, ErrorMessage = "At least one role is required")]
    public List<int> RoleIds { get; set; } = new();
    
    public string? ImagePath { get; set; }
    public string? ImageUrl { get; set; }
}

public class UpdateUserDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Password { get; set; }
    public bool RemoveImage { get; set; }
    public List<int>? RoleIds { get; set; }
    public string? ImagePath { get; set; }
    public string? ImageUrl { get; set; }
}

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public UserDto User { get; set; } = null!;
}

public class UpdateProfileDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
}

public class ChangePasswordDto
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class UpdateProfileImageDto
{
    public IFormFile Image { get; set; } = null!;
}

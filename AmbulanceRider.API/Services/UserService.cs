using AmbulanceRider.API.Data;
using AmbulanceRider.API.DTOs;
using AmbulanceRider.API.Models;
using AmbulanceRider.API.Repositories;
using Microsoft.AspNetCore.Identity;
using System.IO;

namespace AmbulanceRider.API.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;

    public UserService(IUserRepository userRepository, ApplicationDbContext context, UserManager<User> userManager)
    {
        _userRepository = userRepository;
        _context = context;
        _userManager = userManager;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllWithRolesAsync();
        return users.Select(MapToDto);
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdWithRolesAsync(id);
        return user == null ? null : MapToDto(user);
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
    {
        // Check if user already exists using UserManager
        var existingUser = await _userManager.FindByEmailAsync(createUserDto.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("User with this email already exists");
        }

        var user = new User
        {
            UserName = createUserDto.Email, // Required by Identity
            FirstName = createUserDto.FirstName,
            LastName = createUserDto.LastName,
            Email = createUserDto.Email,
            PhoneNumber = createUserDto.PhoneNumber,
            CreatedAt = DateTime.UtcNow
        };

        // Handle image properties if provided
        if (createUserDto.Image != null)
        {
            user.ImagePath = createUserDto.ImagePath;
            user.ImageUrl = createUserDto.ImageUrl;
        }

        // Use UserManager to create user with password (handles normalization and hashing)
        var result = await _userManager.CreateAsync(user, createUserDto.Password);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        // Add roles using role names from IDs
        foreach (var roleId in createUserDto.RoleIds)
        {
            var role = await _context.Roles.FindAsync(Guid.Parse(roleId.ToString()));
            if (role != null)
            {
                await _userManager.AddToRoleAsync(user, role.Name!);
            }
        }

        return (await GetUserByIdAsync(user.Id))!;
    }

    public async Task<UserDto> UpdateUserAsync(Guid id, UpdateUserDto updateUserDto)
    {
        var user = await _userRepository.GetByIdWithRolesAsync(id);
        if (user == null)
            throw new KeyNotFoundException("User not found");

        if (!string.IsNullOrEmpty(updateUserDto.FirstName))
            user.FirstName = updateUserDto.FirstName;
        if (!string.IsNullOrEmpty(updateUserDto.LastName))
            user.LastName = updateUserDto.LastName;
        if (!string.IsNullOrEmpty(updateUserDto.Email) && user.Email != updateUserDto.Email)
        {
            var existingUser = await _userManager.FindByEmailAsync(updateUserDto.Email);
            if (existingUser != null && existingUser.Id != id)
            {
                throw new InvalidOperationException("Email is already in use by another user");
            }

            user.Email = updateUserDto.Email;
            user.UserName = updateUserDto.Email; // Update username to match email
            user.NormalizedEmail = updateUserDto.Email.ToUpperInvariant();
            user.NormalizedUserName = updateUserDto.Email.ToUpperInvariant();
        }

        if (!string.IsNullOrEmpty(updateUserDto.PhoneNumber))
            user.PhoneNumber = updateUserDto.PhoneNumber;
        if (!string.IsNullOrEmpty(updateUserDto.Password))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _userManager.ResetPasswordAsync(user, token, updateUserDto.Password);
        }

        // Handle image path/url updates
        if (updateUserDto.ImagePath != null)
            user.ImagePath = updateUserDto.ImagePath;
        if (updateUserDto.ImageUrl != null)
            user.ImageUrl = updateUserDto.ImageUrl;

        user.UpdatedAt = DateTime.UtcNow;

        // Update roles if provided
        if (updateUserDto.RoleIds != null && updateUserDto.RoleIds.Any())
        {
            // Get current roles
            var currentRoles = await _userManager.GetRolesAsync(user);
            
            // Remove all current roles
            if (currentRoles.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
            }

            // Add new roles
            foreach (var roleId in updateUserDto.RoleIds)
            {
                var role = await _context.Roles.FindAsync(Guid.Parse(roleId.ToString()));
                if (role != null)
                {
                    await _userManager.AddToRoleAsync(user, role.Name!);
                }
            }
        }

        await _userManager.UpdateAsync(user);

        return (await GetUserByIdAsync(user.Id))!;
    }

    public async Task DeleteUserAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            throw new KeyNotFoundException("User not found");

        // Delete the image file if it exists
        if (!string.IsNullOrEmpty(user.ImagePath))
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.ImagePath);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id.ToString(),
            Name = $"{user.FirstName} {user.LastName}",
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email!,
            PhoneNumber = user.PhoneNumber,
            ImagePath = user.ImagePath,
            ImageUrl = user.ImageUrl,
            Roles = user.UserRoles?.Select(ur => ur.Role?.Name ?? string.Empty)
                .Where(name => !string.IsNullOrEmpty(name)).ToList() ?? new List<string>(),
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
}
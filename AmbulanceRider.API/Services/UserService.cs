using AmbulanceRider.API.Data;
using AmbulanceRider.API.DTOs;
using AmbulanceRider.API.Models;
using AmbulanceRider.API.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.IO;

namespace AmbulanceRider.API.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUserRepository userRepository, 
        ApplicationDbContext context, 
        UserManager<User> userManager,
        ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _context = context;
        _userManager = userManager;
        _logger = logger;
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
        // Log password length for debugging (don't log actual password)
        _logger.LogInformation("Creating user with email: {Email}, Password length: {Length}", 
            createUserDto.Email, createUserDto.Password?.Length ?? 0);

        // Validate password is not null or empty
        if (string.IsNullOrWhiteSpace(createUserDto.Password))
        {
            throw new InvalidOperationException("Password is required and cannot be empty");
        }

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
            ImagePath = createUserDto.ImagePath,
            ImageUrl = createUserDto.ImageUrl,
            CreatedAt = DateTime.UtcNow
        };

        // Use UserManager to create user with password (handles normalization and hashing)
        var result = await _userManager.CreateAsync(user, createUserDto.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            _logger.LogError("Failed to create user {Email}. Errors: {Errors}", createUserDto.Email, errors);
            throw new InvalidOperationException(errors);
        }

        // Add roles
        _logger.LogInformation("Adding {RoleCount} roles to user {Email}: {Roles}", 
            createUserDto.Roles?.Count ?? 0, user.Email, string.Join(", ", createUserDto.Roles ?? new List<string>()));
        
        if (createUserDto.Roles != null && createUserDto.Roles.Any())
        {
            foreach (var roleName in createUserDto.Roles)
            {
                var roleResult = await _userManager.AddToRoleAsync(user, roleName);
                if (!roleResult.Succeeded)
                {
                    var roleErrors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    _logger.LogWarning("Failed to add role {RoleName} to user {Email}. Errors: {Errors}", 
                        roleName, user.Email, roleErrors);
                }
                else
                {
                    _logger.LogInformation("Successfully added role {RoleName} to user {Email}", roleName, user.Email);
                }
            }
        }

        _logger.LogInformation("Successfully created user {Email} with ID {UserId}", user.Email, user.Id);
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

        // Update roles if provided (null means don't update, empty list means remove all)
        if (updateUserDto.Roles != null)
        {
            _logger.LogInformation("Updating roles for user {Email}. New roles: {Roles}", 
                user.Email, updateUserDto.Roles.Any() ? string.Join(", ", updateUserDto.Roles) : "(none)");
            
            // Get current roles
            var currentRoles = await _userManager.GetRolesAsync(user);
            _logger.LogInformation("Current roles for user {Email}: {CurrentRoles}", 
                user.Email, currentRoles.Any() ? string.Join(", ", currentRoles) : "(none)");
            
            // Remove all current roles
            foreach (var role in currentRoles)
            {
                await _userManager.RemoveFromRoleAsync(user, role);
            }

            // Add new roles (if any)
            if (updateUserDto.Roles.Any())
            {
                foreach (var roleName in updateUserDto.Roles)
                {
                    var roleResult = await _userManager.AddToRoleAsync(user, roleName);
                    if (!roleResult.Succeeded)
                    {
                        var roleErrors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                        _logger.LogWarning("Failed to add role {RoleName} to user {Email}. Errors: {Errors}", 
                            roleName, user.Email, roleErrors);
                    }
                    else
                    {
                        _logger.LogInformation("Successfully added role {RoleName} to user {Email}", roleName, user.Email);
                    }
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
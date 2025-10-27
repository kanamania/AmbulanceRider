using AmbulanceRider.API.Data;
using AmbulanceRider.API.DTOs;
using AmbulanceRider.API.Models;
using AmbulanceRider.API.Repositories;
using System.IO;

namespace AmbulanceRider.API.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ApplicationDbContext _context;

    public UserService(IUserRepository userRepository, ApplicationDbContext context)
    {
        _userRepository = userRepository;
        _context = context;
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
        var existingUser = await _userRepository.GetByEmailAsync(createUserDto.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("User with this email already exists");
        }

        var user = new User
        {
            FirstName = createUserDto.FirstName,
            LastName = createUserDto.LastName,
            Email = createUserDto.Email,
            PhoneNumber = createUserDto.PhoneNumber,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password),
            CreatedAt = DateTime.UtcNow
        };

        // Handle image properties if provided
        if (createUserDto.Image != null)
        {
            user.ImagePath = createUserDto.ImagePath;
            user.ImageUrl = createUserDto.ImageUrl;
        }

        await _userRepository.AddAsync(user);

        // Add roles
        foreach (var roleId in createUserDto.RoleIds)
        {
            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = Guid.Parse(roleId.ToString())
            };
            _context.UserRoles.Add(userRole);
        }

        await _context.SaveChangesAsync();

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
            var existingUser = await _userRepository.GetByEmailAsync(updateUserDto.Email);
            if (existingUser != null && existingUser.Id != id)
            {
                throw new InvalidOperationException("Email is already in use by another user");
            }

            user.Email = updateUserDto.Email;
        }

        if (!string.IsNullOrEmpty(updateUserDto.PhoneNumber))
            user.PhoneNumber = updateUserDto.PhoneNumber;
        if (!string.IsNullOrEmpty(updateUserDto.Password))
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updateUserDto.Password);

        // Handle image path/url updates
        if (updateUserDto.ImagePath != null)
            user.ImagePath = updateUserDto.ImagePath;
        if (updateUserDto.ImageUrl != null)
            user.ImageUrl = updateUserDto.ImageUrl;

        user.UpdatedAt = DateTime.UtcNow;

        // Update roles if provided
        if (updateUserDto.RoleIds != null && updateUserDto.RoleIds.Any())
        {
            // Remove existing roles
            var existingRoles = _context.UserRoles.Where(ur => ur.UserId == user.Id);
            _context.UserRoles.RemoveRange(existingRoles);

            // Add new roles
            foreach (var roleId in updateUserDto.RoleIds)
            {
                _context.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = Guid.Parse(roleId.ToString()) });
            }
        }

        await _context.SaveChangesAsync();

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
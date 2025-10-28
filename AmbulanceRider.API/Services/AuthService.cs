using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AmbulanceRider.API.DTOs;
using AmbulanceRider.API.Models;
using AmbulanceRider.API.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AmbulanceRider.API.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AuthService(
        IUserRepository userRepository,
        IConfiguration configuration,
        IEmailService emailService,
        IRefreshTokenRepository refreshTokenRepository,
        UserManager<User> userManager,
        SignInManager<User> signInManager)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _emailService = emailService;
        _refreshTokenRepository = refreshTokenRepository;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        var existingUser = await _userRepository.GetByEmailAsync(registerDto.Email);
        if (existingUser != null)
        {
            throw new ApplicationException("Email is already registered");
        }

        var user = new User
        {
            UserName = registerDto.Email,
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            Email = registerDto.Email,
            PhoneNumber = registerDto.Phone,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);
        if (!result.Succeeded)
        {
            throw new ApplicationException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        // Assign default role (you can customize this)
        await _userManager.AddToRoleAsync(user, "User");

        // Generate tokens
        var userDto = await GetUserDtoAsync(user);
        var accessToken = GenerateJwtToken(userDto);
        var refreshToken = GenerateRefreshToken();
        
        // Save refresh token
        await _refreshTokenRepository.AddAsync(new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            Expires = DateTime.UtcNow.AddDays(30),
            Created = DateTime.UtcNow,
            CreatedByIp = "127.0.0.1" // You can get this from the request context
        });

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = int.Parse(_configuration["Jwt:ExpiresInMinutes"] ?? "1440"),
            User = userDto
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _userRepository.GetByEmailAsync(loginDto.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        // Generate tokens
        var userDto = await GetUserDtoAsync(user);
        var accessToken = GenerateJwtToken(userDto);
        var refreshToken = GenerateRefreshToken();
        
        // Save refresh token
        await _refreshTokenRepository.AddAsync(new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            Expires = DateTime.UtcNow.AddDays(30),
            Created = DateTime.UtcNow,
            CreatedByIp = "127.0.0.1" // You can get this from the request context
        });

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = int.Parse(_configuration["Jwt:ExpiresInMinutes"] ?? "1440"),
            User = userDto
        };
    }

    public async Task<TokenResponseDto> RefreshTokenAsync(string refreshToken)
    {
        var token = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
        if (token == null || token.Expires < DateTime.UtcNow)
        {
            throw new SecurityTokenException("Invalid or expired refresh token");
        }

        var user = await _userManager.FindByIdAsync(token.UserId.ToString());
        if (user == null)
        {
            throw new SecurityTokenException("User not found");
        }

        // Generate new tokens
        var userDto = await GetUserDtoAsync(user);
        var newAccessToken = GenerateJwtToken(userDto);
        var newRefreshToken = GenerateRefreshToken();
        
        // Update refresh token
        token.Revoked = DateTime.UtcNow;
        token.RevokedByIp = "127.0.0.1";
        token.ReplacedByToken = newRefreshToken;
        
        await _refreshTokenRepository.UpdateAsync(token);
        
        // Add new refresh token
        await _refreshTokenRepository.AddAsync(new RefreshToken
        {
            Token = newRefreshToken,
            UserId = user.Id,
            Expires = DateTime.UtcNow.AddDays(30),
            Created = DateTime.UtcNow,
            CreatedByIp = "127.0.0.1"
        });

        return new TokenResponseDto
        {
            AccessToken = newAccessToken,
            ExpiresIn = int.Parse(_configuration["Jwt:ExpiresInMinutes"] ?? "1440")
        };
    }

    public async Task RevokeRefreshTokenAsync(string userId)
    {
        var tokens = await _refreshTokenRepository.GetByUserIdAsync(Guid.Parse(userId));
        foreach (var token in tokens.Where(t => !t.IsExpired && t.Revoked == null))
        {
            token.Revoked = DateTime.UtcNow;
            token.RevokedByIp = "127.0.0.1";
            await _refreshTokenRepository.UpdateAsync(token);
        }
    }

    public async Task<UserDto> GetUserByIdAsync(string userId)
    {
        var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));
        if (user == null)
        {
            throw new ApplicationException("User not found");
        }
        return await GetUserDtoAsync(user);
    }

    public async Task SendPasswordResetEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            // Don't reveal that the user doesn't exist
            return;
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var resetUrl = $"{_configuration["ClientApp:Url"]}/reset-password?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(email)}";
        
        await _emailService.SendEmailAsync(
            email,
            "Reset Your Password",
            $"Please reset your password by clicking here: {resetUrl}");
    }

    public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
    {
        var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
        if (user == null)
        {
            throw new ApplicationException("Invalid password reset token");
        }

        return await _userManager.ResetPasswordAsync(
            user, 
            resetPasswordDto.Token, 
            resetPasswordDto.NewPassword);
    }

    public string GenerateJwtToken(UserDto user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.Name),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString())
        };

        // Add roles as claims
        foreach (var role in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:ExpiresInMinutes"] ?? "1440")),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)),
            ValidateLifetime = false // We don't care about the expired token validation here
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        
        if (securityToken is not JwtSecurityToken jwtSecurityToken || 
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }

    public async Task<UserDto> UpdateProfileAsync(string userId, UpdateProfileDto updateProfileDto)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new ApplicationException("User not found");
        }

        // Update fields if provided
        if (!string.IsNullOrWhiteSpace(updateProfileDto.FirstName))
            user.FirstName = updateProfileDto.FirstName;
        
        if (!string.IsNullOrWhiteSpace(updateProfileDto.LastName))
            user.LastName = updateProfileDto.LastName;
        
        if (!string.IsNullOrWhiteSpace(updateProfileDto.PhoneNumber))
            user.PhoneNumber = updateProfileDto.PhoneNumber;

        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            throw new ApplicationException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        return await GetUserDtoAsync(user);
    }

    public async Task<IdentityResult> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new ApplicationException("User not found");
        }

        if (changePasswordDto.NewPassword != changePasswordDto.ConfirmPassword)
        {
            return IdentityResult.Failed(new IdentityError 
            { 
                Code = "PasswordMismatch", 
                Description = "New password and confirmation password do not match" 
            });
        }

        var result = await _userManager.ChangePasswordAsync(
            user, 
            changePasswordDto.CurrentPassword, 
            changePasswordDto.NewPassword);

        if (result.Succeeded)
        {
            user.UpdatedAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
        }

        return result;
    }

    public async Task<UserDto> UpdateProfileImageAsync(string userId, IFormFile image)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new ApplicationException("User not found");
        }

        // Validate image
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
        
        if (!allowedExtensions.Contains(extension))
        {
            throw new ApplicationException("Invalid image format. Allowed formats: jpg, jpeg, png, gif");
        }

        if (image.Length > 5 * 1024 * 1024) // 5MB limit
        {
            throw new ApplicationException("Image size must be less than 5MB");
        }

        // Delete old image if exists
        if (!string.IsNullOrEmpty(user.ImagePath) && File.Exists(user.ImagePath))
        {
            File.Delete(user.ImagePath);
        }

        // Save new image
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "profiles");
        Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = $"{userId}_{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await image.CopyToAsync(fileStream);
        }

        user.ImagePath = filePath;
        user.ImageUrl = $"/uploads/profiles/{uniqueFileName}";
        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            // Delete the uploaded file if update fails
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            throw new ApplicationException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        return await GetUserDtoAsync(user);
    }

    public async Task<UserDto> RemoveProfileImageAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new ApplicationException("User not found");
        }

        // Delete image file if exists
        if (!string.IsNullOrEmpty(user.ImagePath) && File.Exists(user.ImagePath))
        {
            File.Delete(user.ImagePath);
        }

        user.ImagePath = null;
        user.ImageUrl = null;
        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            throw new ApplicationException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        return await GetUserDtoAsync(user);
    }

    private async Task<UserDto> GetUserDtoAsync(User user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        
        return new UserDto
        {
            Id = user.Id.ToString(),
            Name = user.FirstName + " " + user.LastName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email!,
            PhoneNumber = user.PhoneNumber,
            ImagePath = user.ImagePath,
            ImageUrl = user.ImageUrl,
            Roles = roles.ToList(),
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
}

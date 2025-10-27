using AmbulanceRider.API.DTOs;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace AmbulanceRider.API.Services;

public interface IAuthService
{
    // Authentication methods
    Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    Task<TokenResponseDto> RefreshTokenAsync(string refreshToken);
    Task RevokeRefreshTokenAsync(string userId);
    
    // User
    Task<UserDto> GetUserByIdAsync(string userId);
    
    // Password
    Task SendPasswordResetEmailAsync(string email);
    Task<IdentityResult> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
    
    // Token generation
    string GenerateJwtToken(UserDto user);
    string GenerateRefreshToken();
    
    // Token validation
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}

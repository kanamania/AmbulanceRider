using System.Security.Claims;
using AmbulanceRider.API.DTOs;
using AmbulanceRider.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AmbulanceRider.API.Controllers;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ITelemetryService _telemetryService;

    public AuthController(IAuthService authService, ITelemetryService telemetryService)
    {
        _authService = authService;
        _telemetryService = telemetryService;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto registerDto)
    {
        try
        {
            var response = await _authService.RegisterAsync(registerDto);
            
            // Log telemetry
            await _telemetryService.LogTelemetryAsync(
                "Register",
                registerDto.Telemetry,
                Guid.Parse(response.User.Id),
                $"User registered: {registerDto.Email}"
            );
            
            return CreatedAtAction(nameof(GetCurrentUser), new { id = response.User.Id }, response);
        }
        catch (ApplicationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Login with email and password
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            var response = await _authService.LoginAsync(loginDto);
            
            // Log telemetry
            await _telemetryService.LogTelemetryAsync(
                "Login",
                loginDto.Telemetry,
                Guid.Parse(response.User.Id),
                $"User logged in: {loginDto.Email}"
            );
            
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            // Log failed login attempt
            await _telemetryService.LogTelemetryAsync(
                "LoginFailed",
                loginDto.Telemetry,
                null,
                $"Failed login attempt: {loginDto.Email}"
            );
            
            return Unauthorized(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Refresh access token
    /// </summary>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(TokenResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TokenResponseDto>> Refresh([FromBody] RefreshTokenDto refreshTokenDto)
    {
        try
        {
            var response = await _authService.RefreshTokenAsync(refreshTokenDto.RefreshToken);
            return Ok(response);
        }
        catch (SecurityTokenException ex)
        {
            return Unauthorized(new { message = "Invalid or expired refresh token" });
        }
    }

    /// <summary>
    /// Get current authenticated user
    /// </summary>
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var user = await _authService.GetUserByIdAsync(userId);
        if (user == null)
        {
            return Unauthorized();
        }

        return Ok(user);
    }

    /// <summary>
    /// Logout (invalidate refresh token)
    /// </summary>
    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        await _authService.RevokeRefreshTokenAsync(userId);
        return Ok(new { message = "Successfully logged out" });
    }

    /// <summary>
    /// Request password reset
    /// </summary>
    [HttpPost("forgot-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
    {
        await _authService.SendPasswordResetEmailAsync(forgotPasswordDto.Email);
        
        // Log telemetry
        await _telemetryService.LogTelemetryAsync(
            "ForgotPassword",
            forgotPasswordDto.Telemetry,
            null,
            $"Password reset requested: {forgotPasswordDto.Email}"
        );
        
        return Ok(new { message = "Password reset instructions sent to your email" });
    }

    /// <summary>
    /// Reset password with token
    /// </summary>
    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
    {
        try
        {
            var result = await _authService.ResetPasswordAsync(resetPasswordDto);
            if (!result.Succeeded)
            {
                return BadRequest(new { message = "Failed to reset password", errors = result.Errors });
            }
            
            // Log telemetry
            await _telemetryService.LogTelemetryAsync(
                "ResetPassword",
                resetPasswordDto.Telemetry,
                null,
                $"Password reset completed: {resetPasswordDto.Email}"
            );
            
            return Ok(new { message = "Password reset successfully" });
        }
        catch (ApplicationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

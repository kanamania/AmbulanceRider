using System.Security.Claims;
using AmbulanceRider.API.DTOs;
using AmbulanceRider.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;

namespace AmbulanceRider.API.Controllers;

/// <summary>
/// Authentication and authorization endpoints
/// </summary>
/// <remarks>
/// Provides endpoints for user registration, login, token refresh, and password management.
/// All authentication uses JWT Bearer tokens.
/// </remarks>
[ApiController]
[Route("api/auth")]
[Produces("application/json")]
[SwaggerTag("Manage user authentication, registration, and password operations")]
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
    /// Register a new user account
    /// </summary>
    /// <remarks>
    /// Creates a new user account with the specified role. Available roles:
    /// - **Admin**: Full system access
    /// - **Dispatcher**: Manage trips and assignments
    /// - **Driver**: Accept and complete trips
    /// - **User**: Request emergency services
    /// 
    /// Password requirements:
    /// - Minimum 8 characters
    /// - At least one uppercase letter
    /// - At least one lowercase letter
    /// - At least one digit
    /// - At least one special character
    /// </remarks>
    /// <param name="registerDto">User registration details</param>
    /// <returns>Authentication response with JWT tokens and user information</returns>
    /// <response code="201">User successfully registered</response>
    /// <response code="400">Invalid input or user already exists</response>
    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "Register a new user",
        Description = "Creates a new user account with JWT authentication",
        OperationId = "Auth_Register",
        Tags = new[] { "Authentication" }
    )]
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
    /// Authenticate user and obtain JWT tokens
    /// </summary>
    /// <remarks>
    /// Authenticates a user with email and password credentials.
    /// Returns access token (valid for 24 hours) and refresh token (valid for 7 days).
    /// 
    /// **Default Test Accounts:**
    /// - Admin: admin@ambulancerider.com / Admin@123
    /// - Dispatcher: dispatcher@ambulancerider.com / Dispatcher@123
    /// - Driver: driver@ambulancerider.com / Driver@123
    /// </remarks>
    /// <param name="loginDto">Login credentials</param>
    /// <returns>Authentication response with JWT tokens</returns>
    /// <response code="200">Login successful</response>
    /// <response code="401">Invalid credentials or account locked</response>
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [SwaggerOperation(
        Summary = "User login",
        Description = "Authenticate with email and password to receive JWT tokens",
        OperationId = "Auth_Login",
        Tags = new[] { "Authentication" }
    )]
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
    /// Refresh expired access token
    /// </summary>
    /// <remarks>
    /// Use the refresh token to obtain a new access token without requiring the user to login again.
    /// Refresh tokens are valid for 7 days and can only be used once.
    /// </remarks>
    /// <param name="refreshTokenDto">Refresh token</param>
    /// <returns>New access token and refresh token</returns>
    /// <response code="200">Token refreshed successfully</response>
    /// <response code="401">Invalid or expired refresh token</response>
    [AllowAnonymous]
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(TokenResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [SwaggerOperation(
        Summary = "Refresh access token",
        Description = "Obtain new access token using refresh token",
        OperationId = "Auth_RefreshToken",
        Tags = new[] { "Authentication" }
    )]
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
    /// Get current authenticated user profile
    /// </summary>
    /// <remarks>
    /// Retrieves the profile information of the currently authenticated user.
    /// Requires valid JWT Bearer token in Authorization header.
    /// </remarks>
    /// <returns>Current user profile information</returns>
    /// <response code="200">User profile retrieved successfully</response>
    /// <response code="401">Not authenticated or invalid token</response>
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [SwaggerOperation(
        Summary = "Get current user",
        Description = "Retrieve authenticated user's profile information",
        OperationId = "Auth_GetCurrentUser",
        Tags = new[] { "Authentication" }
    )]
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
    /// Logout and invalidate refresh token
    /// </summary>
    /// <remarks>
    /// Invalidates the user's refresh token, preventing it from being used to obtain new access tokens.
    /// The current access token will remain valid until it expires.
    /// </remarks>
    /// <returns>Logout confirmation</returns>
    /// <response code="200">Logout successful</response>
    /// <response code="401">Not authenticated</response>
    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [SwaggerOperation(
        Summary = "User logout",
        Description = "Invalidate refresh token and logout user",
        OperationId = "Auth_Logout",
        Tags = new[] { "Authentication" }
    )]
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
    /// Request password reset email
    /// </summary>
    /// <remarks>
    /// Sends a password reset email to the specified email address if it exists in the system.
    /// For security, always returns success even if email doesn't exist.
    /// </remarks>
    /// <param name="forgotPasswordDto">Email address for password reset</param>
    /// <returns>Success message</returns>
    /// <response code="200">Reset email sent (if email exists)</response>
    [AllowAnonymous]
    [HttpPost("forgot-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Forgot password",
        Description = "Request password reset email",
        OperationId = "Auth_ForgotPassword",
        Tags = new[] { "Authentication" }
    )]
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
    /// Reset password using reset token
    /// </summary>
    /// <remarks>
    /// Resets the user's password using the token received via email.
    /// The token is valid for 24 hours.
    /// </remarks>
    /// <param name="resetPasswordDto">Reset token and new password</param>
    /// <returns>Success or error message</returns>
    /// <response code="200">Password reset successfully</response>
    /// <response code="400">Invalid token or password requirements not met</response>
    [AllowAnonymous]
    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "Reset password",
        Description = "Reset user password with reset token",
        OperationId = "Auth_ResetPassword",
        Tags = new[] { "Authentication" }
    )]
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

    /// <summary>
    /// Update current user's profile
    /// </summary>
    /// <remarks>
    /// Updates the authenticated user's profile information including first name, last name, and phone number.
    /// Only the fields provided will be updated. Empty or null fields will be ignored.
    /// </remarks>
    /// <param name="updateProfileDto">Profile information to update</param>
    /// <returns>Updated user profile</returns>
    /// <response code="200">Profile updated successfully</response>
    /// <response code="400">Invalid input data</response>
    /// <response code="401">Not authenticated</response>
    [Authorize]
    [HttpPut("profile")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [SwaggerOperation(
        Summary = "Update profile",
        Description = "Update current user's profile information",
        OperationId = "Auth_UpdateProfile",
        Tags = new[] { "Authentication" }
    )]
    public async Task<ActionResult<UserDto>> UpdateProfile([FromBody] UpdateProfileDto updateProfileDto)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var updatedUser = await _authService.UpdateProfileAsync(userId, updateProfileDto);
            return Ok(updatedUser);
        }
        catch (ApplicationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Change current user's password
    /// </summary>
    /// <remarks>
    /// Changes the authenticated user's password. Requires the current password for verification.
    /// 
    /// Password requirements:
    /// - Minimum 8 characters
    /// - At least one uppercase letter
    /// - At least one lowercase letter
    /// - At least one digit
    /// - At least one special character
    /// </remarks>
    /// <param name="changePasswordDto">Current and new password</param>
    /// <returns>Success or error message</returns>
    /// <response code="200">Password changed successfully</response>
    /// <response code="400">Invalid current password or new password doesn't meet requirements</response>
    /// <response code="401">Not authenticated</response>
    [Authorize]
    [HttpPost("change-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [SwaggerOperation(
        Summary = "Change password",
        Description = "Change current user's password",
        OperationId = "Auth_ChangePassword",
        Tags = new[] { "Authentication" }
    )]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _authService.ChangePasswordAsync(userId, changePasswordDto);
            if (!result.Succeeded)
            {
                return BadRequest(new { message = "Failed to change password", errors = result.Errors });
            }

            return Ok(new { message = "Password changed successfully" });
        }
        catch (ApplicationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Update current user's profile image
    /// </summary>
    /// <remarks>
    /// Uploads and sets a new profile image for the authenticated user.
    /// 
    /// Image requirements:
    /// - Allowed formats: JPG, JPEG, PNG, GIF
    /// - Maximum size: 5MB
    /// - Previous image will be automatically deleted
    /// </remarks>
    /// <param name="request">Image file upload request</param>
    /// <returns>Updated user profile with new image URL</returns>
    /// <response code="200">Image uploaded successfully</response>
    /// <response code="400">Invalid image format or size</response>
    /// <response code="401">Not authenticated</response>
    [Authorize]
    [HttpPost("profile/image")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [SwaggerOperation(
        Summary = "Upload profile image",
        Description = "Upload new profile image for current user",
        OperationId = "Auth_UploadProfileImage",
        Tags = new[] { "Authentication" }
    )]
    public async Task<ActionResult<UserDto>> UploadProfileImage([FromForm] UpdateProfileImageDto request)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            if (request?.Image == null || request.Image.Length == 0)
            {
                return BadRequest(new { message = "No image file provided" });
            }

            var updatedUser = await _authService.UpdateProfileImageAsync(userId, request.Image);
            return Ok(updatedUser);
        }
        catch (ApplicationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Remove current user's profile image
    /// </summary>
    /// <remarks>
    /// Removes the profile image for the authenticated user.
    /// The image file will be deleted from the server.
    /// </remarks>
    /// <returns>Updated user profile without image</returns>
    /// <response code="200">Image removed successfully</response>
    /// <response code="401">Not authenticated</response>
    [Authorize]
    [HttpDelete("profile/image")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [SwaggerOperation(
        Summary = "Remove profile image",
        Description = "Remove profile image for current user",
        OperationId = "Auth_RemoveProfileImage",
        Tags = new[] { "Authentication" }
    )]
    public async Task<ActionResult<UserDto>> RemoveProfileImage()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var updatedUser = await _authService.RemoveProfileImageAsync(userId);
            return Ok(updatedUser);
        }
        catch (ApplicationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

using AmbulanceRider.API.DTOs;
using AmbulanceRider.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;

namespace AmbulanceRider.API.Controllers;

/// <summary>
/// User management endpoints
/// </summary>
/// <remarks>
/// Manage system users including admins, dispatchers, drivers, and regular users.
/// Supports user CRUD operations, role management, and profile updates.
/// </remarks>
[Authorize]
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[SwaggerTag("Manage users, roles, and permissions")]
public class UsersController(IUserService userService, IConfiguration configuration) : ControllerBase
{
    /// <summary>
    /// Get all users
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Dispatcher")]
    [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
    {
        var users = await userService.GetAllUsersAsync();
        return Ok(users);
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetById(Guid id)
    {
        var user = await userService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound();
        
        return Ok(user);
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserDto createUserDto)
    {
        try
        {
            var user = await userService.CreateUserAsync(createUserDto);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing user
    /// </summary>
    /// <remarks>
    /// To update user image, provide ImagePath and ImageUrl in the request body.
    /// Set RemoveImage to true to remove the existing image.
    /// </remarks>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserDto>> Update(Guid id, [FromBody] UpdateUserDto updateUserDto)
    {
        try
        {
            var existingUser = await userService.GetUserByIdAsync(id);
            if (existingUser == null)
                return NotFound();

            var user = await userService.UpdateUserAsync(id, updateUserDto);
            return Ok(user);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Delete a user (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await userService.DeleteUserAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}

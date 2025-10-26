using AmbulanceRider.API.DTOs;
using AmbulanceRider.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AmbulanceRider.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LocationsController(ILocationService locationService, IConfiguration configuration)
    : ControllerBase
{
    /// <summary>
    /// Get all locations
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<LocationDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<LocationDto>>> GetAll()
    {
        var locations = await locationService.GetAllLocationsAsync();
        return Ok(locations);
    }

    /// <summary>
    /// Get location by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(LocationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LocationDto>> GetById(int id)
    {
        var location = await locationService.GetLocationByIdAsync(id);
        if (location == null)
            return NotFound();
        
        return Ok(location);
    }

    /// <summary>
    /// Create a new location
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Dispatcher")]
    [ProducesResponseType(typeof(LocationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LocationDto>> Create([FromForm] CreateLocationDto createLocationDto)
    {
        try
        {
            if (createLocationDto.Image != null)
            {
                var baseUrl = configuration["BaseUrl"] ?? $"{Request.Scheme}://{Request.Host}";
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "locations");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(createLocationDto.Image.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await createLocationDto.Image.CopyToAsync(stream);
                }

                createLocationDto.ImagePath = $"uploads/locations/{uniqueFileName}";
                createLocationDto.ImageUrl = $"{baseUrl}/uploads/locations/{uniqueFileName}";
            }

            var location = await locationService.CreateLocationAsync(createLocationDto);
            return CreatedAtAction(nameof(GetById), new { id = location.Id }, location);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing location
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Dispatcher")]
    [ProducesResponseType(typeof(LocationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LocationDto>> Update(int id, [FromForm] UpdateLocationDto updateLocationDto)
    {
        try
        {
            var existingLocation = await locationService.GetLocationByIdAsync(id);
            if (existingLocation == null)
                return NotFound();

            var baseUrl = configuration["BaseUrl"] ?? $"{Request.Scheme}://{Request.Host}";
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "locations");
            
            // Handle image upload if a new image is provided
            if (updateLocationDto.Image != null)
            {
                // Delete old image if it exists
                if (!string.IsNullOrEmpty(existingLocation.ImagePath))
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingLocation.ImagePath);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // Save new image
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(updateLocationDto.Image.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await updateLocationDto.Image.CopyToAsync(stream);
                }

                updateLocationDto = new UpdateLocationDto
                {
                    Name = updateLocationDto.Name,
                    Image = updateLocationDto.Image,
                    RemoveImage = updateLocationDto.RemoveImage,
                    ImagePath = $"uploads/locations/{uniqueFileName}",
                    ImageUrl = $"{baseUrl}/uploads/locations/{uniqueFileName}"
                };
            }
            // Handle image removal if requested
            else if (updateLocationDto.RemoveImage && !string.IsNullOrEmpty(existingLocation.ImagePath))
            {
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingLocation.ImagePath);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
                updateLocationDto = new UpdateLocationDto
                {
                    ImagePath = null,
                    ImageUrl = null
                };
            }

            var location = await locationService.UpdateLocationAsync(id, updateLocationDto);
            return Ok(location);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Delete a location
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await locationService.DeleteLocationAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace AmbulanceRider.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FileUploadController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;
    private readonly IConfiguration _configuration;
    private const long MaxFileSize = 5 * 1024 * 1024; // 5MB
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

    public FileUploadController(IWebHostEnvironment environment, IConfiguration configuration)
    {
        _environment = environment;
        _configuration = configuration;
    }

    [HttpPost("vehicle-image")]
    [Authorize(Roles = "Admin,Dispatcher")]
    public async Task<IActionResult> UploadVehicleImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        if (file.Length > MaxFileSize)
            return BadRequest("File size exceeds the 5MB limit.");

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (string.IsNullOrEmpty(extension) || !AllowedExtensions.Contains(extension))
            return BadRequest("Invalid file type. Only JPG, JPEG, PNG, and GIF are allowed.");

        var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "vehicles");
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var baseUrl = _configuration["BaseUrl"] ?? $"{Request.Scheme}://{Request.Host}";
        var fileUrl = $"{baseUrl}/uploads/vehicles/{uniqueFileName}";

        return Ok(new { filePath = $"uploads/vehicles/{uniqueFileName}", fileUrl });
    }

    [HttpDelete("vehicle-image")]
    [Authorize(Roles = "Admin,Dispatcher")]
    public IActionResult DeleteVehicleImage([FromQuery] string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            return BadRequest("File path is required.");

        var fullPath = Path.Combine(_environment.WebRootPath, filePath);
        
        if (!System.IO.File.Exists(fullPath))
            return NotFound("File not found.");

        try
        {
            System.IO.File.Delete(fullPath);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error deleting file: {ex.Message}");
        }
    }
}

using AmbulanceRider.API.Data;
using AmbulanceRider.API.DTOs;
using AmbulanceRider.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;

namespace AmbulanceRider.API.Controllers;

/// <summary>
/// Vehicle fleet management endpoints
/// </summary>
/// <remarks>
/// Manage ambulance fleet including registration, status tracking, and availability.
/// Supports vehicle types, capacity management, and real-time status updates.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
[SwaggerTag("Manage ambulance fleet and vehicle information")]
public class VehiclesController(IVehicleService vehicleService, ApplicationDbContext context, IConfiguration configuration) : ControllerBase
{
    /// <summary>
    /// Get all vehicles
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<VehicleDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<VehicleDto>>> GetAll()
    {
        var vehicles = await vehicleService.GetAllVehiclesAsync();
        return Ok(vehicles);
    }

    /// <summary>
    /// Get all vehicle types
    /// </summary>
    [HttpGet("types")]
    [ProducesResponseType(typeof(IEnumerable<VehicleTypeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<VehicleTypeDto>>> GetVehicleTypes()
    {
        var types = await context.VehicleTypes
            .Select(vt => new VehicleTypeDto { Id = vt.Id, Name = vt.Name })
            .ToListAsync();
        return Ok(types);
    }

    /// <summary>
    /// Get vehicle by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(VehicleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VehicleDto>> GetById(int id)
    {
        var vehicle = await vehicleService.GetVehicleByIdAsync(id);
        if (vehicle == null)
            return NotFound();
        
        return Ok(vehicle);
    }

    /// <summary>
    /// Create a new vehicle
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Dispatcher")]
    [ProducesResponseType(typeof(VehicleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<VehicleDto>> Create([FromBody] CreateVehicleDto createVehicleDto)
    {
        var vehicle = await vehicleService.CreateVehicleAsync(createVehicleDto);
        return CreatedAtAction(nameof(GetById), new { id = vehicle.Id }, vehicle);
    }

    /// <summary>
    /// Update an existing vehicle
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Dispatcher")]
    [ProducesResponseType(typeof(VehicleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<VehicleDto>> Update(int id, [FromBody] UpdateVehicleDto updateVehicleDto)
    {
        try
        {
            var existingVehicle = await vehicleService.GetVehicleByIdAsync(id);
            if (existingVehicle == null)
                return NotFound();

            var vehicle = await vehicleService.UpdateVehicleAsync(id, updateVehicleDto);
            return Ok(vehicle);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Delete a vehicle (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await vehicleService.DeleteVehicleAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}

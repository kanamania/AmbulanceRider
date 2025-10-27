using AmbulanceRider.API.DTOs;
using AmbulanceRider.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AmbulanceRider.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TripTypesController : ControllerBase
{
    private readonly ITripTypeService _tripTypeService;

    public TripTypesController(ITripTypeService tripTypeService)
    {
        _tripTypeService = tripTypeService;
    }

    /// <summary>
    /// Get all trip types
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TripTypeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TripTypeDto>>> GetAll()
    {
        var tripTypes = await _tripTypeService.GetAllTripTypesAsync();
        return Ok(tripTypes);
    }

    /// <summary>
    /// Get active trip types only
    /// </summary>
    [HttpGet("active")]
    [ProducesResponseType(typeof(IEnumerable<TripTypeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TripTypeDto>>> GetActive()
    {
        var tripTypes = await _tripTypeService.GetActiveTripTypesAsync();
        return Ok(tripTypes);
    }

    /// <summary>
    /// Get trip type by ID with attributes
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TripTypeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TripTypeDto>> GetById(int id)
    {
        var tripType = await _tripTypeService.GetTripTypeByIdAsync(id);
        if (tripType == null)
            return NotFound();

        return Ok(tripType);
    }

    /// <summary>
    /// Create a new trip type
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Dispatcher")]
    [ProducesResponseType(typeof(TripTypeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TripTypeDto>> Create([FromBody] CreateTripTypeDto dto)
    {
        var tripType = await _tripTypeService.CreateTripTypeAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = tripType.Id }, tripType);
    }

    /// <summary>
    /// Update an existing trip type
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Dispatcher")]
    [ProducesResponseType(typeof(TripTypeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TripTypeDto>> Update(int id, [FromBody] UpdateTripTypeDto dto)
    {
        try
        {
            var tripType = await _tripTypeService.UpdateTripTypeAsync(id, dto);
            return Ok(tripType);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Delete a trip type
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _tripTypeService.DeleteTripTypeAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Create a new attribute for a trip type
    /// </summary>
    [HttpPost("attributes")]
    [Authorize(Roles = "Admin,Dispatcher")]
    [ProducesResponseType(typeof(TripTypeAttributeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TripTypeAttributeDto>> CreateAttribute([FromBody] CreateTripTypeAttributeDto dto)
    {
        var attribute = await _tripTypeService.CreateAttributeAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = dto.TripTypeId }, attribute);
    }

    /// <summary>
    /// Update an existing attribute
    /// </summary>
    [HttpPut("attributes/{id}")]
    [Authorize(Roles = "Admin,Dispatcher")]
    [ProducesResponseType(typeof(TripTypeAttributeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TripTypeAttributeDto>> UpdateAttribute(int id, [FromBody] UpdateTripTypeAttributeDto dto)
    {
        try
        {
            var attribute = await _tripTypeService.UpdateAttributeAsync(id, dto);
            return Ok(attribute);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Delete an attribute
    /// </summary>
    [HttpDelete("attributes/{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAttribute(int id)
    {
        try
        {
            await _tripTypeService.DeleteAttributeAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}

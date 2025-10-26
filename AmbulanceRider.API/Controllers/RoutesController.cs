using AmbulanceRider.API.DTOs;
using AmbulanceRider.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AmbulanceRider.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RoutesController(IRouteService routeService) : ControllerBase
{
    /// <summary>
    /// Get all routes
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<RouteDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<RouteDto>>> GetAll()
    {
        var routes = await routeService.GetAllRoutesAsync();
        return Ok(routes);
    }

    /// <summary>
    /// Get route by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(RouteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RouteDto>> GetById(int id)
    {
        var route = await routeService.GetRouteByIdAsync(id);
        if (route == null)
            return NotFound();
        
        return Ok(route);
    }

    /// <summary>
    /// Create a new route
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Dispatcher")]
    [ProducesResponseType(typeof(RouteDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<RouteDto>> Create([FromBody] CreateRouteDto createRouteDto)
    {
        var route = await routeService.CreateRouteAsync(createRouteDto);
        return CreatedAtAction(nameof(GetById), new { id = route.Id }, route);
    }

    /// <summary>
    /// Update an existing route
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Dispatcher")]
    [ProducesResponseType(typeof(RouteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RouteDto>> Update(int id, [FromBody] UpdateRouteDto updateRouteDto)
    {
        try
        {
            var route = await routeService.UpdateRouteAsync(id, updateRouteDto);
            return Ok(route);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Delete a route (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await routeService.DeleteRouteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}

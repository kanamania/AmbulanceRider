using AmbulanceRider.API.DTOs;
using AmbulanceRider.API.Models;
using AmbulanceRider.API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AmbulanceRider.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RegionsController : ControllerBase
{
    private readonly IRegionRepository _regionRepo;

    public RegionsController(IRegionRepository regionRepo)
    {
        _regionRepo = regionRepo;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var regions = await _regionRepo.GetAllAsync();
        var regionDtos = regions.Select(r => new RegionDto
        {
            Id = r.Id,
            Name = r.Name,
            Description = r.Description,
            Code = r.Code,
            IsDefault = r.IsDefault,
            IsActive = r.IsActive,
            CreatedAt = r.CreatedAt
        });
        return Ok(regionDtos);
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
    {
        var regions = await _regionRepo.GetActiveRegionsAsync();
        var regionDtos = regions.Select(r => new RegionDto
        {
            Id = r.Id,
            Name = r.Name,
            Description = r.Description,
            Code = r.Code,
            IsDefault = r.IsDefault,
            IsActive = r.IsActive,
            CreatedAt = r.CreatedAt
        });
        return Ok(regionDtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var region = await _regionRepo.GetByIdAsync(id);
        if (region == null)
            return NotFound();

        var regionDto = new RegionDto
        {
            Id = region.Id,
            Name = region.Name,
            Description = region.Description,
            Code = region.Code,
            IsDefault = region.IsDefault,
            IsActive = region.IsActive,
            CreatedAt = region.CreatedAt
        };
        return Ok(regionDto);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateRegionDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Invalid user token" });
        }

        var region = new Region
        {
            Name = dto.Name,
            Description = dto.Description,
            Code = dto.Code,
            IsActive = dto.IsActive,
            IsDefault = false,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

        await _regionRepo.AddAsync(region);
        
        var regionDto = new RegionDto
        {
            Id = region.Id,
            Name = region.Name,
            Description = region.Description,
            Code = region.Code,
            IsDefault = region.IsDefault,
            IsActive = region.IsActive,
            CreatedAt = region.CreatedAt
        };
        
        return CreatedAtAction(nameof(GetById), new { id = region.Id }, regionDto);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRegionDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Invalid user token" });
        }

        var region = await _regionRepo.GetByIdAsync(id);
        if (region == null)
            return NotFound();

        if (dto.Name != null) region.Name = dto.Name;
        if (dto.Description != null) region.Description = dto.Description;
        if (dto.Code != null) region.Code = dto.Code;
        if (dto.IsActive.HasValue) region.IsActive = dto.IsActive.Value;

        region.UpdatedAt = DateTime.UtcNow;
        region.UpdatedBy = userId;

        await _regionRepo.UpdateAsync(region);
        
        var regionDto = new RegionDto
        {
            Id = region.Id,
            Name = region.Name,
            Description = region.Description,
            Code = region.Code,
            IsDefault = region.IsDefault,
            IsActive = region.IsActive,
            CreatedAt = region.CreatedAt
        };
        
        return Ok(regionDto);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var region = await _regionRepo.GetByIdAsync(id);
        if (region == null)
            return NotFound();

        // Prevent deletion of default region
        if (region.IsDefault)
            return BadRequest(new { message = "Cannot delete the default region" });

        await _regionRepo.DeleteAsync(region.Id);
        return NoContent();
    }
}

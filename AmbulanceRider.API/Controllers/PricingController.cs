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
public class PricingController : ControllerBase
{
    private readonly IPricingMatrixRepository _pricingRepo;

    public PricingController(IPricingMatrixRepository pricingRepo)
    {
        _pricingRepo = pricingRepo;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var prices = await _pricingRepo.GetAllAsync();
        
        var dtos = prices.Select(p => new PricingMatrixDto
        {
            Id = p.Id,
            Name = p.Region != null ? $"{p.Name} - {p.Region.Name}" : p.Name,
            Description = p.Description,
            MinWeight = p.MinWeight,
            MaxWeight = p.MaxWeight,
            MinHeight = p.MinHeight,
            MaxHeight = p.MaxHeight,
            MinLength = p.MinLength,
            MaxLength = p.MaxLength,
            MinWidth = p.MinWidth,
            MaxWidth = p.MaxWidth,
            BasePrice = p.BasePrice,
            TaxRate = p.TaxRate,
            TotalPrice = p.TotalPrice,
            RegionId = p.RegionId,
            RegionName = p.Region?.Name,
            IsDefault = p.IsDefault,
            CompanyId = p.CompanyId,
            CompanyName = p.Company?.Name,
            VehicleTypeId = p.VehicleTypeId,
            VehicleTypeName = p.VehicleType?.Name,
            TripTypeId = p.TripTypeId,
            TripTypeName = p.TripType?.Name
        });
        
        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var price = await _pricingRepo.GetByIdAsync(id);
        if (price == null)
            return NotFound();

        return Ok(price);
    }

    [HttpGet("by-dimensions")]
    public async Task<IActionResult> GetByDimensions(
        [FromQuery] decimal weight, 
        [FromQuery] decimal height,
        [FromQuery] decimal length,
        [FromQuery] decimal width)
    {
        var prices = await _pricingRepo.GetByDimensionsAsync(weight, height, length, width);
        return Ok(prices);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreatePricingMatrixDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Invalid user token" });
        }

        var pricingMatrix = new PricingMatrix
        {
            Name = dto.Name,
            Description = dto.Description,
            MinWeight = dto.MinWeight,
            MaxWeight = dto.MaxWeight,
            MinHeight = dto.MinHeight,
            MaxHeight = dto.MaxHeight,
            MinLength = dto.MinLength,
            MaxLength = dto.MaxLength,
            MinWidth = dto.MinWidth,
            MaxWidth = dto.MaxWidth,
            BasePrice = dto.BasePrice,
            TaxRate = dto.TaxRate,
            RegionId = dto.RegionId,
            IsDefault = dto.IsDefault,
            CompanyId = dto.CompanyId,
            VehicleTypeId = dto.VehicleTypeId,
            TripTypeId = dto.TripTypeId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

        await _pricingRepo.AddAsync(pricingMatrix);
        var created = await _pricingRepo.GetByIdAsync(pricingMatrix.Id);
        return CreatedAtAction(nameof(GetById), new { id = created!.Id }, created);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePricingMatrixDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Invalid user token" });
        }

        var pricingMatrix = await _pricingRepo.GetByIdAsync(id);
        if (pricingMatrix == null)
            return NotFound();

        if (dto.Name != null) pricingMatrix.Name = dto.Name;
        if (dto.Description != null) pricingMatrix.Description = dto.Description;
        if (dto.MinWeight.HasValue) pricingMatrix.MinWeight = dto.MinWeight.Value;
        if (dto.MaxWeight.HasValue) pricingMatrix.MaxWeight = dto.MaxWeight.Value;
        if (dto.MinHeight.HasValue) pricingMatrix.MinHeight = dto.MinHeight.Value;
        if (dto.MaxHeight.HasValue) pricingMatrix.MaxHeight = dto.MaxHeight.Value;
        if (dto.MinLength.HasValue) pricingMatrix.MinLength = dto.MinLength.Value;
        if (dto.MaxLength.HasValue) pricingMatrix.MaxLength = dto.MaxLength.Value;
        if (dto.MinWidth.HasValue) pricingMatrix.MinWidth = dto.MinWidth.Value;
        if (dto.MaxWidth.HasValue) pricingMatrix.MaxWidth = dto.MaxWidth.Value;
        if (dto.BasePrice.HasValue) pricingMatrix.BasePrice = dto.BasePrice.Value;
        if (dto.TaxRate.HasValue) pricingMatrix.TaxRate = dto.TaxRate.Value;
        if (dto.RegionId.HasValue) pricingMatrix.RegionId = dto.RegionId;
        if (dto.CompanyId.HasValue) pricingMatrix.CompanyId = dto.CompanyId;
        if (dto.VehicleTypeId.HasValue) pricingMatrix.VehicleTypeId = dto.VehicleTypeId;
        if (dto.TripTypeId.HasValue) pricingMatrix.TripTypeId = dto.TripTypeId;

        pricingMatrix.UpdatedAt = DateTime.UtcNow;
        pricingMatrix.UpdatedBy = userId;

        await _pricingRepo.UpdateAsync(pricingMatrix);
        var updated = await _pricingRepo.GetByIdAsync(id);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var pricingMatrix = await _pricingRepo.GetByIdAsync(id);
        if (pricingMatrix == null)
            return NotFound();

        // Prevent deletion of default pricing matrix
        if (pricingMatrix.IsDefault)
            return BadRequest(new { message = "Cannot delete the default pricing matrix" });

        await _pricingRepo.DeleteAsync(pricingMatrix.Id);
        return NoContent();
    }
}

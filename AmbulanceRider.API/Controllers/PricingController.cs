using AmbulanceRider.API.Models;
using AmbulanceRider.API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AmbulanceRider.API.Controllers;

[ApiController]
[Route("api/[controller]")]
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
        return Ok(prices);
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
}

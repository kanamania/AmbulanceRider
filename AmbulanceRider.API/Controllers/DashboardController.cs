using AmbulanceRider.API.DTOs;
using AmbulanceRider.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AmbulanceRider.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly CompanyStatsService _statsService;

    public DashboardController(CompanyStatsService statsService)
    {
        _statsService = statsService;
    }

    [HttpGet("company-stats/{companyId}")]
    public async Task<IActionResult> GetCompanyStats(int companyId)
    {
        var stats = await _statsService.GetCompanyStatsAsync(companyId);
        return Ok(stats);
    }
}

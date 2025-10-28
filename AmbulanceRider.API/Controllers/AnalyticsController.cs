using AmbulanceRider.API.Data;
using AmbulanceRider.API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AmbulanceRider.API.Controllers;

/// <summary>
/// Controller for analytics and reporting
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AnalyticsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AnalyticsController> _logger;

    public AnalyticsController(ApplicationDbContext context, ILogger<AnalyticsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get dashboard statistics
    /// </summary>
    [HttpGet("dashboard")]
    public async Task<ActionResult<DashboardStatsDto>> GetDashboardStats(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        try
        {
            var start = startDate ?? DateTime.UtcNow.AddMonths(-1);
            var end = endDate ?? DateTime.UtcNow;

            var stats = new DashboardStatsDto
            {
                TotalTrips = await _context.Trips.CountAsync(t => t.CreatedAt >= start && t.CreatedAt <= end),
                PendingTrips = await _context.Trips.CountAsync(t => t.Status == Models.TripStatus.Pending && t.CreatedAt >= start && t.CreatedAt <= end),
                ApprovedTrips = await _context.Trips.CountAsync(t => t.Status == Models.TripStatus.Approved && t.CreatedAt >= start && t.CreatedAt <= end),
                InProgressTrips = await _context.Trips.CountAsync(t => t.Status == Models.TripStatus.InProgress && t.CreatedAt >= start && t.CreatedAt <= end),
                CompletedTrips = await _context.Trips.CountAsync(t => t.Status == Models.TripStatus.Completed && t.CreatedAt >= start && t.CreatedAt <= end),
                CancelledTrips = await _context.Trips.CountAsync(t => t.Status == Models.TripStatus.Cancelled && t.CreatedAt >= start && t.CreatedAt <= end),
                RejectedTrips = await _context.Trips.CountAsync(t => t.Status == Models.TripStatus.Rejected && t.CreatedAt >= start && t.CreatedAt <= end),
                TotalVehicles = await _context.Vehicles.CountAsync(),
                ActiveVehicles = await _context.Trips.Where(t => t.Status == Models.TripStatus.InProgress).Select(t => t.VehicleId).Distinct().CountAsync(),
                TotalDrivers = await _context.Users.CountAsync(u => u.UserRoles.Any(ur => ur.Role != null && ur.Role.Name == "Driver")),
                ActiveDrivers = await _context.Trips.Where(t => t.Status == Models.TripStatus.InProgress).Select(t => t.DriverId).Distinct().CountAsync(),
                TotalUsers = await _context.Users.CountAsync(),
                StartDate = start,
                EndDate = end
            };

            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dashboard stats");
            return StatusCode(500, "An error occurred while retrieving dashboard statistics");
        }
    }

    /// <summary>
    /// Get trip statistics by status
    /// </summary>
    [HttpGet("trips/by-status")]
    public async Task<ActionResult<IEnumerable<TripStatusStatsDto>>> GetTripsByStatus(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        try
        {
            var start = startDate ?? DateTime.UtcNow.AddMonths(-1);
            var end = endDate ?? DateTime.UtcNow;

            var stats = await _context.Trips
                .Where(t => t.CreatedAt >= start && t.CreatedAt <= end)
                .GroupBy(t => t.Status)
                .Select(g => new TripStatusStatsDto
                {
                    Status = g.Key.ToString(),
                    Count = g.Count(),
                    Percentage = 0 // Will calculate after
                })
                .ToListAsync();

            var total = stats.Sum(s => s.Count);
            foreach (var stat in stats)
            {
                stat.Percentage = total > 0 ? (double)stat.Count / total * 100 : 0;
            }

            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting trips by status");
            return StatusCode(500, "An error occurred while retrieving trip statistics");
        }
    }

    /// <summary>
    /// Get trip statistics by date
    /// </summary>
    [HttpGet("trips/by-date")]
    public async Task<ActionResult<IEnumerable<TripDateStatsDto>>> GetTripsByDate(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] string groupBy = "day") // day, week, month
    {
        try
        {
            var start = startDate ?? DateTime.UtcNow.AddMonths(-1);
            var end = endDate ?? DateTime.UtcNow;

            var trips = await _context.Trips
                .Where(t => t.CreatedAt >= start && t.CreatedAt <= end)
                .ToListAsync();

            IEnumerable<TripDateStatsDto> stats;

            switch (groupBy.ToLower())
            {
                case "week":
                    stats = trips
                        .GroupBy(t => new { Year = t.CreatedAt.Year, Week = GetWeekOfYear(t.CreatedAt) })
                        .Select(g => new TripDateStatsDto
                        {
                            Date = GetDateFromWeek(g.Key.Year, g.Key.Week),
                            Count = g.Count(),
                            Label = $"Week {g.Key.Week}, {g.Key.Year}"
                        })
                        .OrderBy(s => s.Date);
                    break;
                case "month":
                    stats = trips
                        .GroupBy(t => new { t.CreatedAt.Year, t.CreatedAt.Month })
                        .Select(g => new TripDateStatsDto
                        {
                            Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                            Count = g.Count(),
                            Label = $"{new DateTime(g.Key.Year, g.Key.Month, 1):MMMM yyyy}"
                        })
                        .OrderBy(s => s.Date);
                    break;
                default: // day
                    stats = trips
                        .GroupBy(t => t.CreatedAt.Date)
                        .Select(g => new TripDateStatsDto
                        {
                            Date = g.Key,
                            Count = g.Count(),
                            Label = g.Key.ToString("yyyy-MM-dd")
                        })
                        .OrderBy(s => s.Date);
                    break;
            }

            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting trips by date");
            return StatusCode(500, "An error occurred while retrieving trip statistics");
        }
    }

    /// <summary>
    /// Get vehicle utilization statistics
    /// </summary>
    [HttpGet("vehicles/utilization")]
    public async Task<ActionResult<IEnumerable<VehicleUtilizationDto>>> GetVehicleUtilization(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        try
        {
            var start = startDate ?? DateTime.UtcNow.AddMonths(-1);
            var end = endDate ?? DateTime.UtcNow;

            var stats = await _context.Vehicles
                .Select(v => new VehicleUtilizationDto
                {
                    VehicleId = v.Id,
                    VehicleName = v.Name,
                    PlateNumber = v.PlateNumber,
                    TotalTrips = v.Trips.Count(t => t.CreatedAt >= start && t.CreatedAt <= end),
                    CompletedTrips = v.Trips.Count(t => t.Status == Models.TripStatus.Completed && t.CreatedAt >= start && t.CreatedAt <= end),
                    InProgressTrips = v.Trips.Count(t => t.Status == Models.TripStatus.InProgress),
                    UtilizationRate = v.Trips.Any() ? (double)v.Trips.Count(t => t.Status == Models.TripStatus.Completed && t.CreatedAt >= start && t.CreatedAt <= end) / v.Trips.Count(t => t.CreatedAt >= start && t.CreatedAt <= end) * 100 : 0
                })
                .OrderByDescending(v => v.TotalTrips)
                .ToListAsync();

            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting vehicle utilization");
            return StatusCode(500, "An error occurred while retrieving vehicle utilization");
        }
    }

    /// <summary>
    /// Get driver performance statistics
    /// </summary>
    [HttpGet("drivers/performance")]
    public async Task<ActionResult<IEnumerable<DriverPerformanceDto>>> GetDriverPerformance(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        try
        {
            var start = startDate ?? DateTime.UtcNow.AddMonths(-1);
            var end = endDate ?? DateTime.UtcNow;

            var stats = await _context.Users
                .Where(u => u.UserRoles.Any(ur => ur.Role != null && ur.Role.Name == "Driver"))
                .Select(u => new DriverPerformanceDto
                {
                    DriverId = u.Id,
                    DriverName = u.FullName,
                    Email = u.Email ?? "",
                    TotalTrips = u.DriverTrips.Count(t => t.CreatedAt >= start && t.CreatedAt <= end),
                    CompletedTrips = u.DriverTrips.Count(t => t.Status == Models.TripStatus.Completed && t.CreatedAt >= start && t.CreatedAt <= end),
                    InProgressTrips = u.DriverTrips.Count(t => t.Status == Models.TripStatus.InProgress),
                    CancelledTrips = u.DriverTrips.Count(t => t.Status == Models.TripStatus.Cancelled && t.CreatedAt >= start && t.CreatedAt <= end),
                    CompletionRate = u.DriverTrips.Any(t => t.CreatedAt >= start && t.CreatedAt <= end) ? (double)u.DriverTrips.Count(t => t.Status == Models.TripStatus.Completed && t.CreatedAt >= start && t.CreatedAt <= end) / u.DriverTrips.Count(t => t.CreatedAt >= start && t.CreatedAt <= end) * 100 : 0
                })
                .OrderByDescending(d => d.TotalTrips)
                .ToListAsync();

            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting driver performance");
            return StatusCode(500, "An error occurred while retrieving driver performance");
        }
    }

    private static int GetWeekOfYear(DateTime date)
    {
        var culture = System.Globalization.CultureInfo.CurrentCulture;
        return culture.Calendar.GetWeekOfYear(date, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday);
    }

    private static DateTime GetDateFromWeek(int year, int week)
    {
        var jan1 = new DateTime(year, 1, 1);
        var daysOffset = DayOfWeek.Monday - jan1.DayOfWeek;
        var firstMonday = jan1.AddDays(daysOffset);
        return firstMonday.AddDays((week - 1) * 7);
    }
}

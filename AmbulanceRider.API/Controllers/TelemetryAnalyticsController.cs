using System.Globalization;
using System.Text;
using System.Text.Json;
using AmbulanceRider.API.Data;
using AmbulanceRider.API.DTOs;
using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AmbulanceRider.API.Controllers;

/// <summary>
/// Controller for telemetry analytics and export
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class TelemetryAnalyticsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TelemetryAnalyticsController> _logger;

    public TelemetryAnalyticsController(ApplicationDbContext context, ILogger<TelemetryAnalyticsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get telemetry statistics
    /// </summary>
    [HttpGet("stats")]
    public async Task<ActionResult<TelemetryStatsDto>> GetTelemetryStats(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        try
        {
            var start = startDate ?? DateTime.UtcNow.AddMonths(-1);
            var end = endDate ?? DateTime.UtcNow;

            var telemetry = await _context.Telemetries
                .Where(t => t.CreatedAt >= start && t.CreatedAt <= end)
                .ToListAsync();

            var stats = new TelemetryStatsDto
            {
                TotalEvents = telemetry.Count,
                UniqueUsers = telemetry.Where(t => t.UserId.HasValue).Select(t => t.UserId).Distinct().Count(),
                EventTypeCounts = telemetry.GroupBy(t => t.EventType).ToDictionary(g => g.Key, g => g.Count()),
                DeviceTypeCounts = telemetry.Where(t => !string.IsNullOrEmpty(t.DeviceType)).GroupBy(t => t.DeviceType!).ToDictionary(g => g.Key, g => g.Count()),
                BrowserCounts = telemetry.Where(t => !string.IsNullOrEmpty(t.Browser)).GroupBy(t => t.Browser!).ToDictionary(g => g.Key, g => g.Count()),
                OsCounts = telemetry.Where(t => !string.IsNullOrEmpty(t.OperatingSystem)).GroupBy(t => t.OperatingSystem!).ToDictionary(g => g.Key, g => g.Count()),
                StartDate = start,
                EndDate = end
            };

            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting telemetry stats");
            return StatusCode(500, "An error occurred while retrieving telemetry statistics");
        }
    }

    /// <summary>
    /// Get telemetry events with filtering and pagination
    /// </summary>
    [HttpGet("events")]
    public async Task<ActionResult<IEnumerable<TelemetryAnalyticsEventDto>>> GetTelemetryEvents(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] string? eventType = null,
        [FromQuery] Guid? userId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        try
        {
            var start = startDate ?? DateTime.UtcNow.AddMonths(-1);
            var end = endDate ?? DateTime.UtcNow;

            var query = _context.Telemetries
                .Include(t => t.User)
                .Where(t => t.CreatedAt >= start && t.CreatedAt <= end);

            if (!string.IsNullOrEmpty(eventType))
            {
                query = query.Where(t => t.EventType == eventType);
            }

            if (userId.HasValue)
            {
                query = query.Where(t => t.UserId == userId);
            }

            var totalCount = await query.CountAsync();
            var events = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TelemetryAnalyticsEventDto
                {
                    Id = t.Id,
                    EventType = t.EventType,
                    EventDetails = t.EventDetails,
                    UserId = t.UserId,
                    UserName = t.User != null ? t.User.FullName : null,
                    DeviceType = t.DeviceType,
                    Browser = t.Browser,
                    OperatingSystem = t.OperatingSystem,
                    Latitude = t.Latitude,
                    Longitude = t.Longitude,
                    IpAddress = t.IpAddress,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();

            Response.Headers.Append("X-Total-Count", totalCount.ToString());
            Response.Headers.Append("X-Page", page.ToString());
            Response.Headers.Append("X-Page-Size", pageSize.ToString());

            return Ok(events);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting telemetry events");
            return StatusCode(500, "An error occurred while retrieving telemetry events");
        }
    }

    /// <summary>
    /// Export telemetry data to CSV
    /// </summary>
    [HttpGet("export/csv")]
    public async Task<IActionResult> ExportToCsv(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] string? eventType = null)
    {
        try
        {
            var start = startDate ?? DateTime.UtcNow.AddMonths(-1);
            var end = endDate ?? DateTime.UtcNow;

            var query = _context.Telemetries
                .Include(t => t.User)
                .Where(t => t.CreatedAt >= start && t.CreatedAt <= end);

            if (!string.IsNullOrEmpty(eventType))
            {
                query = query.Where(t => t.EventType == eventType);
            }

            var telemetry = await query
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new
                {
                    t.Id,
                    t.EventType,
                    t.EventDetails,
                    t.UserId,
                    UserName = t.User != null ? t.User.FullName : null,
                    t.DeviceType,
                    t.DeviceModel,
                    t.OperatingSystem,
                    t.OsVersion,
                    t.Browser,
                    t.BrowserVersion,
                    t.AppVersion,
                    t.Latitude,
                    t.Longitude,
                    t.Accuracy,
                    t.IpAddress,
                    t.ConnectionType,
                    t.ScreenWidth,
                    t.ScreenHeight,
                    t.BatteryLevel,
                    t.IsCharging,
                    t.CreatedAt
                })
                .ToListAsync();

            using var memoryStream = new MemoryStream();
            using var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8);
            using var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);
            
            await csvWriter.WriteRecordsAsync(telemetry);
            await streamWriter.FlushAsync();
            
            var fileName = $"telemetry_{start:yyyyMMdd}_{end:yyyyMMdd}.csv";
            return File(memoryStream.ToArray(), "text/csv", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting telemetry to CSV");
            return StatusCode(500, "An error occurred while exporting telemetry data");
        }
    }

    /// <summary>
    /// Export telemetry data to JSON
    /// </summary>
    [HttpGet("export/json")]
    public async Task<IActionResult> ExportToJson(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] string? eventType = null)
    {
        try
        {
            var start = startDate ?? DateTime.UtcNow.AddMonths(-1);
            var end = endDate ?? DateTime.UtcNow;

            var query = _context.Telemetries
                .Include(t => t.User)
                .Where(t => t.CreatedAt >= start && t.CreatedAt <= end);

            if (!string.IsNullOrEmpty(eventType))
            {
                query = query.Where(t => t.EventType == eventType);
            }

            var telemetry = await query
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new
                {
                    t.Id,
                    t.EventType,
                    t.EventDetails,
                    t.UserId,
                    UserName = t.User != null ? t.User.FullName : null,
                    t.DeviceType,
                    t.DeviceModel,
                    t.OperatingSystem,
                    t.OsVersion,
                    t.Browser,
                    t.BrowserVersion,
                    t.AppVersion,
                    Location = t.Latitude.HasValue && t.Longitude.HasValue ? new
                    {
                        t.Latitude,
                        t.Longitude,
                        t.Accuracy,
                        t.Altitude,
                        t.Speed,
                        t.Heading
                    } : null,
                    Network = new
                    {
                        t.IpAddress,
                        t.ConnectionType,
                        t.IsOnline
                    },
                    Screen = new
                    {
                        t.ScreenWidth,
                        t.ScreenHeight,
                        t.Orientation
                    },
                    Battery = new
                    {
                        t.BatteryLevel,
                        t.IsCharging
                    },
                    t.CreatedAt
                })
                .ToListAsync();

            var json = JsonSerializer.Serialize(telemetry, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var fileName = $"telemetry_{start:yyyyMMdd}_{end:yyyyMMdd}.json";
            return File(Encoding.UTF8.GetBytes(json), "application/json", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting telemetry to JSON");
            return StatusCode(500, "An error occurred while exporting telemetry data");
        }
    }

    /// <summary>
    /// Get telemetry aggregation by event type over time
    /// </summary>
    [HttpGet("aggregation/by-event-type")]
    public async Task<ActionResult> GetAggregationByEventType(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] string groupBy = "day")
    {
        try
        {
            var start = startDate ?? DateTime.UtcNow.AddMonths(-1);
            var end = endDate ?? DateTime.UtcNow;

            var telemetry = await _context.Telemetries
                .Where(t => t.CreatedAt >= start && t.CreatedAt <= end)
                .ToListAsync();

            var result = new Dictionary<string, List<object>>();

            var eventTypes = telemetry.Select(t => t.EventType).Distinct();

            foreach (var eventType in eventTypes)
            {
                var eventData = telemetry.Where(t => t.EventType == eventType);

                IEnumerable<object> aggregated;

                switch (groupBy.ToLower())
                {
                    case "hour":
                        aggregated = eventData
                            .GroupBy(t => new { t.CreatedAt.Year, t.CreatedAt.Month, t.CreatedAt.Day, t.CreatedAt.Hour })
                            .Select(g => new
                            {
                                Date = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day, g.Key.Hour, 0, 0),
                                Count = g.Count()
                            })
                            .OrderBy(x => x.Date);
                        break;
                    case "week":
                        aggregated = eventData
                            .GroupBy(t => new { Year = t.CreatedAt.Year, Week = GetWeekOfYear(t.CreatedAt) })
                            .Select(g => new
                            {
                                Date = GetDateFromWeek(g.Key.Year, g.Key.Week),
                                Count = g.Count()
                            })
                            .OrderBy(x => x.Date);
                        break;
                    case "month":
                        aggregated = eventData
                            .GroupBy(t => new { t.CreatedAt.Year, t.CreatedAt.Month })
                            .Select(g => new
                            {
                                Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                                Count = g.Count()
                            })
                            .OrderBy(x => x.Date);
                        break;
                    default: // day
                        aggregated = eventData
                            .GroupBy(t => t.CreatedAt.Date)
                            .Select(g => new
                            {
                                Date = g.Key,
                                Count = g.Count()
                            })
                            .OrderBy(x => x.Date);
                        break;
                }

                result[eventType] = aggregated.Cast<object>().ToList();
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting telemetry aggregation");
            return StatusCode(500, "An error occurred while aggregating telemetry data");
        }
    }

    /// <summary>
    /// Get telemetry heatmap data (location-based)
    /// </summary>
    [HttpGet("heatmap")]
    public async Task<ActionResult> GetTelemetryHeatmap(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] string? eventType = null)
    {
        try
        {
            var start = startDate ?? DateTime.UtcNow.AddMonths(-1);
            var end = endDate ?? DateTime.UtcNow;

            var query = _context.Telemetries
                .Where(t => t.CreatedAt >= start && t.CreatedAt <= end && t.Latitude.HasValue && t.Longitude.HasValue);

            if (!string.IsNullOrEmpty(eventType))
            {
                query = query.Where(t => t.EventType == eventType);
            }

            var heatmapData = await query
                .Select(t => new
                {
                    Latitude = t.Latitude!.Value,
                    Longitude = t.Longitude!.Value,
                    t.EventType
                })
                .ToListAsync();

            return Ok(heatmapData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting telemetry heatmap");
            return StatusCode(500, "An error occurred while retrieving heatmap data");
        }
    }

    private static int GetWeekOfYear(DateTime date)
    {
        var culture = CultureInfo.CurrentCulture;
        return culture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
    }

    private static DateTime GetDateFromWeek(int year, int week)
    {
        var jan1 = new DateTime(year, 1, 1);
        var daysOffset = DayOfWeek.Monday - jan1.DayOfWeek;
        var firstMonday = jan1.AddDays(daysOffset);
        return firstMonday.AddDays((week - 1) * 7);
    }
}

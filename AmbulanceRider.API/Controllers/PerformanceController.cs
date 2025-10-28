using AmbulanceRider.API.Data;
using AmbulanceRider.API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AmbulanceRider.API.Controllers;

/// <summary>
/// Controller for performance monitoring
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class PerformanceController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PerformanceController> _logger;

    public PerformanceController(ApplicationDbContext context, ILogger<PerformanceController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get performance metrics
    /// </summary>
    [HttpGet("metrics")]
    public async Task<ActionResult<PerformanceMetricsDto>> GetPerformanceMetrics(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        try
        {
            var start = startDate ?? DateTime.UtcNow.AddHours(-24);
            var end = endDate ?? DateTime.UtcNow;

            var logs = await _context.PerformanceLogs
                .Where(l => l.Timestamp >= start && l.Timestamp <= end)
                .ToListAsync();

            var metrics = new PerformanceMetricsDto
            {
                AverageResponseTime = logs.Any() ? logs.Average(l => l.ResponseTimeMs) : 0,
                TotalRequests = logs.Count,
                FailedRequests = logs.Count(l => l.StatusCode >= 400),
                ErrorRate = logs.Any() ? (double)logs.Count(l => l.StatusCode >= 400) / logs.Count * 100 : 0,
                EndpointCounts = logs.GroupBy(l => l.Endpoint).ToDictionary(g => g.Key, g => g.Count()),
                EndpointAverageResponseTimes = logs.GroupBy(l => l.Endpoint).ToDictionary(g => g.Key, g => g.Average(l => l.ResponseTimeMs)),
                StartDate = start,
                EndDate = end
            };

            return Ok(metrics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting performance metrics");
            return StatusCode(500, "An error occurred while retrieving performance metrics");
        }
    }

    /// <summary>
    /// Get slow requests
    /// </summary>
    [HttpGet("slow-requests")]
    public async Task<ActionResult> GetSlowRequests(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] double threshold = 1000)
    {
        try
        {
            var start = startDate ?? DateTime.UtcNow.AddHours(-24);
            var end = endDate ?? DateTime.UtcNow;

            var slowRequests = await _context.PerformanceLogs
                .Where(l => l.Timestamp >= start && l.Timestamp <= end && l.ResponseTimeMs > threshold)
                .OrderByDescending(l => l.ResponseTimeMs)
                .Take(100)
                .Select(l => new
                {
                    l.Id,
                    l.Endpoint,
                    l.HttpMethod,
                    l.StatusCode,
                    l.ResponseTimeMs,
                    l.Timestamp,
                    l.UserId,
                    l.IpAddress
                })
                .ToListAsync();

            return Ok(slowRequests);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting slow requests");
            return StatusCode(500, "An error occurred while retrieving slow requests");
        }
    }

    /// <summary>
    /// Get error logs
    /// </summary>
    [HttpGet("errors")]
    public async Task<ActionResult> GetErrors(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        try
        {
            var start = startDate ?? DateTime.UtcNow.AddHours(-24);
            var end = endDate ?? DateTime.UtcNow;

            var errors = await _context.PerformanceLogs
                .Where(l => l.Timestamp >= start && l.Timestamp <= end && l.StatusCode >= 400)
                .OrderByDescending(l => l.Timestamp)
                .Take(100)
                .Select(l => new
                {
                    l.Id,
                    l.Endpoint,
                    l.HttpMethod,
                    l.StatusCode,
                    l.ResponseTimeMs,
                    l.Timestamp,
                    l.UserId,
                    l.IpAddress,
                    l.ErrorMessage
                })
                .ToListAsync();

            return Ok(errors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting error logs");
            return StatusCode(500, "An error occurred while retrieving error logs");
        }
    }

    /// <summary>
    /// Get performance trends over time
    /// </summary>
    [HttpGet("trends")]
    public async Task<ActionResult> GetPerformanceTrends(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] string groupBy = "hour")
    {
        try
        {
            var start = startDate ?? DateTime.UtcNow.AddHours(-24);
            var end = endDate ?? DateTime.UtcNow;

            var logs = await _context.PerformanceLogs
                .Where(l => l.Timestamp >= start && l.Timestamp <= end)
                .ToListAsync();

            IEnumerable<object> trends;

            switch (groupBy.ToLower())
            {
                case "minute":
                    trends = logs
                        .GroupBy(l => new { l.Timestamp.Year, l.Timestamp.Month, l.Timestamp.Day, l.Timestamp.Hour, l.Timestamp.Minute })
                        .Select(g => new
                        {
                            Timestamp = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day, g.Key.Hour, g.Key.Minute, 0),
                            AverageResponseTime = g.Average(l => l.ResponseTimeMs),
                            RequestCount = g.Count(),
                            ErrorCount = g.Count(l => l.StatusCode >= 400)
                        })
                        .OrderBy(t => t.Timestamp);
                    break;
                case "day":
                    trends = logs
                        .GroupBy(l => l.Timestamp.Date)
                        .Select(g => new
                        {
                            Timestamp = g.Key,
                            AverageResponseTime = g.Average(l => l.ResponseTimeMs),
                            RequestCount = g.Count(),
                            ErrorCount = g.Count(l => l.StatusCode >= 400)
                        })
                        .OrderBy(t => t.Timestamp);
                    break;
                default: // hour
                    trends = logs
                        .GroupBy(l => new { l.Timestamp.Year, l.Timestamp.Month, l.Timestamp.Day, l.Timestamp.Hour })
                        .Select(g => new
                        {
                            Timestamp = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day, g.Key.Hour, 0, 0),
                            AverageResponseTime = g.Average(l => l.ResponseTimeMs),
                            RequestCount = g.Count(),
                            ErrorCount = g.Count(l => l.StatusCode >= 400)
                        })
                        .OrderBy(t => t.Timestamp);
                    break;
            }

            return Ok(trends);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting performance trends");
            return StatusCode(500, "An error occurred while retrieving performance trends");
        }
    }

    /// <summary>
    /// Get endpoint statistics
    /// </summary>
    [HttpGet("endpoints")]
    public async Task<ActionResult> GetEndpointStatistics(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        try
        {
            var start = startDate ?? DateTime.UtcNow.AddHours(-24);
            var end = endDate ?? DateTime.UtcNow;

            var stats = await _context.PerformanceLogs
                .Where(l => l.Timestamp >= start && l.Timestamp <= end)
                .GroupBy(l => new { l.Endpoint, l.HttpMethod })
                .Select(g => new
                {
                    g.Key.Endpoint,
                    g.Key.HttpMethod,
                    TotalRequests = g.Count(),
                    AverageResponseTime = g.Average(l => l.ResponseTimeMs),
                    MinResponseTime = g.Min(l => l.ResponseTimeMs),
                    MaxResponseTime = g.Max(l => l.ResponseTimeMs),
                    ErrorCount = g.Count(l => l.StatusCode >= 400),
                    ErrorRate = (double)g.Count(l => l.StatusCode >= 400) / g.Count() * 100
                })
                .OrderByDescending(s => s.TotalRequests)
                .ToListAsync();

            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting endpoint statistics");
            return StatusCode(500, "An error occurred while retrieving endpoint statistics");
        }
    }
}

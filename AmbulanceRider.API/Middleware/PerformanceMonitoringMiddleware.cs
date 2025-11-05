using System.Diagnostics;
using AmbulanceRider.API.Data;
using AmbulanceRider.API.Models;

namespace AmbulanceRider.API.Middleware;

/// <summary>
/// Middleware to monitor API performance
/// </summary>
public class PerformanceMonitoringMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<PerformanceMonitoringMiddleware> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public PerformanceMonitoringMiddleware(RequestDelegate next, ILogger<PerformanceMonitoringMiddleware> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _next = next;
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var originalBodyStream = context.Response.Body;
        
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        Exception? exception = null;
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            exception = ex;
            throw;
        }
        finally
        {
            stopwatch.Stop();
            
            var performanceLog = new PerformanceLog
            {
                Endpoint = context.Request.Path,
                HttpMethod = context.Request.Method,
                StatusCode = context.Response.StatusCode,
                ResponseTimeMs = stopwatch.Elapsed.TotalMilliseconds,
                Timestamp = DateTime.UtcNow,
                UserId = context.User?.FindFirst("sub")?.Value ?? context.User?.FindFirst("id")?.Value,
                IpAddress = context.Connection.RemoteIpAddress?.ToString(),
                UserAgent = context.Request.Headers.UserAgent.ToString(),
                RequestSize = context.Request.ContentLength,
                ResponseSize = responseBody.Length,
                ErrorMessage = exception?.Message
            };

            // Log to database asynchronously (fire and forget)
            _ = Task.Run(async () =>
            {
                try
                {
                    // Use a new scope to avoid DbContext threading issues
                    using var scope = _serviceScopeFactory.CreateScope();
                    var scopedDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    await scopedDbContext.PerformanceLogs.AddAsync(performanceLog);
                    await scopedDbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error saving performance log");
                }
            });

            // Log slow requests
            if (stopwatch.Elapsed.TotalMilliseconds > 1000)
            {
                _logger.LogWarning($"Slow request: {context.Request.Method} {context.Request.Path} took {stopwatch.Elapsed.TotalMilliseconds}ms");
            }

            // Copy the response body back to the original stream
            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);
        }
    }
}

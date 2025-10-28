using System.Security.Claims;
using AmbulanceRider.API.Models;
using AmbulanceRider.API.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AmbulanceRider.API.Services;

public class AuditService : IAuditService
{
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<AuditService> _logger;

    public AuditService(
        IAuditLogRepository auditLogRepository,
        IHttpContextAccessor httpContextAccessor,
        IUserRepository userRepository,
        ILogger<AuditService> logger)
    {
        _auditLogRepository = auditLogRepository;
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task LogAsync(
        string entityType, 
        int entityId, 
        string action, 
        string? oldValues = null, 
        string? newValues = null, 
        string? affectedProperties = null, 
        string? notes = null,
        string? ipAddress = null,
        string? userAgent = null)
    {
        try
        {
            var userId = GetCurrentUserId();
            var userName = await GetCurrentUserNameAsync(userId);
            var userRole = GetCurrentUserRole();
            
            ipAddress ??= _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
            userAgent ??= _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString();

            var auditLog = new AuditLog
            {
                EntityType = entityType,
                EntityId = entityId,
                Action = action,
                OldValues = oldValues,
                NewValues = newValues,
                AffectedProperties = affectedProperties,
                UserId = userId,
                UserName = userName,
                UserRole = userRole,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                Notes = notes,
                Timestamp = DateTime.UtcNow
            };

            await _auditLogRepository.AddAsync(auditLog);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging audit trail");
            // Don't throw to avoid breaking the main operation
        }
    }

    public async Task<IEnumerable<AuditLog>> SearchLogsAsync(
        DateTime? startDate = null, 
        DateTime? endDate = null, 
        string? action = null, 
        string? entityType = null, 
        Guid? userId = null,
        int page = 1, 
        int pageSize = 20)
    {
        try
        {
            var logs = await _auditLogRepository.SearchAsync(
                startDate, 
                endDate, 
                action, 
                entityType, 
                userId);

            return logs
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching audit logs");
            throw;
        }
    }

    public async Task<Dictionary<string, int>> GetActionStatsAsync(DateTime? startDate = null, DateTime? endDate = null, string? entityType = null)
    {
        try
        {
            var logs = await _auditLogRepository.SearchAsync(startDate, endDate, null, entityType);
            return logs
                .GroupBy(log => log.Action)
                .ToDictionary(g => g.Key, g => g.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting action statistics");
            return new Dictionary<string, int>();
        }
    }

    public async Task<Dictionary<string, int>> GetUserActivityStatsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var logs = await _auditLogRepository.SearchAsync(startDate, endDate);
            return logs
                .GroupBy(log => log.UserName ?? log.UserId.ToString())
                .ToDictionary(g => g.Key, g => g.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user activity statistics");
            return new Dictionary<string, int>();
        }
    }

    public async Task<Stream> ExportLogsToCsvAsync(DateTime? startDate = null, DateTime? endDate = null, string? action = null, string? entityType = null, Guid? userId = null)
    {
        var stream = new MemoryStream();
        await _auditLogRepository.ExportToCsvAsync(stream, startDate, endDate, action, entityType, userId);
        stream.Position = 0;
        return stream;
    }

    public async Task<Stream> ExportLogsToPdfAsync(DateTime? startDate = null, DateTime? endDate = null, string? action = null, string? entityType = null, Guid? userId = null)
    {
        var stream = new MemoryStream();
        await _auditLogRepository.ExportToPdfAsync(stream, startDate, endDate, action, entityType, userId);
        stream.Position = 0;
        return stream;
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Guid.Empty;
        }
        return userId;
    }

    private async Task<string?> GetCurrentUserNameAsync(Guid userId)
    {
        if (userId == Guid.Empty) return null;
        
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            return user?.UserName;
        }
        catch
        {
            return null;
        }
    }

    private string? GetCurrentUserRole()
    {
        return _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;
    }
}

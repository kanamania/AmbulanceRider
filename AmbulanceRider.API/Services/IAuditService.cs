using AmbulanceRider.API.Models;

namespace AmbulanceRider.API.Services;

public interface IAuditService
{
    Task LogAsync(string entityType, int entityId, string action, string? oldValues = null, string? newValues = null, string? affectedProperties = null, string? notes = null, string? ipAddress = null, string? userAgent = null);
    Task<IEnumerable<AuditLog>> SearchLogsAsync(DateTime? startDate = null, DateTime? endDate = null, string? action = null, string? entityType = null, Guid? userId = null, int page = 1, int pageSize = 20);
    Task<Dictionary<string, int>> GetActionStatsAsync(DateTime? startDate = null, DateTime? endDate = null, string? entityType = null);
    Task<Dictionary<string, int>> GetUserActivityStatsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<Stream> ExportLogsToCsvAsync(DateTime? startDate = null, DateTime? endDate = null, string? action = null, string? entityType = null, Guid? userId = null);
    Task<Stream> ExportLogsToPdfAsync(DateTime? startDate = null, DateTime? endDate = null, string? action = null, string? entityType = null, Guid? userId = null);
}

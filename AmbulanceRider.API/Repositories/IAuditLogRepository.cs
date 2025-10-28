using AmbulanceRider.API.Models;

namespace AmbulanceRider.API.Repositories;

public interface IAuditLogRepository
{
    Task<AuditLog> AddAsync(AuditLog auditLog);
    Task<IEnumerable<AuditLog>> GetByEntityAsync(string entityType, int entityId);
    Task<IEnumerable<AuditLog>> GetByUserAsync(Guid userId);
    Task<IEnumerable<AuditLog>> SearchAsync(DateTime? startDate, DateTime? endDate, string? action = null, string? entityType = null, Guid? userId = null);
    Task<bool> ExportToCsvAsync(Stream stream, DateTime? startDate = null, DateTime? endDate = null, string? action = null, string? entityType = null, Guid? userId = null);
    Task<bool> ExportToPdfAsync(Stream stream, DateTime? startDate = null, DateTime? endDate = null, string? action = null, string? entityType = null, Guid? userId = null);
}

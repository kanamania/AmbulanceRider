using System.Globalization;
using AmbulanceRider.API.Data;
using AmbulanceRider.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text;
using CsvHelper;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace AmbulanceRider.API.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AuditLogRepository> _logger;

    public AuditLogRepository(ApplicationDbContext context, ILogger<AuditLogRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<AuditLog> AddAsync(AuditLog auditLog)
    {
        try
        {
            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
            return auditLog;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding audit log");
            throw;
        }
    }

    public async Task<IEnumerable<AuditLog>> GetByEntityAsync(string entityType, int entityId)
    {
        return await _context.AuditLogs
            .Where(log => log.EntityType == entityType && log.EntityId == entityId)
            .OrderByDescending(log => log.Timestamp)
            .ToListAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetByUserAsync(Guid userId)
    {
        return await _context.AuditLogs
            .Where(log => log.UserId == userId)
            .OrderByDescending(log => log.Timestamp)
            .ToListAsync();
    }

    public async Task<IEnumerable<AuditLog>> SearchAsync(
        DateTime? startDate, 
        DateTime? endDate, 
        string? action = null, 
        string? entityType = null, 
        Guid? userId = null)
    {
        var query = _context.AuditLogs.AsQueryable();

        if (startDate.HasValue)
            query = query.Where(log => log.Timestamp >= startDate.Value);
            
        if (endDate.HasValue)
            query = query.Where(log => log.Timestamp <= endDate.Value);
            
        if (!string.IsNullOrEmpty(action))
            query = query.Where(log => log.Action == action);
            
        if (!string.IsNullOrEmpty(entityType))
            query = query.Where(log => log.EntityType == entityType);
            
        if (userId.HasValue)
            query = query.Where(log => log.UserId == userId.Value);

        return await query.OrderByDescending(log => log.Timestamp).ToListAsync();
    }

    public async Task<bool> ExportToCsvAsync(Stream stream, DateTime? startDate = null, DateTime? endDate = null, string? action = null, string? entityType = null, Guid? userId = null)
    {
        try
        {
            var logs = await SearchAsync(startDate, endDate, action, entityType, userId);
            
            using var writer = new StreamWriter(stream, leaveOpen: true);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            
            await csv.WriteRecordsAsync(logs);
            await writer.FlushAsync();
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting audit logs to CSV");
            return false;
        }
    }

    public async Task<bool> ExportToPdfAsync(Stream stream, DateTime? startDate = null, DateTime? endDate = null, string? action = null, string? entityType = null, Guid? userId = null)
    {
        try
        {
            var logs = (await SearchAsync(startDate, endDate, action, entityType, userId)).ToList();
            
            using var writer = new PdfWriter(stream);
            using var pdf = new PdfDocument(writer);
            using var document = new Document(pdf);
            
            // Add title
            document.Add(new Paragraph("Audit Logs")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(20));
                
            // Add date range if specified
            if (startDate.HasValue || endDate.HasValue)
            {
                var dateRange = $"Date Range: {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}";
                document.Add(new Paragraph(dateRange).SetItalic());
            }
            
            // Add filters
            var filters = new List<string>();
            if (!string.IsNullOrEmpty(action)) filters.Add($"Action: {action}");
            if (!string.IsNullOrEmpty(entityType)) filters.Add($"Entity Type: {entityType}");
            if (userId.HasValue) filters.Add($"User ID: {userId}");
            
            if (filters.Any())
            {
                document.Add(new Paragraph("Filters: " + string.Join(", ", filters)).SetItalic());
            }
            
            // Add table
            var table = new Table(5);
            
            // Add headers
            table.AddHeaderCell("Timestamp");
            table.AddHeaderCell("Action");
            table.AddHeaderCell("Entity");
            table.AddHeaderCell("User");
            table.AddHeaderCell("Details");
            
            // Add rows
            foreach (var log in logs)
            {
                table.AddCell(log.Timestamp.ToString("g"));
                table.AddCell(log.Action);
                table.AddCell($"{log.EntityType} #{log.EntityId}");
                table.AddCell(log.UserName ?? log.UserId.ToString());
                table.AddCell(log.Notes ?? string.Empty);
            }
            
            document.Add(table);
            
            // Add summary
            var summaryText = new Text($"Total records: {logs.Count}");
            var summary = new Paragraph().Add(summaryText);
            summary.SetTextAlignment(TextAlignment.RIGHT);
            summary.SetItalic();
            document.Add(summary);
                
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting audit logs to PDF");
            return false;
        }
    }
}

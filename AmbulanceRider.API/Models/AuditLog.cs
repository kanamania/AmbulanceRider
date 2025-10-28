using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AmbulanceRider.API.Models;

public class AuditLog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string EntityType { get; set; } = string.Empty;
    
    [Required]
    public int EntityId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Action { get; set; } = string.Empty;
    
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? AffectedProperties { get; set; }
    
    [Required]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    [Required]
    public Guid UserId { get; set; }
    
    public string? UserName { get; set; }
    public string? UserRole { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    
    [MaxLength(500)]
    public string? Notes { get; set; }
}

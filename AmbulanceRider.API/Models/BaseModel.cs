using System.ComponentModel.DataAnnotations.Schema;

namespace AmbulanceRider.API.Models;

public abstract class BaseModel
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [Column("CreatedBy")]
    [NotMapped]
    public User? Creator { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    [Column("UpdatedBy")]
    [NotMapped]
    public User? Updater { get; set; }
    public Guid? UpdatedBy { get; set; }
    [Column("DeletedBy")]
    [NotMapped]
    public User? Deleter { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
    public bool IsDeleted => DeletedAt.HasValue;
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AmbulanceRider.API.Models;

public class Company : BaseModel
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    [Required]
    [StringLength(100)]
    public string ContactEmail { get; set; } = string.Empty;
    
    [StringLength(20)]
    public string? ContactPhone { get; set; }
    
    [StringLength(255)]
    public string? Address { get; set; }
    
    // Navigation properties
    public virtual ICollection<User> Users { get; set; } = new List<User>();
    public decimal TaxRate { get; set; }
}

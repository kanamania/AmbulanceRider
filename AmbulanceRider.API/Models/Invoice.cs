using System.ComponentModel.DataAnnotations;

namespace AmbulanceRider.API.Models;

public class Invoice : BaseModel
{
    [Required]
    [StringLength(50)]
    public string InvoiceNumber { get; set; } = string.Empty;
    
    [Required]
    public InvoiceType Type { get; set; }
    
    [Required]
    public int CompanyId { get; set; }
    public virtual Company? Company { get; set; }
    
    [Required]
    public DateTime InvoiceDate { get; set; }
    
    [Required]
    public DateTime PeriodStart { get; set; }
    
    [Required]
    public DateTime PeriodEnd { get; set; }
    
    [Required]
    public decimal SubTotal { get; set; }
    
    [Required]
    public decimal TaxAmount { get; set; }
    
    [Required]
    public decimal TotalAmount { get; set; }
    
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
    
    public DateTime? PaidDate { get; set; }
    
    public DateTime? SentDate { get; set; }
    
    [StringLength(1000)]
    public string? Notes { get; set; }
    
    [StringLength(500)]
    public string? PdfPath { get; set; }
    
    [StringLength(500)]
    public string? ExcelPath { get; set; }
    
    public virtual ICollection<InvoiceTrip> InvoiceTrips { get; set; } = new List<InvoiceTrip>();
}

public class InvoiceTrip : BaseModel
{
    [Required]
    public int InvoiceId { get; set; }
    public virtual Invoice? Invoice { get; set; }
    
    [Required]
    public int TripId { get; set; }
    public virtual Trip? Trip { get; set; }
    
    [Required]
    public decimal BasePrice { get; set; }
    
    [Required]
    public decimal TaxAmount { get; set; }
    
    [Required]
    public decimal TotalPrice { get; set; }
}

public enum InvoiceType
{
    Proforma = 0,
    Final = 1
}

public enum InvoiceStatus
{
    Draft = 0,
    Sent = 1,
    Paid = 2,
    Cancelled = 3
}

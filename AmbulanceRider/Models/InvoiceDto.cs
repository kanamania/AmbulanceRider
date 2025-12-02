namespace AmbulanceRider.Models;

public class InvoiceDto
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? PaidDate { get; set; }
    public DateTime? SentDate { get; set; }
    public string? Notes { get; set; }
    public int TripCount { get; set; }
    public List<InvoiceTripDto> Trips { get; set; } = new();
}

public class InvoiceTripDto
{
    public int Id { get; set; }
    public int TripId { get; set; }
    public string TripName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime ScheduledStartTime { get; set; }
    public DateTime? ActualStartTime { get; set; }
    public DateTime? ActualEndTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? FromLocationName { get; set; }
    public string? ToLocationName { get; set; }
    public string? VehicleName { get; set; }
    public string? DriverName { get; set; }
    public decimal BasePrice { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalPrice { get; set; }
}

public class CreateInvoiceDto
{
    public int CompanyId { get; set; }
    public string Type { get; set; } = "Proforma";
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public string? Notes { get; set; }
}

public class InvoicePreviewDto
{
    public int CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public List<InvoiceTripDto> Trips { get; set; } = new();
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public int TripCount { get; set; }
}

public class MarkInvoicePaidDto
{
    public DateTime PaidDate { get; set; } = DateTime.UtcNow;
}

public class InvoiceFilterDto
{
    public int? CompanyId { get; set; }
    public string? Type { get; set; }
    public string? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

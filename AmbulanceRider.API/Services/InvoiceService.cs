using AmbulanceRider.API.Data;
using AmbulanceRider.API.DTOs;
using AmbulanceRider.API.Models;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Identity;

namespace AmbulanceRider.API.Services;

public class InvoiceService
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;
    private readonly IEmailService _emailService;
    private readonly UserManager<User> _userManager;

    public InvoiceService(UserManager<User> userManager, ApplicationDbContext context,
        IWebHostEnvironment environment, IEmailService emailService)
    {
        _context = context;
        _environment = environment;
        _emailService = emailService;
        _userManager = userManager;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<List<InvoiceDto>> GetInvoicesAsync(InvoiceFilterDto filter)
    {
        var query = _context.Invoices
            .Include(i => i.Company)
            .Include(i => i.InvoiceTrips)
            .ThenInclude(it => it.Trip)
            .ThenInclude(t => t!.Vehicle)
            .Include(i => i.InvoiceTrips)
            .ThenInclude(it => it.Trip)
            .ThenInclude(t => t!.Driver)
            .AsQueryable();

        if (filter.CompanyId.HasValue)
            query = query.Where(i => i.CompanyId == filter.CompanyId.Value);

        if (!string.IsNullOrEmpty(filter.Type))
        {
            if (Enum.TryParse<InvoiceType>(filter.Type, true, out var type))
                query = query.Where(i => i.Type == type);
        }

        if (!string.IsNullOrEmpty(filter.Status))
        {
            if (Enum.TryParse<InvoiceStatus>(filter.Status, true, out var status))
                query = query.Where(i => i.Status == status);
        }

        if (filter.StartDate.HasValue)
            query = query.Where(i => i.InvoiceDate >= filter.StartDate.Value);

        if (filter.EndDate.HasValue)
            query = query.Where(i => i.InvoiceDate <= filter.EndDate.Value);

        var invoices = await query.OrderByDescending(i => i.InvoiceDate).ToListAsync();

        return invoices.Select(MapToDto).ToList();
    }

    public async Task<InvoiceDto?> GetInvoiceByIdAsync(int id)
    {
        var invoice = await _context.Invoices
            .Include(i => i.Company)
            .Include(i => i.InvoiceTrips)
            .ThenInclude(it => it.Trip)
            .ThenInclude(t => t!.Vehicle)
            .Include(i => i.InvoiceTrips)
            .ThenInclude(it => it.Trip)
            .ThenInclude(t => t!.Driver)
            .FirstOrDefaultAsync(i => i.Id == id);

        return invoice == null ? null : MapToDto(invoice);
    }

    public async Task<InvoicePreviewDto> GetInvoicePreviewAsync(int companyId, DateTime periodStart, DateTime periodEnd)
    {
        var company = await _context.Companies.FindAsync(companyId);
        if (company == null)
            throw new Exception("Company not found");

        var trips = await _context.Trips
            .Include(t => t.Vehicle)
            .Include(t => t.Driver)
            .Where(t =>
                t.Status == TripStatus.Completed &&
                !t.IsPaid &&
                t.CreatedAt >= periodStart &&
                t.CreatedAt <= periodEnd &&
                _context.Users.Any(u => u.Id == t.CreatedBy && u.CompanyId == companyId))
            .OrderBy(t => t.CreatedAt)
            .ToListAsync();

        var tripDtos = trips.Select(t => new InvoiceTripDto
        {
            TripId = t.Id,
            TripName = t.Name,
            CreatedAt = t.CreatedAt,
            ScheduledStartTime = t.ScheduledStartTime,
            ActualStartTime = t.ActualStartTime,
            ActualEndTime = t.ActualEndTime,
            Status = t.Status.ToString(),
            FromLocationName = t.FromLocationName,
            ToLocationName = t.ToLocationName,
            VehicleName = t.Vehicle?.Name,
            DriverName = t.Driver != null ? $"{t.Driver.FirstName} {t.Driver.LastName}" : null,
            BasePrice = t.BasePrice ?? 0,
            TaxAmount = t.TaxAmount ?? 0,
            TotalPrice = t.TotalPrice ?? 0
        }).ToList();

        var subTotal = tripDtos.Sum(t => t.BasePrice);
        var taxAmount = tripDtos.Sum(t => t.TaxAmount);
        var totalAmount = tripDtos.Sum(t => t.TotalPrice);

        return new InvoicePreviewDto
        {
            CompanyId = companyId,
            CompanyName = company.Name,
            PeriodStart = periodStart,
            PeriodEnd = periodEnd,
            Trips = tripDtos,
            SubTotal = subTotal,
            TaxAmount = taxAmount,
            TotalAmount = totalAmount,
            TripCount = tripDtos.Count
        };
    }

    public async Task<InvoiceDto> CreateInvoiceAsync(CreateInvoiceDto dto)
    {
        var company = await _context.Companies.FindAsync(dto.CompanyId);
        if (company == null)
            throw new Exception("Company not found");

        var trips = await _context.Trips
            .Include(t => t.Vehicle)
            .Include(t => t.Driver)
            .Where(t =>
                t.Status == TripStatus.Completed &&
                !t.IsPaid &&
                t.CreatedAt >= dto.PeriodStart &&
                t.CreatedAt <= dto.PeriodEnd &&
                _context.Users.Any(u => u.Id == t.CreatedBy && u.CompanyId == dto.CompanyId))
            .ToListAsync();

        if (!trips.Any())
            throw new Exception("No unpaid trips found for the specified period");

        var invoiceType = Enum.Parse<InvoiceType>(dto.Type, true);
        var invoiceNumber = await GenerateInvoiceNumberAsync(invoiceType);

        var invoice = new Invoice
        {
            InvoiceNumber = invoiceNumber,
            Type = invoiceType,
            CompanyId = dto.CompanyId,
            InvoiceDate = DateTime.UtcNow,
            PeriodStart = dto.PeriodStart,
            PeriodEnd = dto.PeriodEnd,
            SubTotal = trips.Sum(t => t.BasePrice ?? 0),
            TaxAmount = trips.Sum(t => t.TaxAmount ?? 0),
            TotalAmount = trips.Sum(t => t.TotalPrice ?? 0),
            Status = InvoiceStatus.Draft,
            Notes = dto.Notes
        };

        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();

        foreach (var trip in trips)
        {
            var invoiceTrip = new InvoiceTrip
            {
                InvoiceId = invoice.Id,
                TripId = trip.Id,
                BasePrice = trip.BasePrice ?? 0,
                TaxAmount = trip.TaxAmount ?? 0,
                TotalPrice = trip.TotalPrice ?? 0
            };
            _context.InvoiceTrips.Add(invoiceTrip);
        }

        await _context.SaveChangesAsync();

        var createdInvoice = await GetInvoiceByIdAsync(invoice.Id);
        return createdInvoice!;
    }

    public async Task<InvoiceDto> MarkInvoiceAsPaidAsync(int id, MarkInvoicePaidDto dto)
    {
        var invoice = await _context.Invoices
            .Include(i => i.InvoiceTrips)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (invoice == null)
            throw new Exception("Invoice not found");

        if (invoice.Status == InvoiceStatus.Paid)
            throw new Exception("Invoice is already marked as paid");

        invoice.Status = InvoiceStatus.Paid;
        invoice.PaidDate = dto.PaidDate;

        var tripIds = invoice.InvoiceTrips.Select(it => it.TripId).ToList();
        var trips = await _context.Trips.Where(t => tripIds.Contains(t.Id)).ToListAsync();

        foreach (var trip in trips)
        {
            trip.IsPaid = true;
            trip.PaidDate = dto.PaidDate;
        }

        await _context.SaveChangesAsync();

        return (await GetInvoiceByIdAsync(id))!;
    }

    public async Task<(byte[] PdfBytes, byte[] ExcelBytes)> GenerateInvoiceFilesAsync(int id)
    {
        var invoice = await _context.Invoices
            .Include(i => i.Company)
            .Include(i => i.InvoiceTrips)
            .ThenInclude(it => it.Trip)
            .ThenInclude(t => t!.Vehicle)
            .Include(i => i.InvoiceTrips)
            .ThenInclude(it => it.Trip)
            .ThenInclude(t => t!.Driver)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (invoice == null)
            throw new Exception("Invoice not found");

        var pdfBytes = GeneratePdf(invoice);
        var excelBytes = GenerateExcel(invoice);

        var invoicesDir = Path.Combine(_environment.WebRootPath, "invoices");
        Directory.CreateDirectory(invoicesDir);

        var pdfFileName = $"{invoice.InvoiceNumber}.pdf";
        var excelFileName = $"{invoice.InvoiceNumber}.xlsx";
        var pdfPath = Path.Combine(invoicesDir, pdfFileName);
        var excelPath = Path.Combine(invoicesDir, excelFileName);

        await File.WriteAllBytesAsync(pdfPath, pdfBytes);
        await File.WriteAllBytesAsync(excelPath, excelBytes);

        invoice.PdfPath = $"/invoices/{pdfFileName}";
        invoice.ExcelPath = $"/invoices/{excelFileName}";
        invoice.Status = InvoiceStatus.Sent;
        invoice.SentDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return (pdfBytes, excelBytes);
    }

    public async Task<List<InvoiceDto>> GenerateTestInvoicesAsync(int count, int companyId)
    {
        try
        {
            var random = new Random();
            var invoices = new List<InvoiceDto>();

            // Get required data
            var drivers = (await _userManager.GetUsersInRoleAsync("Driver")) ?? new List<User>();

            var creators = (await _userManager.GetUsersInRoleAsync("User")) ?? new List<User>();

            var vehicles = await _context.Vehicles.ToListAsync();
            var pricingMatrices = await _context.PricingMatrices
                .Include(pm => pm.Region)
                .Where(pm => pm.Region!.Code == "DAR")
                .ToListAsync();
            var locations = await _context.Locations.ToListAsync();

            Console.WriteLine($"Drivers count: {drivers.Count}");
            Console.WriteLine($"Creators count: {creators.Count}");
            Console.WriteLine($"Vehicles count: {vehicles.Count}");
            Console.WriteLine($"Pricing matrices count: {pricingMatrices.Count}");
            Console.WriteLine($"Locations count: {locations.Count}");
            // Create test trips
            var testTrips = new List<Trip>();
            for (int i = 0; i < count * 5; i++)
            {
                var pricingMatrix = pricingMatrices[random.Next(pricingMatrices.Count)];
                var fromLocation = locations[random.Next(locations.Count)];
                var toLocation = locations[random.Next(locations.Count)];
                var vehicle = vehicles[random.Next(vehicles.Count)];
                var driver = drivers[random.Next(drivers.Count)];
                var creator = creators[random.Next(creators.Count)];

                testTrips.Add(new Trip
                {
                    Name = $"Test Trip {i + 1} {fromLocation.Name} -> {toLocation.Name}",
                    Status = TripStatus.Completed,
                    IsPaid = false,
                    PricingMatrixId = pricingMatrix.Id,
                    BasePrice = pricingMatrix.BasePrice,
                    TaxAmount = pricingMatrix.BasePrice * pricingMatrix.TaxRate,
                    TotalPrice = pricingMatrix.BasePrice * (1 + pricingMatrix.TaxRate),
                    Weight = random.Next((int)pricingMatrix.MinWeight, (int)pricingMatrix.MaxWeight),
                    Width = random.Next((int)pricingMatrix.MinWidth, (int)pricingMatrix.MaxWidth),
                    Height = random.Next((int)pricingMatrix.MinHeight, (int)pricingMatrix.MaxHeight),
                    Length = random.Next((int)pricingMatrix.MinLength, (int)pricingMatrix.MaxLength),
                    CreatedAt = DateTime.UtcNow.AddDays(-random.Next(30)),
                    VehicleId = vehicle.Id,
                    DriverId = driver.Id,
                    CreatedBy = creator.Id,
                    ScheduledStartTime = DateTime.UtcNow.AddHours(-random.Next(24, 72)),
                    ActualStartTime = DateTime.UtcNow.AddHours(-random.Next(12, 24)),
                    ActualEndTime = DateTime.UtcNow.AddHours(-random.Next(1, 12)),
                    FromLocationName = fromLocation.Name,
                    ToLocationName = toLocation.Name,
                    FromLatitude = fromLocation.Latitude,
                    FromLongitude = fromLocation.Longitude,
                    ToLatitude = toLocation.Latitude,
                    ToLongitude = toLocation.Longitude,
                    EstimatedDistance = random.Next(5, 100),
                    EstimatedDuration = random.Next(15, 120),
                });
            }

            await _context.Trips.AddRangeAsync(testTrips);
            await _context.SaveChangesAsync();

            // Create invoices
            for (int i = 0; i < count; i++)
            {
                var invoiceType = random.Next(2) == 0 ? InvoiceType.Proforma : InvoiceType.Final;
                var invoiceNumber = await GenerateInvoiceNumberAsync(invoiceType);
                var periodStart = DateTime.UtcNow.AddDays(-random.Next(30));
                var periodEnd = periodStart.AddDays(random.Next(1, 30));

                // Select 2-5 trips for this invoice
                var tripCount = random.Next(2, 6);
                var selectedTrips = testTrips
                    .Skip(i * 2)
                    .Take(tripCount)
                    .ToList();

                var invoice = new Invoice
                {
                    InvoiceNumber = invoiceNumber,
                    Type = invoiceType,
                    CompanyId = companyId,
                    InvoiceDate = DateTime.UtcNow,
                    PeriodStart = periodStart,
                    PeriodEnd = periodEnd,
                    SubTotal = selectedTrips.Sum(t => t.BasePrice)!.Value,
                    TaxAmount = selectedTrips.Sum(t => t.TaxAmount)!.Value,
                    TotalAmount = selectedTrips.Sum(t => t.TotalPrice)!.Value,
                    Status = InvoiceStatus.Draft,
                    Notes = "Test invoice",
                    InvoiceTrips = selectedTrips.Select(t => new InvoiceTrip
                    {
                        TripId = t.Id,
                        BasePrice = t.BasePrice!.Value,
                        TaxAmount = t.TaxAmount!.Value,
                        TotalPrice = t.TotalPrice!.Value
                    }).ToList()
                };

                _context.Invoices.Add(invoice);
                await _context.SaveChangesAsync();
                invoices.Add(MapToDto(invoice));
            }

            return invoices;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating test invoices: {ex}");
            throw new Exception($"Failed to generate test invoices: {ex.Message}");
        }
    }

    public async Task SendInvoiceEmailAsync(int id, List<string> recipientEmails, string? subject = null,
        string? body = null)
    {
        var invoice = await _context.Invoices
            .Include(i => i.Company)
            .Include(i => i.InvoiceTrips)
            .ThenInclude(it => it.Trip)
            .FirstOrDefaultAsync(i => i.Id == id) ?? throw new Exception("Invoice not found");

        var (pdfBytes, excelBytes) = await GenerateInvoiceFilesAsync(id);

        var defaultSubject = $"{invoice.Type} Invoice {invoice.InvoiceNumber} - {invoice.Company?.Name}";
        var defaultBody = $"""
                           <div style="font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;">
                               <!-- Header -->
                               <div style="text-align: center; margin-bottom: 20px;">
                                   <h2 style="color: #007bff;">AmbulanceRider Invoice</h2>
                                   <p style="color: #6c757d;">{invoice.InvoiceNumber}</p>
                               </div>

                               <!-- Invoice Summary -->
                               <div style="background-color: #f8f9fa; padding: 20px; border-radius: 5px; margin-bottom: 20px;">
                                   <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 15px; margin-bottom: 15px;">
                                       <div>
                                           <h3 style="margin-top: 0;">Invoice Details</h3>
                                           <p><strong>Date:</strong> {invoice.InvoiceDate:dd/MM/yyyy}</p>
                                           <p><strong>Type:</strong> {invoice.Type}</p>
                                           <p><strong>Status:</strong> 
                                               <span style="color: {GetStatusColor(invoice.Status)}">
                                                   {invoice.Status}
                                               </span>
                                           </p>
                                           {(invoice.PaidDate != null ? $"<p><strong>Paid Date:</strong> {invoice.PaidDate:dd/MM/yyyy}</p>" : "")}
                                       </div>
                                       <div>
                                           <h3 style="margin-top: 0;">Company Details</h3>
                                           <p><strong>Name:</strong> {invoice.Company?.Name}</p>
                                           <p><strong>Billing Period:</strong> {invoice.PeriodStart:dd/MM/yyyy} to {invoice.PeriodEnd:dd/MM/yyyy}</p>
                                           <p><strong>Trips:</strong> {invoice.InvoiceTrips.Count}</p>
                                       </div>
                                   </div>

                                   <!-- Amount Breakdown -->
                                   <div style="border-top: 1px solid #dee2e6; padding-top: 15px;">
                                       <div style="display: grid; grid-template-columns: 2fr 1fr; gap: 10px;">
                                           <div>
                                               <p><strong>Subtotal:</strong></p>
                                               <p><strong>Tax ({invoice.Company?.TaxRate ?? 16}%):</strong></p>
                                               <p style="font-weight: bold; margin-top: 10px;">Total Amount:</p>
                                           </div>
                                           <div style="text-align: right;">
                                               <p>TZS {invoice.SubTotal:N2}</p>
                                               <p>TZS {invoice.TaxAmount:N2}</p>
                                               <p style="font-size: 1.2em; color: #28a745;">TZS {invoice.TotalAmount:N2}</p>
                                           </div>
                                       </div>
                                   </div>
                               </div>

                               <!-- Payment Instructions -->
                               <div style="background-color: #e7f5ff; padding: 15px; border-radius: 5px; margin-bottom: 20px;">
                                   <h3 style="margin-top: 0; color: #0056b3;">Payment Instructions</h3>
                                   <p><strong>Due Date:</strong> {invoice.InvoiceDate.AddDays(30):dd/MM/yyyy}</p>
                                   <p><strong>Bank:</strong> Equity Bank Kenya</p>
                                   <p><strong>Account:</strong> AmbulanceRider Ltd (1234567890)</p>
                                   <p><strong>Reference:</strong> {invoice.InvoiceNumber}</p>
                                   {(invoice.Status == InvoiceStatus.Paid ?
                                        "<p style=\"color: #28a745;\">✓ Payment received - thank you!</p>" : "") +
                                    "<p>Please make payment within 30 days</p>"}
                               </div>

                               <!-- Notes -->
                               {(!string.IsNullOrEmpty(invoice.Notes) ? $"""
                                                                         <div style="margin-bottom: 20px;">
                                                                             <h3 style="margin-top: 0;">Notes</h3>
                                                                             <p style="padding: 10px; background-color: #f8f9fa; border-radius: 5px;">
                                                                                 {invoice.Notes}
                                                                             </p>
                                                                         </div>
                                                                         """ : "")}

                               <!-- Attachments -->
                               <div style="margin-bottom: 20px;">
                                   <h3 style="margin-top: 0;">Attachments</h3>
                                   <ul>
                                       <li>{invoice.InvoiceNumber}.pdf - Full invoice details</li>
                                       <li>{invoice.InvoiceNumber}.xlsx - Itemized trip records</li>
                                   </ul>
                               </div>

                               <!-- Footer -->
                               <div style="text-align: center; color: #6c757d; font-size: 0.9em; margin-top: 30px;">
                                   <p>AmbulanceRider Ltd | P.O. Box 12345-00100, Nairobi</p>
                                   <p>support@ambulancerider.com | +254 700 000 000</p>
                               </div>
                           </div>
                           """;

        var attachments = new Dictionary<string, byte[]>
        {
            { $"{invoice.InvoiceNumber}.pdf", pdfBytes },
            { $"{invoice.InvoiceNumber}.xlsx", excelBytes }
        };

        await _emailService.SendEmailWithAttachmentsAsync(
            recipientEmails,
            subject ?? defaultSubject,
            body ?? defaultBody,
            attachments);
    }

    private string GetStatusColor(InvoiceStatus status)
    {
        return status switch
        {
            InvoiceStatus.Paid => "#28a745",
            InvoiceStatus.Sent => "#17a2b8",
            InvoiceStatus.Cancelled => "#dc3545",
            _ => "#6c757d" // Draft
        };
    }

    private byte[] GeneratePdf(Invoice invoice)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(50);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Page ");
                    x.CurrentPageNumber();
                    x.Span(" of ");
                    x.TotalPages();
                });
            });
        });

        void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text("AmbulanceRider").FontSize(20).Bold().FontColor(Colors.Blue.Medium);
                    column.Item().Text("Ambulance Management System").FontSize(10);
                    column.Item().PaddingTop(5).Text("Email: info@ambulancerider.com").FontSize(9);
                    column.Item().Text("Phone: +254 700 000 000").FontSize(9);
                });

                row.RelativeItem().Column(column =>
                {
                    column.Item().AlignRight().Text($"{invoice.Type.ToString().ToUpper()} INVOICE").FontSize(16).Bold();
                    column.Item().AlignRight().Text($"Invoice #: {invoice.InvoiceNumber}").FontSize(10);
                    column.Item().AlignRight().Text($"Date: {invoice.InvoiceDate:dd/MM/yyyy}").FontSize(10);
                    column.Item().AlignRight().Text($"Status: {invoice.Status}").FontSize(10).Bold();
                });
            });
        }

        void ComposeContent(IContainer container)
        {
            container.PaddingVertical(20).Column(column =>
            {
                column.Spacing(10);

                column.Item().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("Bill To:").Bold().FontSize(11);
                        col.Item().PaddingTop(5).Text(invoice.Company?.Name ?? "").FontSize(10);
                        if (!string.IsNullOrEmpty(invoice.Company?.Address))
                            col.Item().Text(invoice.Company.Address).FontSize(9);
                        if (!string.IsNullOrEmpty(invoice.Company?.ContactEmail))
                            col.Item().Text($"Email: {invoice.Company.ContactEmail}").FontSize(9);
                        if (!string.IsNullOrEmpty(invoice.Company?.ContactPhone))
                            col.Item().Text($"Phone: {invoice.Company.ContactPhone}").FontSize(9);
                    });

                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("Invoice Period:").Bold().FontSize(11);
                        col.Item().PaddingTop(5).Text($"From: {invoice.PeriodStart:dd/MM/yyyy}").FontSize(10);
                        col.Item().Text($"To: {invoice.PeriodEnd:dd/MM/yyyy}").FontSize(10);
                    });
                });

                column.Item().PaddingTop(20).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(30);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(1.5f);
                        columns.RelativeColumn(1.5f);
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("#").Bold();
                        header.Cell().Element(CellStyle).Text("Trip Name").Bold();
                        header.Cell().Element(CellStyle).Text("Route").Bold();
                        header.Cell().Element(CellStyle).Text("Vehicle").Bold();
                        header.Cell().Element(CellStyle).Text("Driver").Bold();
                        header.Cell().Element(CellStyle).AlignRight().Text("Base").Bold();
                        header.Cell().Element(CellStyle).AlignRight().Text("Tax").Bold();
                        header.Cell().Element(CellStyle).AlignRight().Text("Total").Bold();

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten1).PaddingVertical(5);
                        }
                    });

                    var index = 1;
                    foreach (var item in invoice.InvoiceTrips)
                    {
                        var trip = item.Trip;
                        table.Cell().Element(CellStyle).Text(index.ToString());
                        table.Cell().Element(CellStyle).Text(trip?.Name ?? "");
                        table.Cell().Element(CellStyle)
                            .Text($"{trip?.FromLocationName ?? "N/A"} → {trip?.ToLocationName ?? "N/A"}");
                        table.Cell().Element(CellStyle).Text(trip?.Vehicle?.Name ?? "N/A");
                        table.Cell().Element(CellStyle).Text(trip?.Driver != null
                            ? $"{trip.Driver.FirstName} {trip.Driver.LastName}"
                            : "N/A");
                        table.Cell().Element(CellStyle).AlignRight().Text($"{item.BasePrice:N2}");
                        table.Cell().Element(CellStyle).AlignRight().Text($"{item.TaxAmount:N2}");
                        table.Cell().Element(CellStyle).AlignRight().Text($"{item.TotalPrice:N2}");

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                        }

                        index++;
                    }
                });

                column.Item().PaddingTop(10).AlignRight().Column(col =>
                {
                    col.Item().Row(row =>
                    {
                        row.AutoItem().Width(100).Text("Subtotal:").Bold();
                        row.AutoItem().Width(80).AlignRight().Text($"TZS {invoice.SubTotal:N2}");
                    });
                    col.Item().Row(row =>
                    {
                        row.AutoItem().Width(100).Text("Tax:").Bold();
                        row.AutoItem().Width(80).AlignRight().Text($"TZS {invoice.TaxAmount:N2}");
                    });
                    col.Item().PaddingTop(5).BorderTop(1).BorderColor(Colors.Grey.Medium).Row(row =>
                    {
                        row.AutoItem().Width(100).Text("Total:").Bold().FontSize(12);
                        row.AutoItem().Width(80).AlignRight().Text($"TZS {invoice.TotalAmount:N2}").Bold().FontSize(12);
                    });
                });

                if (!string.IsNullOrEmpty(invoice.Notes))
                {
                    column.Item().PaddingTop(20).Column(col =>
                    {
                        col.Item().Text("Notes:").Bold();
                        col.Item().PaddingTop(5).Text(invoice.Notes).FontSize(9);
                    });
                }

                column.Item().PaddingTop(30).Column(col =>
                {
                    col.Item().Text("Payment Terms:").Bold().FontSize(9);
                    col.Item().Text("Payment is due within 30 days of invoice date.").FontSize(8);
                    col.Item().Text("Please include invoice number with payment.").FontSize(8);
                });
            });
        }

        return document.GeneratePdf();
    }

    private byte[] GenerateExcel(Invoice invoice)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Invoice");

        worksheet.Cell("A1").Value = "AmbulanceRider";
        worksheet.Cell("A1").Style.Font.Bold = true;
        worksheet.Cell("A1").Style.Font.FontSize = 16;

        worksheet.Cell("A2").Value = $"{invoice.Type} Invoice";
        worksheet.Cell("A2").Style.Font.Bold = true;

        worksheet.Cell("A4").Value = "Invoice Number:";
        worksheet.Cell("B4").Value = invoice.InvoiceNumber;
        worksheet.Cell("A5").Value = "Invoice Date:";
        worksheet.Cell("B5").Value = invoice.InvoiceDate.ToString("dd/MM/yyyy");
        worksheet.Cell("A6").Value = "Status:";
        worksheet.Cell("B6").Value = invoice.Status.ToString();

        worksheet.Cell("A8").Value = "Company:";
        worksheet.Cell("B8").Value = invoice.Company?.Name ?? "";
        worksheet.Cell("A9").Value = "Period:";
        worksheet.Cell("B9").Value = $"{invoice.PeriodStart:dd/MM/yyyy} - {invoice.PeriodEnd:dd/MM/yyyy}";

        var headerRow = 11;
        worksheet.Cell(headerRow, 1).Value = "#";
        worksheet.Cell(headerRow, 2).Value = "Trip Name";
        worksheet.Cell(headerRow, 3).Value = "From";
        worksheet.Cell(headerRow, 4).Value = "To";
        worksheet.Cell(headerRow, 5).Value = "Vehicle";
        worksheet.Cell(headerRow, 6).Value = "Driver";
        worksheet.Cell(headerRow, 7).Value = "Date";
        worksheet.Cell(headerRow, 8).Value = "Base Price";
        worksheet.Cell(headerRow, 9).Value = "Tax";
        worksheet.Cell(headerRow, 10).Value = "Total";

        var headerRange = worksheet.Range(headerRow, 1, headerRow, 10);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

        var row = headerRow + 1;
        var index = 1;
        foreach (var item in invoice.InvoiceTrips)
        {
            var trip = item.Trip;
            worksheet.Cell(row, 1).Value = index;
            worksheet.Cell(row, 2).Value = trip?.Name ?? "";
            worksheet.Cell(row, 3).Value = trip?.FromLocationName ?? "N/A";
            worksheet.Cell(row, 4).Value = trip?.ToLocationName ?? "N/A";
            worksheet.Cell(row, 5).Value = trip?.Vehicle?.Name ?? "N/A";
            worksheet.Cell(row, 6).Value =
                trip?.Driver != null ? $"{trip.Driver.FirstName} {trip.Driver.LastName}" : "N/A";
            worksheet.Cell(row, 7).Value = trip?.ActualEndTime?.ToString("dd/MM/yyyy") ?? "";
            worksheet.Cell(row, 8).Value = item.BasePrice;
            worksheet.Cell(row, 9).Value = item.TaxAmount;
            worksheet.Cell(row, 10).Value = item.TotalPrice;
            row++;
            index++;
        }

        row++;
        worksheet.Cell(row, 9).Value = "Subtotal:";
        worksheet.Cell(row, 9).Style.Font.Bold = true;
        worksheet.Cell(row, 10).Value = invoice.SubTotal;
        worksheet.Cell(row, 10).Style.Font.Bold = true;

        row++;
        worksheet.Cell(row, 9).Value = "Tax:";
        worksheet.Cell(row, 9).Style.Font.Bold = true;
        worksheet.Cell(row, 10).Value = invoice.TaxAmount;
        worksheet.Cell(row, 10).Style.Font.Bold = true;

        row++;
        worksheet.Cell(row, 9).Value = "Total:";
        worksheet.Cell(row, 9).Style.Font.Bold = true;
        worksheet.Cell(row, 10).Value = invoice.TotalAmount;
        worksheet.Cell(row, 10).Style.Font.Bold = true;

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private async Task<string> GenerateInvoiceNumberAsync(InvoiceType type)
    {
        var prefix = type == InvoiceType.Proforma ? "PRO" : "INV";
        var year = DateTime.UtcNow.Year;
        var month = DateTime.UtcNow.Month;

        var lastInvoice = await _context.Invoices
            .Where(i => i.Type == type && i.InvoiceNumber.StartsWith($"{prefix}-{year}{month:D2}"))
            .OrderByDescending(i => i.InvoiceNumber)
            .FirstOrDefaultAsync();

        int sequence = 1;
        if (lastInvoice != null)
        {
            var parts = lastInvoice.InvoiceNumber.Split('-');
            if (parts.Length == 3 && int.TryParse(parts[2], out var lastSequence))
            {
                sequence = lastSequence + 1;
            }
        }

        return $"{prefix}-{year}{month:D2}-{sequence:D4}";
    }

    private InvoiceDto MapToDto(Invoice invoice)
    {
        return new InvoiceDto
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            Type = invoice.Type.ToString(),
            CompanyId = invoice.CompanyId,
            CompanyName = invoice.Company?.Name ?? "",
            InvoiceDate = invoice.InvoiceDate,
            PeriodStart = invoice.PeriodStart,
            PeriodEnd = invoice.PeriodEnd,
            SubTotal = invoice.SubTotal,
            TaxAmount = invoice.TaxAmount,
            TotalAmount = invoice.TotalAmount,
            Status = invoice.Status.ToString(),
            PaidDate = invoice.PaidDate,
            SentDate = invoice.SentDate,
            Notes = invoice.Notes,
            TripCount = invoice.InvoiceTrips.Count,
            Trips = invoice.InvoiceTrips.Select(it => new InvoiceTripDto
            {
                Id = it.Id,
                TripId = it.TripId,
                TripName = it.Trip?.Name ?? "",
                ScheduledStartTime = it.Trip?.ScheduledStartTime ?? DateTime.MinValue,
                ActualStartTime = it.Trip?.ActualStartTime,
                ActualEndTime = it.Trip?.ActualEndTime,
                Status = it.Trip?.Status.ToString() ?? "",
                FromLocationName = it.Trip?.FromLocationName,
                ToLocationName = it.Trip?.ToLocationName,
                VehicleName = it.Trip?.Vehicle?.Name,
                DriverName = it.Trip?.Driver != null ? $"{it.Trip.Driver.FirstName} {it.Trip.Driver.LastName}" : null,
                BasePrice = it.BasePrice,
                TaxAmount = it.TaxAmount,
                TotalPrice = it.TotalPrice
            }).ToList()
        };
    }
}
# Invoice System Documentation

## Overview

The AmbulanceRider Invoice System provides comprehensive invoicing capabilities for billing companies based on completed trips. The system supports both **Proforma** and **Final** invoices with professional PDF and Excel generation.

## Features

### Invoice Types

1. **Proforma Invoice**
   - Preliminary invoice for estimation
   - Prefix: `PRO-YYYYMM-XXXX`
   - Used for quotations and advance billing

2. **Final Invoice**
   - Official invoice for payment
   - Prefix: `INV-YYYYMM-XXXX`
   - Used for actual billing and payment collection

### Key Capabilities

- ✅ **Automatic Invoice Generation** - Create invoices from completed trips
- ✅ **PDF Export** - Professional PDF invoices using QuestPDF
- ✅ **Excel Export** - Detailed spreadsheets using ClosedXML
- ✅ **Invoice Preview** - Preview invoice before creation
- ✅ **Payment Tracking** - Mark invoices as paid
- ✅ **Automatic Trip Updates** - Mark all trips as paid when invoice is paid
- ✅ **Flexible Date Ranges** - Weekly, monthly, or custom date ranges
- ✅ **Multi-format Download** - Download PDF, Excel, or both as ZIP
- ✅ **Sequential Numbering** - Automatic invoice number generation

## Invoice Workflow

### 1. Create Invoice

```
1. Navigate to Invoices page
2. Click "Create Invoice"
3. Select Company
4. Choose Invoice Type (Proforma/Final)
5. Set Period Start and End dates
6. Preview shows:
   - Number of trips
   - Subtotal, Tax, Total amounts
7. Add optional notes
8. Click "Create Invoice"
```

### 2. View Invoice

```
- Invoice list shows all invoices with filters
- Click on invoice to view details
- See complete trip breakdown
- View invoice summary and company details
```

### 3. Download Invoice

```
Options:
- Download PDF only
- Download Excel only
- Download both as ZIP file

Files are automatically generated and saved to:
/wwwroot/invoices/
```

### 4. Mark as Paid

```
1. Open invoice detail or click "Mark as Paid" from list
2. Confirm payment date (defaults to current date)
3. System automatically:
   - Updates invoice status to "Paid"
   - Marks all trips in invoice as paid
   - Records payment date
```

## API Endpoints

### GET /api/invoice

Get all invoices with optional filters.

**Query Parameters:**
- `CompanyId` (optional) - Filter by company
- `Type` (optional) - Filter by type (Proforma/Final)
- `Status` (optional) - Filter by status (Draft/Sent/Paid/Cancelled)
- `StartDate` (optional) - Filter by invoice date start
- `EndDate` (optional) - Filter by invoice date end

**Response:**
```json
[
  {
    "id": 1,
    "invoiceNumber": "INV-202412-0001",
    "type": "Final",
    "companyId": 1,
    "companyName": "ABC Ambulance Services",
    "invoiceDate": "2024-12-02T08:00:00Z",
    "periodStart": "2024-11-25T00:00:00Z",
    "periodEnd": "2024-12-01T23:59:59Z",
    "subTotal": 15000.00,
    "taxAmount": 2400.00,
    "totalAmount": 17400.00,
    "status": "Sent",
    "tripCount": 12
  }
]
```

### GET /api/invoice/{id}

Get invoice details by ID.

**Response:**
```json
{
  "id": 1,
  "invoiceNumber": "INV-202412-0001",
  "type": "Final",
  "companyId": 1,
  "companyName": "ABC Ambulance Services",
  "invoiceDate": "2024-12-02T08:00:00Z",
  "periodStart": "2024-11-25T00:00:00Z",
  "periodEnd": "2024-12-01T23:59:59Z",
  "subTotal": 15000.00,
  "taxAmount": 2400.00,
  "totalAmount": 17400.00,
  "status": "Sent",
  "paidDate": null,
  "sentDate": "2024-12-02T08:00:00Z",
  "notes": "Monthly invoice for November 2024",
  "tripCount": 12,
  "trips": [
    {
      "id": 1,
      "tripId": 101,
      "tripName": "Emergency Transport",
      "scheduledStartTime": "2024-11-25T10:00:00Z",
      "actualStartTime": "2024-11-25T10:05:00Z",
      "actualEndTime": "2024-11-25T10:45:00Z",
      "status": "Completed",
      "fromLocationName": "City Hospital",
      "toLocationName": "Regional Medical Center",
      "vehicleName": "AMB-001",
      "driverName": "John Doe",
      "basePrice": 1200.00,
      "taxAmount": 192.00,
      "totalPrice": 1392.00
    }
  ]
}
```

### POST /api/invoice/preview

Preview invoice before creation.

**Request Body:**
```json
{
  "companyId": 1,
  "type": "Final",
  "periodStart": "2024-11-25T00:00:00Z",
  "periodEnd": "2024-12-01T23:59:59Z",
  "notes": "Monthly invoice"
}
```

**Response:**
```json
{
  "companyId": 1,
  "companyName": "ABC Ambulance Services",
  "periodStart": "2024-11-25T00:00:00Z",
  "periodEnd": "2024-12-01T23:59:59Z",
  "trips": [...],
  "subTotal": 15000.00,
  "taxAmount": 2400.00,
  "totalAmount": 17400.00,
  "tripCount": 12
}
```

### POST /api/invoice

Create a new invoice.

**Request Body:**
```json
{
  "companyId": 1,
  "type": "Final",
  "periodStart": "2024-11-25T00:00:00Z",
  "periodEnd": "2024-12-01T23:59:59Z",
  "notes": "Monthly invoice for November 2024"
}
```

**Response:** Returns created invoice DTO

### POST /api/invoice/{id}/mark-paid

Mark invoice as paid.

**Request Body:**
```json
{
  "paidDate": "2024-12-02T10:00:00Z"
}
```

**Response:** Returns updated invoice DTO

### GET /api/invoice/{id}/download/pdf

Download invoice as PDF.

**Response:** PDF file

### GET /api/invoice/{id}/download/excel

Download invoice as Excel.

**Response:** Excel file (.xlsx)

### GET /api/invoice/{id}/download/both

Download both PDF and Excel as ZIP.

**Response:** ZIP file containing both formats

## Database Schema

### invoices Table

| Column | Type | Description |
|--------|------|-------------|
| Id | int | Primary key |
| InvoiceNumber | varchar(50) | Unique invoice number |
| Type | int | Invoice type (0=Proforma, 1=Final) |
| CompanyId | int | Foreign key to companies |
| InvoiceDate | timestamp | Invoice creation date |
| PeriodStart | timestamp | Billing period start |
| PeriodEnd | timestamp | Billing period end |
| SubTotal | decimal(18,2) | Sum of base prices |
| TaxAmount | decimal(18,2) | Sum of taxes |
| TotalAmount | decimal(18,2) | Total amount |
| Status | int | Status (0=Draft, 1=Sent, 2=Paid, 3=Cancelled) |
| PaidDate | timestamp | Payment date (nullable) |
| SentDate | timestamp | Send date (nullable) |
| Notes | varchar(1000) | Optional notes |
| PdfPath | varchar(500) | Path to PDF file |
| ExcelPath | varchar(500) | Path to Excel file |
| CreatedAt | timestamp | Record creation |
| UpdatedAt | timestamp | Last update |
| DeletedAt | timestamp | Soft delete |

### invoice_trips Table

| Column | Type | Description |
|--------|------|-------------|
| Id | int | Primary key |
| InvoiceId | int | Foreign key to invoices |
| TripId | int | Foreign key to trips |
| BasePrice | decimal(18,2) | Trip base price |
| TaxAmount | decimal(18,2) | Trip tax amount |
| TotalPrice | decimal(18,2) | Trip total price |
| CreatedAt | timestamp | Record creation |
| UpdatedAt | timestamp | Last update |
| DeletedAt | timestamp | Soft delete |

### trips Table (Updated)

Added fields:
- `IsPaid` (boolean) - Payment status
- `PaidDate` (timestamp) - Payment date

## Invoice Numbering

Format: `{PREFIX}-{YYYYMM}-{SEQUENCE}`

- **PREFIX**: `PRO` for Proforma, `INV` for Final
- **YYYYMM**: Year and month (e.g., 202412)
- **SEQUENCE**: 4-digit sequential number (e.g., 0001)

Examples:
- `PRO-202412-0001` - First proforma invoice in December 2024
- `INV-202412-0001` - First final invoice in December 2024

Sequence resets monthly for each invoice type.

## PDF Invoice Layout

The PDF invoice includes:

### Header
- Company logo and name
- Invoice type (PROFORMA/FINAL)
- Invoice number
- Invoice date
- Status badge

### Bill To Section
- Company name
- Address
- Email
- Phone

### Invoice Period
- Period start date
- Period end date

### Trip Details Table
- Trip number
- Trip name
- Route (from → to)
- Vehicle
- Driver
- Base price
- Tax
- Total

### Summary
- Subtotal
- Tax amount
- **Total amount** (highlighted)

### Footer
- Payment terms
- Notes (if any)
- Page numbers

## Excel Invoice Layout

The Excel file includes:

### Header Section
- Company branding
- Invoice type and number
- Invoice date and status
- Company details
- Billing period

### Trip Details Table
Columns:
1. # (sequence)
2. Trip Name
3. From Location
4. To Location
5. Vehicle
6. Driver
7. Date
8. Base Price
9. Tax
10. Total

### Summary Section
- Subtotal
- Tax
- **Total** (bold)

## Usage Examples

### Create Weekly Invoice

```csharp
var dto = new CreateInvoiceDto
{
    CompanyId = 1,
    Type = "Final",
    PeriodStart = DateTime.Today.AddDays(-7),
    PeriodEnd = DateTime.Today,
    Notes = "Weekly invoice"
};

var invoice = await invoiceService.CreateInvoiceAsync(dto);
```

### Create Monthly Invoice

```csharp
var startOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

var dto = new CreateInvoiceDto
{
    CompanyId = 1,
    Type = "Final",
    PeriodStart = startOfMonth,
    PeriodEnd = endOfMonth,
    Notes = $"Monthly invoice for {startOfMonth:MMMM yyyy}"
};

var invoice = await invoiceService.CreateInvoiceAsync(dto);
```

### Filter Unpaid Invoices

```csharp
var filter = new InvoiceFilterDto
{
    Status = "Sent" // Not yet paid
};

var unpaidInvoices = await invoiceService.GetInvoicesAsync(filter);
```

## Security & Permissions

- **Admin Role**: Full access to all invoice operations
- **Dispatcher Role**: Full access to all invoice operations
- **Other Roles**: No access to invoice module

All endpoints require authentication via JWT Bearer token.

## File Storage

Invoice files are stored in:
```
/wwwroot/invoices/
├── PRO-202412-0001.pdf
├── PRO-202412-0001.xlsx
├── INV-202412-0001.pdf
└── INV-202412-0001.xlsx
```

Files are accessible via:
- `/invoices/PRO-202412-0001.pdf`
- `/invoices/PRO-202412-0001.xlsx`

## Best Practices

1. **Preview Before Creating** - Always preview to verify trip selection
2. **Use Appropriate Type** - Proforma for estimates, Final for billing
3. **Regular Invoicing** - Create invoices weekly or monthly
4. **Verify Completeness** - Ensure all trips are completed before invoicing
5. **Mark Paid Promptly** - Update payment status when received
6. **Archive Files** - Keep PDF/Excel files for records
7. **Add Notes** - Include relevant information in notes field

## Troubleshooting

### No Trips Found
- Ensure trips are marked as "Completed"
- Verify trips are not already paid
- Check date range includes trip completion dates
- Confirm trips belong to selected company

### Invoice Not Generating
- Check QuestPDF license (Community license set)
- Verify wwwroot/invoices directory exists
- Check file permissions
- Review server logs for errors

### Payment Not Updating Trips
- Verify invoice contains trips
- Check database transaction completion
- Ensure trips are not in another paid invoice

## Future Enhancements

- Email invoice delivery
- Recurring invoice schedules
- Invoice templates customization
- Multi-currency support
- Payment gateway integration
- Invoice approval workflow
- Credit notes and refunds
- Invoice reminders
- Batch invoice generation
- Custom tax rates per company

## Related Documentation

- [API Documentation](./API_DOCUMENTATION.md)
- [Trip Management](./TRIP_MODULE_SUMMARY.md)
- [README](./README.md)
- [Changelog](./changelog.md)

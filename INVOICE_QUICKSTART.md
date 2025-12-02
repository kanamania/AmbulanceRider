# Invoice System Quick Start Guide

## Prerequisites

1. Ensure the application is running
2. Have at least one company with completed trips
3. Login as Admin or Dispatcher

## Step-by-Step Testing

### 1. Apply Database Migration

```bash
# Navigate to project root
cd D:\Projects\AmbulanceRider

# Apply migration (if using Docker)
docker-compose exec api dotnet ef database update

# Or if running locally
cd AmbulanceRider.API
dotnet ef database update
```

### 2. Verify Trip Data

Ensure you have:
- At least one company in the system
- Vehicles assigned to that company
- Completed trips (status = Completed)
- Trips with pricing information (BasePrice, TaxAmount, TotalPrice)
- Trips that are not yet paid (IsPaid = false)

### 3. Access Invoice Module

1. Login to the application
2. Navigate to **Invoices** from the menu
3. You should see the invoice list page

### 4. Create Your First Invoice

#### Navigate to Create Page

1. Click **"Create Invoice"** button (top right)
2. You'll be taken to the invoice creation page

#### Select Date Range

1. Choose a range type:
   - **This Week** - Current week (Sunday to Saturday)
   - **This Month** - Current calendar month
   - **Custom** - Set your own dates
2. Select **Invoice Type** (Proforma or Final)
3. Optionally add **Notes**

#### Find Companies with Trips

1. Click **"Find Companies"** button
2. System will search for all companies with:
   - Completed trips
   - Unpaid trips
   - Trips within the selected date range
3. Results show for each company:
   - Company name
   - Number of trips
   - Subtotal amount
   - Tax amount
   - Total amount

#### Create Invoice

1. Review the company list and amounts
2. Click **"Create Invoice"** for the desired company
3. You'll be redirected to the invoice detail page
4. Invoice is created with status "Draft"

### 5. View Invoice Details

1. Click on the invoice number or "View" button
2. You'll see:
   - Invoice header with number and status
   - Company details
   - Invoice summary (trips, amounts)
   - Complete trip breakdown table
   - Download options

### 6. Generate Invoice Files

From the invoice detail page:

1. Click **"Download PDF"** - Downloads professional PDF invoice
2. Click **"Download Excel"** - Downloads detailed Excel spreadsheet
3. Click **"Download Both"** - Downloads ZIP file with both formats

Files are saved to: `/wwwroot/invoices/`

### 7. Mark Invoice as Paid

1. From invoice detail or list page
2. Click **"Mark as Paid"** button
3. Confirm the payment date (defaults to now)
4. System will:
   - Update invoice status to "Paid"
   - Set invoice PaidDate
   - Mark all trips in the invoice as paid (IsPaid = true)
   - Set PaidDate on all trips

### 8. Filter Invoices

On the invoice list page, use filters:

- **Company** - Filter by specific company
- **Type** - Proforma or Final
- **Status** - Draft, Sent, Paid, or Cancelled
- **Start Date** - Invoice date from
- **End Date** - Invoice date to

Click the search button to apply filters.

## API Testing with Swagger

### 1. Access Swagger UI

Navigate to: `http://localhost:5000` (or your API URL)

### 2. Authenticate

1. Click **"Authorize"** button
2. Login via `/api/auth/login` endpoint
3. Copy the `accessToken` from response
4. Enter: `Bearer {your-token}`
5. Click **"Authorize"**

### 3. Test Invoice Endpoints

#### Preview Invoice
```
POST /api/invoice/preview
{
  "companyId": 1,
  "type": "Final",
  "periodStart": "2024-11-25T00:00:00Z",
  "periodEnd": "2024-12-02T23:59:59Z",
  "notes": "Test invoice"
}
```

#### Create Invoice
```
POST /api/invoice
{
  "companyId": 1,
  "type": "Final",
  "periodStart": "2024-11-25T00:00:00Z",
  "periodEnd": "2024-12-02T23:59:59Z",
  "notes": "Test invoice"
}
```

#### Get All Invoices
```
GET /api/invoice
```

#### Get Invoice by ID
```
GET /api/invoice/1
```

#### Mark as Paid
```
POST /api/invoice/1/mark-paid
{
  "paidDate": "2024-12-02T10:00:00Z"
}
```

#### Download PDF
```
GET /api/invoice/1/download/pdf
```

#### Download Excel
```
GET /api/invoice/1/download/excel
```

#### Download Both
```
GET /api/invoice/1/download/both
```

## Testing with cURL

### Preview Invoice
```bash
curl -X POST "http://localhost:5000/api/invoice/preview" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "companyId": 1,
    "type": "Final",
    "periodStart": "2024-11-25T00:00:00Z",
    "periodEnd": "2024-12-02T23:59:59Z"
  }'
```

### Create Invoice
```bash
curl -X POST "http://localhost:5000/api/invoice" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "companyId": 1,
    "type": "Final",
    "periodStart": "2024-11-25T00:00:00Z",
    "periodEnd": "2024-12-02T23:59:59Z",
    "notes": "Monthly invoice"
  }'
```

### Get Invoices
```bash
curl -X GET "http://localhost:5000/api/invoice" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

### Download PDF
```bash
curl -X GET "http://localhost:5000/api/invoice/1/download/pdf" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  --output invoice.pdf
```

## Common Test Scenarios

### Scenario 1: Weekly Invoice

```json
{
  "companyId": 1,
  "type": "Final",
  "periodStart": "2024-11-25T00:00:00Z",
  "periodEnd": "2024-12-01T23:59:59Z",
  "notes": "Weekly invoice - Week 48"
}
```

### Scenario 2: Monthly Invoice

```json
{
  "companyId": 1,
  "type": "Final",
  "periodStart": "2024-11-01T00:00:00Z",
  "periodEnd": "2024-11-30T23:59:59Z",
  "notes": "Monthly invoice - November 2024"
}
```

### Scenario 3: Custom Range

```json
{
  "companyId": 1,
  "type": "Proforma",
  "periodStart": "2024-11-15T00:00:00Z",
  "periodEnd": "2024-11-22T23:59:59Z",
  "notes": "Custom period invoice"
}
```

## Verification Checklist

- [ ] Invoice created successfully
- [ ] Invoice number generated correctly (PRO-YYYYMM-XXXX or INV-YYYYMM-XXXX)
- [ ] All completed unpaid trips included
- [ ] Amounts calculated correctly (subtotal, tax, total)
- [ ] PDF generated and downloadable
- [ ] Excel generated and downloadable
- [ ] ZIP file contains both PDF and Excel
- [ ] Invoice status updates correctly
- [ ] Marking as paid updates invoice
- [ ] Marking as paid updates all trips
- [ ] Filters work correctly
- [ ] Invoice detail page displays correctly

## Troubleshooting

### No Trips Found

**Problem:** Preview shows 0 trips

**Solutions:**
1. Verify trips exist for the company
2. Check trips are marked as "Completed"
3. Ensure trips are not already paid
4. Verify date range includes trip completion dates
5. Check trip ActualEndTime is within the period

### PDF Generation Error

**Problem:** PDF download fails

**Solutions:**
1. Check QuestPDF package is installed
2. Verify wwwroot/invoices directory exists
3. Check file system permissions
4. Review application logs

### Excel Generation Error

**Problem:** Excel download fails

**Solutions:**
1. Check ClosedXML package is installed
2. Verify wwwroot/invoices directory exists
3. Check file system permissions
4. Review application logs

### Invoice Number Duplicate

**Problem:** Duplicate invoice number error

**Solutions:**
1. Check database for existing invoice numbers
2. Verify unique index on InvoiceNumber column
3. Review invoice number generation logic

## Next Steps

After successful testing:

1. **Create Real Invoices** - Start billing companies
2. **Track Payments** - Mark invoices as paid when received
3. **Archive Files** - Backup PDF/Excel files regularly
4. **Monitor Unpaid** - Filter by "Sent" status to see unpaid invoices
5. **Generate Reports** - Use invoice data for financial reporting

## Support

For issues or questions:
- Check [INVOICE_SYSTEM.md](./INVOICE_SYSTEM.md) for detailed documentation
- Review [API_DOCUMENTATION.md](./API_DOCUMENTATION.md) for API details
- Check application logs for error messages
- Verify database migration was applied successfully

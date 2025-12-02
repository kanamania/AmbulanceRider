# Invoice System Workflow

## User Interface Flow

### 1. Invoice List Page (`/invoices`)

**Purpose:** View and manage existing invoices

**Features:**
- List of all invoices with filtering
- Quick actions (view, mark paid, download)
- "Create Invoice" button (top right)

**Layout:**
```
┌─────────────────────────────────────────────────────────┐
│  📄 Invoices                    [+ Create Invoice]      │
├─────────────────────────────────────────────────────────┤
│  Filters:                                               │
│  [Company ▼] [Type ▼] [Status ▼] [Start] [End] [🔍]   │
├─────────────────────────────────────────────────────────┤
│  Invoice #  │ Type │ Company │ Period │ Amount │ Actions│
│  INV-001    │Final │ ABC Co  │ Nov 24 │ 17,400 │ [👁][✓]│
│  PRO-002    │Profo │ XYZ Ltd │ Dec 24 │ 12,500 │ [👁][✓]│
└─────────────────────────────────────────────────────────┘
```

---

### 2. Create Invoice Page (`/invoices/create`)

**Purpose:** Smart invoice creation with company discovery

**Features:**
- Date range selection (Week/Month/Custom)
- Invoice type selection
- Automatic company discovery
- Quick summary per company
- One-click invoice creation

**Layout:**
```
┌─────────────────────────────────────────────────────────────────┐
│  📝 Create Invoice                    [← Back to Invoices]      │
├──────────────────────┬──────────────────────────────────────────┤
│  INVOICE SETTINGS    │  COMPANIES WITH UNPAID TRIPS             │
│                      │                                          │
│  Invoice Type:       │  Company      │Trips│ Sub  │ Tax │Total │
│  [Final ▼]          │  ABC Ambulance│ 12  │15,000│2,400│17,400│
│                      │               │     │      │     │[Create]│
│  Date Range:         │  ─────────────────────────────────────── │
│  [Week][Month][Custom]│ XYZ Medical   │  8  │10,000│1,600│11,600│
│                      │               │     │      │     │[Create]│
│  Period Start:       │  ─────────────────────────────────────── │
│  [2024-11-25]       │  Total        │ 20  │25,000│4,000│29,000│
│                      │                                          │
│  Period End:         │  ℹ Period: 25 Nov - 01 Dec 2024         │
│  [2024-12-01]       │  Only companies with completed,          │
│                      │  unpaid trips are shown.                 │
│  Notes:              │                                          │
│  [Optional notes]    │                                          │
│                      │                                          │
│  [🔍 Find Companies] │                                          │
└──────────────────────┴──────────────────────────────────────────┘
```

---

### 3. Invoice Detail Page (`/invoices/{id}`)

**Purpose:** View complete invoice details and download files

**Features:**
- Full invoice information
- Trip breakdown table
- Download options (PDF, Excel, ZIP)
- Mark as paid functionality

**Layout:**
```
┌─────────────────────────────────────────────────────────┐
│  [← Back]    Invoice INV-202412-0001    [Paid ✓]       │
│                                                         │
│  [Download PDF] [Download Excel] [Download Both]       │
├─────────────────────────────────────────────────────────┤
│  Invoice Details          │  Company Details            │
│  Number: INV-202412-0001  │  ABC Ambulance Services     │
│  Type: Final              │  info@abc.com               │
│  Date: 02 Dec 2024        │  +254 700 000 000          │
│  Period: 25 Nov - 01 Dec  │                             │
├─────────────────────────────────────────────────────────┤
│  Trip Breakdown                                         │
│  # │ Trip Name │ Route │ Vehicle │ Driver │ Total      │
│  1 │ Emergency │ A → B │ AMB-001 │ John   │ 1,392.00  │
│  2 │ Transfer  │ C → D │ AMB-002 │ Jane   │ 1,200.00  │
│  ...                                                    │
├─────────────────────────────────────────────────────────┤
│  Summary                                                │
│  Subtotal: KES 15,000.00                               │
│  Tax:      KES  2,400.00                               │
│  Total:    KES 17,400.00                               │
└─────────────────────────────────────────────────────────┘
```

---

## Workflow Steps

### Step 1: Access Invoice Creation
```
User clicks "Create Invoice" → Navigate to /invoices/create
```

### Step 2: Configure Invoice Settings
```
1. Select Invoice Type (Proforma/Final)
2. Choose Date Range:
   - This Week (automatic dates)
   - This Month (automatic dates)
   - Custom (manual date selection)
3. Add optional notes
```

### Step 3: Find Companies
```
Click "Find Companies" button
   ↓
System searches for companies with:
   - Completed trips
   - Unpaid trips (IsPaid = false)
   - Trips in selected date range
   ↓
Display results table showing:
   - Company name
   - Trip count
   - Subtotal
   - Tax amount
   - Total amount
   - Create button
```

### Step 4: Create Invoice
```
User clicks "Create Invoice" for a company
   ↓
System creates invoice with:
   - Selected invoice type
   - Selected date range
   - All unpaid trips for that company
   - Optional notes
   ↓
Redirect to invoice detail page
```

### Step 5: Download Files
```
From invoice detail page:
   - Click "Download PDF" → Professional PDF invoice
   - Click "Download Excel" → Detailed spreadsheet
   - Click "Download Both" → ZIP with both files
```

### Step 6: Mark as Paid
```
Click "Mark as Paid" button
   ↓
System updates:
   - Invoice status → Paid
   - Invoice PaidDate → Current date
   - All trips IsPaid → true
   - All trips PaidDate → Current date
```

---

## Date Range Presets

### This Week
- Start: Sunday of current week
- End: Saturday of current week
- Example: 26 Nov 2024 - 02 Dec 2024

### This Month
- Start: 1st day of current month
- End: Last day of current month
- Example: 01 Dec 2024 - 31 Dec 2024

### Custom
- User selects both start and end dates
- No automatic calculation
- Full flexibility

---

## Company Discovery Logic

### Query Criteria
```sql
SELECT DISTINCT companies.*
FROM companies
JOIN vehicles ON vehicles.CompanyId = companies.Id
JOIN trips ON trips.VehicleId = vehicles.Id
WHERE trips.Status = 'Completed'
  AND trips.IsPaid = false
  AND trips.ActualEndTime >= @PeriodStart
  AND trips.ActualEndTime <= @PeriodEnd
```

### Summary Calculation
For each company:
```
TripCount = COUNT(trips)
SubTotal = SUM(trips.BasePrice)
TaxAmount = SUM(trips.TaxAmount)
TotalAmount = SUM(trips.TotalPrice)
```

### Display Rules
- Only show companies with TripCount > 0
- Sort by company name
- Show grand totals at bottom

---

## User Experience Benefits

### 1. **Smart Discovery**
- No need to know which companies have trips
- System automatically finds eligible companies
- Shows quick summary before creation

### 2. **Quick Actions**
- One-click range selection (Week/Month)
- One-click invoice creation per company
- Batch visibility of all eligible companies

### 3. **Clear Information**
- See trip counts before creating
- See amounts before creating
- Know exactly what will be invoiced

### 4. **Flexible Filtering**
- Use presets for common scenarios
- Use custom dates for special cases
- Switch between invoice types easily

### 5. **Efficient Workflow**
```
Old: Select company → Set dates → Preview → Create
New: Set dates → See all companies → Create
```

---

## Technical Implementation

### Frontend Components

**Invoices.razor** (List Page)
- Displays existing invoices
- Filtering and search
- Navigation to create page

**CreateInvoice.razor** (Creation Page)
- Date range selection
- Company discovery
- Quick summary display
- Invoice creation

**InvoiceDetail.razor** (Detail Page)
- Full invoice view
- Download options
- Payment marking

### API Endpoints Used

**GET /api/companies**
- Fetch all companies

**POST /api/invoice/preview**
- Preview invoice for a company
- Used to get trip counts and amounts

**POST /api/invoice**
- Create the actual invoice

### State Management

```csharp
// Page state
string invoiceType = "Final"
string rangeType = "month"
DateTime periodStart
DateTime periodEnd
string notes

// Data state
List<InvoicePreviewDto> companiesWithTrips
bool loading
bool searchPerformed
```

---

## Example Scenarios

### Scenario 1: Weekly Billing
```
1. Navigate to Create Invoice
2. Click "This Week"
3. Click "Find Companies"
4. See 3 companies with trips
5. Create invoice for each company
6. Download PDFs for email
```

### Scenario 2: Monthly Billing
```
1. Navigate to Create Invoice
2. Click "This Month"
3. Select "Final" invoice type
4. Click "Find Companies"
5. See 5 companies with trips
6. Create invoices for all
7. Mark as paid when payment received
```

### Scenario 3: Custom Period
```
1. Navigate to Create Invoice
2. Click "Custom"
3. Set start: 15 Nov
4. Set end: 22 Nov
5. Click "Find Companies"
6. Create proforma invoices
7. Send to companies for approval
```

---

## Best Practices

### For Users
1. **Use Presets** - Week/Month for regular billing
2. **Review Amounts** - Check totals before creating
3. **Add Notes** - Include period description
4. **Download Immediately** - Get files right after creation
5. **Mark Paid Promptly** - Update status when payment received

### For Administrators
1. **Regular Schedule** - Bill weekly or monthly
2. **Verify Completeness** - Ensure all trips are completed
3. **Check Pricing** - Verify trip prices before invoicing
4. **Archive Files** - Keep PDF/Excel for records
5. **Monitor Unpaid** - Follow up on outstanding invoices

---

## Troubleshooting

### No Companies Found
**Problem:** "Find Companies" returns empty list

**Solutions:**
1. Verify trips exist in date range
2. Check trips are marked "Completed"
3. Ensure trips are not already paid
4. Confirm trips have pricing data
5. Try different date range

### Wrong Amounts
**Problem:** Amounts don't match expectations

**Solutions:**
1. Check trip pricing (BasePrice, TaxAmount)
2. Verify all trips are included
3. Review date range selection
4. Confirm trip completion dates

### Can't Create Invoice
**Problem:** Create button doesn't work

**Solutions:**
1. Check network connection
2. Verify authentication token
3. Review browser console for errors
4. Ensure company has trips

---

## Future Enhancements

### Planned Features
- [ ] Bulk invoice creation (all companies at once)
- [ ] Email delivery from creation page
- [ ] Invoice templates selection
- [ ] Recurring invoice schedules
- [ ] Draft saving before creation
- [ ] Invoice preview before creation
- [ ] Export to accounting software

### Under Consideration
- [ ] Multi-currency support
- [ ] Discount application
- [ ] Payment terms customization
- [ ] Invoice approval workflow
- [ ] Automated reminders
- [ ] Payment gateway integration

---

## Summary

The invoice creation workflow provides:
- ✅ **Smart Discovery** - Automatic company finding
- ✅ **Quick Summary** - See amounts before creating
- ✅ **Flexible Ranges** - Week, Month, or Custom
- ✅ **One-Click Creation** - Create per company easily
- ✅ **Clear Information** - Trip counts and totals visible
- ✅ **Efficient Process** - Fewer steps, better UX

This design makes invoice creation fast, accurate, and user-friendly!

# End User Documentation

**Last Updated:** 2025-12-10T04:12:00+03:00  
**Version:** 0.0.24

---

## Table of Contents

1. [Getting Started](#getting-started)
2. [User Roles & Permissions](#user-roles--permissions)
3. [Dashboard](#dashboard)
4. [Trip Management](#trip-management)
5. [Invoice Management](#invoice-management)
6. [Vehicle Management](#vehicle-management)
7. [Pricing Management](#pricing-management)
8. [User Management](#user-management)
9. [Company Management](#company-management)
10. [Telemetry & Analytics](#telemetry--analytics)
11. [Profile Management](#profile-management)
12. [Troubleshooting](#troubleshooting)

---

## Getting Started

### System Requirements
- Modern web browser (Chrome, Firefox, Edge, Safari)
- Stable internet connection
- Screen resolution: 1280x720 minimum

### Accessing the Application
1. Navigate to the application URL
2. Enter your email and password
3. Click **Login**

### First-Time Setup
1. Contact your administrator for account credentials
2. Login with provided credentials
3. Update your profile information
4. Change your password (recommended)

---

## User Roles & Permissions

| Role | Description | Key Permissions |
|------|-------------|-----------------|
| **Admin** | Full system access | All operations, user management, system settings |
| **Dispatcher** | Trip coordination | Create/manage trips, approve trips, view all trips |
| **Driver** | Trip execution | View assigned trips, update trip status, complete trips |
| **User** | Basic access | Create trips, view own trips, update own trips |

### Permission Matrix

| Action | Admin | Dispatcher | Driver | User |
|--------|-------|------------|--------|------|
| View Dashboard | ✅ | ✅ | ✅ | ✅ |
| Create Trip | ✅ | ✅ | ✅ | ✅ |
| Approve/Reject Trip | ✅ | ✅ | ❌ | ❌ |
| Complete Trip | ✅ | ✅ | ✅ | ✅ |
| Cancel Trip | ✅ | ✅ | Own only | Own only |
| Force Complete | ✅ | ✅ | ❌ | ❌ |
| Manage Users | ✅ | ❌ | ❌ | ❌ |
| Manage Vehicles | ✅ | ✅ | ❌ | ❌ |
| Manage Pricing | ✅ | ❌ | ❌ | ❌ |
| Create Invoices | ✅ | ✅ | ❌ | ❌ |
| View Telemetry | ✅ | ✅ | Own only | Own only |

---

## Dashboard

The dashboard provides an overview of system activity and key metrics.

### Dashboard Widgets

| Widget | Description |
|--------|-------------|
| **Active Trips** | Count of trips currently in progress |
| **Pending Trips** | Trips awaiting approval |
| **Completed Today** | Trips completed in the last 24 hours |
| **Total Revenue** | Sum of completed trip values |
| **Recent Activity** | Timeline of recent system events |
| **Trip Status Chart** | Visual breakdown by status |

### Company Dashboard (Admin/Dispatcher)
- View statistics filtered by company
- Compare performance across companies
- Monitor trip completion rates

---

## Trip Management

### Trip Status Workflow

```
Pending → Approved → InProgress → Completed
    ↓         ↓           ↓
 Rejected  Cancelled   Cancelled
```

### Creating a Trip

1. Navigate to **Trips** → **Create Trip**
2. Fill in required fields:
   - **Trip Name**: Descriptive name
   - **Trip Type**: Select category (Emergency, Routine, etc.)
   - **From Location**: Click map or enter coordinates
   - **To Location**: Click map or enter coordinates
   - **Vehicle**: Select from available vehicles
   - **Driver**: Auto-filled based on vehicle or select manually
   - **Scheduled Time**: When the trip should start
3. Add optional dimensions for pricing:
   - Weight, Height, Length, Width
4. Click **Create Trip**

### Managing Trip Status

| Current Status | Available Actions | Who Can Perform |
|----------------|-------------------|-----------------|
| Pending | Approve, Reject, Cancel | Admin, Dispatcher |
| Approved | Start, Cancel | Driver, Admin, Dispatcher |
| InProgress | Complete, Cancel | Driver, Admin, Dispatcher |
| Completed | - | - |
| Cancelled | Reactivate | Admin only |
| Rejected | Reactivate | Admin only |

### Completing a Trip
1. Open the trip details
2. Click **Complete Trip**
3. Add optional completion notes
4. Confirm completion

### Cancelling a Trip
1. Open the trip details
2. Click **Cancel Trip**
3. Enter cancellation reason (required for Admin/Dispatcher)
4. Confirm cancellation

### Viewing Trip History
- Each trip maintains a complete audit trail
- View the **Status History** section to see:
  - Who changed the status
  - When it was changed
  - Notes/reasons provided
  - User role at time of change

### Trip Types & Custom Attributes
Trips can be categorized with custom attributes:

| Trip Type | Description | Custom Fields |
|-----------|-------------|---------------|
| Emergency | Urgent medical transport | Priority Level, Patient Condition |
| Routine | Scheduled transport | Appointment Time, Return Trip |
| Transfer | Inter-facility transfer | Origin Facility, Destination Facility |
| Delivery | Package/supply delivery | Package Type, Handling Instructions |

### Managing Select Options for Attributes
When creating or editing a trip type attribute with DataType "Select":
1. Click **Add Attribute** or edit an existing attribute
2. Select **Select (dropdown)** as the Data Type
3. Use the options management panel:
   - Type an option value and click **Add** or press Enter
   - Edit existing options inline by modifying the text field
   - Remove options using the trash button
4. Options are numbered for easy reference
5. Save the attribute to persist changes

---

## Invoice Management

### Creating an Invoice

1. Navigate to **Invoices** → **Create Invoice**
2. Select date range:
   - This Week
   - This Month
   - Custom Range
3. Select company (auto-filters to companies with unpaid trips)
4. Review trip summary:
   - Number of trips
   - Subtotal
   - Tax amount
   - Total
5. Select invoice type:
   - **Proforma**: Preliminary invoice
   - **Final**: Official invoice
6. Click **Generate Invoice**

### Invoice Actions

| Action | Description |
|--------|-------------|
| **View** | Open invoice details |
| **Download PDF** | Download professional PDF invoice |
| **Download Excel** | Download detailed spreadsheet |
| **Download Both** | Download ZIP with PDF and Excel |
| **Mark as Paid** | Update payment status |
| **Send Email** | Email invoice to company |

### Invoice Status

| Status | Description |
|--------|-------------|
| Draft | Invoice created, not finalized |
| Sent | Invoice sent to customer |
| Paid | Payment received |
| Overdue | Past due date, unpaid |
| Cancelled | Invoice voided |

---

## Vehicle Management

### Adding a Vehicle

1. Navigate to **Vehicles** → **Add Vehicle**
2. Enter vehicle details:
   - Name/Identifier
   - Plate Number
   - Vehicle Type
   - Image (optional)
3. Assign drivers (optional)
4. Click **Save**

### Vehicle-Driver Assignment
- Each vehicle can have multiple assigned drivers
- When creating a trip, selecting a vehicle auto-fills available drivers
- Drivers can be assigned to multiple vehicles

### Vehicle Types
- Ambulance
- Medical Van
- Delivery Vehicle
- Custom types (Admin configurable)

---

## Pricing Management

### Pricing Matrix

Pricing is calculated based on package dimensions:

| Size Category | Dimensions (cm) | Base Price |
|---------------|-----------------|------------|
| Small Parcel | 28×34×37 | Configured |
| Medium Box | 30×47×49 | Configured |
| Large Box | 38×47×65 | Configured |
| Extra Large | 60×61×70 | Configured |

### Region-Based Pricing
- Different regions can have different pricing
- Default pricing applies when no region-specific pricing exists
- Admin can configure region-specific rates

### Managing Pricing (Admin Only)

1. Navigate to **Settings** → **Pricing**
2. View existing pricing matrices
3. To add new pricing:
   - Click **Add Pricing**
   - Set dimension ranges
   - Set base price and tax rate
   - Select region (optional)
   - Mark as default (optional)
4. Click **Save**

### Managing Regions

1. From Pricing page, click **Manage Regions**
2. Add/Edit regions with:
   - Name
   - Code
   - Description
   - Active status
   - Default flag

---

## User Management

### Creating a User (Admin Only)

1. Navigate to **Users** → **Add User**
2. Enter user details:
   - First Name, Last Name
   - Email (must be unique)
   - Phone Number
   - Password
   - Role(s)
   - Company (optional)
3. Click **Create**

### Editing a User

1. Navigate to **Users**
2. Click **Edit** on the user row
3. Update fields as needed
4. Click **Save**

### Deactivating a User
- Users are soft-deleted (not permanently removed)
- Deactivated users cannot login
- Historical data is preserved

---

## Company Management

### Company Features
- Multi-tenant support
- Company-specific dashboards
- Trip filtering by company
- Invoice generation per company

### Managing Companies (Admin Only)

1. Navigate to **Companies**
2. View company directory
3. Click company card to view details
4. Edit company information as needed

---

## Telemetry & Analytics

### What is Tracked

| Data Type | Description | Privacy |
|-----------|-------------|---------|
| Device Info | OS, browser, device type | Anonymous |
| GPS Location | Coordinates with accuracy | User consent required |
| Network Status | Connection type, online status | Anonymous |
| Battery Level | Charge level, charging state | Anonymous |
| User Events | Login, trip actions | Logged |

### Viewing Telemetry

1. Navigate to **Settings** → **Telemetry**
2. Select date range
3. View:
   - Location history on map
   - Device information
   - Event timeline

### Privacy Controls
- Users can only view their own telemetry data
- Admin/Dispatcher can view all user telemetry
- Location tracking requires browser permission
- Telemetry collection is non-blocking

---

## Profile Management

### Updating Your Profile

1. Click your name in the top navigation
2. Select **Profile**
3. Update:
   - Personal information
   - Contact details
   - Profile picture
4. Click **Save Changes**

### Changing Password

1. Go to Profile
2. Click **Change Password**
3. Enter current password
4. Enter new password (twice)
5. Click **Update Password**

### Password Requirements
- Minimum 8 characters
- At least one uppercase letter
- At least one lowercase letter
- At least one number
- At least one special character

---

## Troubleshooting

### Common Issues

| Issue | Resolution |
|-------|------------|
| Docker build fails with `At least one invalid signature was encountered` | Already addressed in v0.0.24 Dockerfiles. If using older images, switch `/etc/apt/sources.list` entries to `https://deb.debian.org` before `apt-get update`. |

| Issue | Solution |
|-------|----------|
| Cannot login | Check email/password, contact admin if locked out |
| Session expired | Login again, tokens expire after 24 hours |
| Map not loading | Check internet connection, allow location access |
| Invoice download fails | Ensure you're logged in, try again |
| Trip status won't update | Check permissions, refresh page |

### Error Messages

| Error | Meaning | Action |
|-------|---------|--------|
| 401 Unauthorized | Session expired or invalid | Login again |
| 403 Forbidden | Insufficient permissions | Contact admin |
| 404 Not Found | Resource doesn't exist | Check URL/ID |
| 500 Server Error | System error | Try again, contact support |

### Getting Help
- Contact your system administrator
- Check the FAQ section
- Submit a support ticket

---

## Keyboard Shortcuts

| Shortcut | Action |
|----------|--------|
| `Ctrl + N` | New trip (on Trips page) |
| `Ctrl + S` | Save current form |
| `Esc` | Close modal/dialog |
| `Enter` | Submit form |

---

## Mobile Usage

The application is responsive and works on mobile devices:
- Touch-friendly interface
- Swipe gestures for navigation
- GPS location from device
- Camera access for image uploads

### Mobile Limitations
- Some features optimized for desktop
- Large data tables may require scrolling
- Map interactions work best with touch

---

## Glossary

| Term | Definition |
|------|------------|
| **Trip** | A transport request from origin to destination |
| **Dispatch** | Assignment of a trip to a driver/vehicle |
| **Telemetry** | Device and location tracking data |
| **Proforma Invoice** | Preliminary invoice before final billing |
| **Soft Delete** | Data marked as deleted but preserved in database |
| **JWT** | JSON Web Token for authentication |
| **SignalR** | Real-time communication technology |

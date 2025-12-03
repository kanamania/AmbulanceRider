# Changelog

## [0.0.15] - 2025-12-04T01:38:00+03:00

### Added
- Pricing matrix seeding for Dar es Salaam region and default pricing with exact dimensions and pricing values
  - Small Parcel/Box (28x34x37)
  - Medium Box (30x47x49)
  - Large Box (38x47x65)
  - Extra Large Box (60x61x70)

## [0.0.14] - 2025-12-04T01:15:00+03:00

### Added
- Regions management UI integrated into Pricing Matrix page
- "Manage Regions" button in pricing page header
- Modal dialog for regions CRUD operations with inline form
- Real-time region creation, editing, and deletion within pricing page
- Regions list table showing name, code, description, and status
- Visual indicators for default and active/inactive regions
- Form validation for required region fields

### Changed
- Regions can now be managed directly from the pricing page without navigation
- Improved workflow for creating pricing matrices with region selection

## [0.0.13] - 2025-12-04T01:10:00+03:00

### Added
- Region-aware pricing matrix system with default fallback mechanism
- Region model and repository for managing geographical pricing zones
- RegionsController with full CRUD operations (Admin only)
- Default pricing matrix flag to serve as fallback for regions without specific pricing
- Region selection in pricing matrix form with IsDefault checkbox
- Region column in pricing matrices list showing region-specific pricing
- Protection against deleting default pricing matrices and default regions
- Client-side and API-side Region DTOs

### Changed
- Pricing matrices now support region-specific pricing with automatic fallback to default
- Updated PricingMatrix model to include RegionId and IsDefault flag
- Enhanced pricing matrix list to display region information and default badges
- Pricing matrix queries now order by IsDefault flag (default pricing shown first)

## [0.0.12] - 2025-12-04T00:55:00+03:00

### Added
- Pricing Matrix management UI with full CRUD operations
- Pricing Matrix list page with dimension ranges, pricing details, and filters
- Create/Edit Pricing Matrix form with dimension inputs, pricing configuration, and optional filters (Company, Vehicle Type, Trip Type)
- Pricing menu link in main navigation (Admin only)
- PricingController expanded with Create, Update, Delete endpoints (Admin role required)
- Client-side and API-side DTOs for Pricing Matrix operations

## [0.0.11] - 2025-12-04T00:50:00+03:00

### Fixed
- Fixed JSON deserialization error in Blazor client by making PricingMatrixId, BasePrice, TaxAmount, and TotalPrice nullable in client-side TripDto to match API DTOs

## [0.0.10] - 2025-12-02T21:11:00+03:00

### Added
- Seeded baseline driver-to-vehicle assignments to support trip auto-fill scenarios

## [0.0.9] - 2025-12-02T20:44:00+03:00

### Changed
- Restyled companies administration table to match modern dashboard visuals and align with Tailwind patterns

## [0.0.8] - 2025-12-02T20:40:00+03:00

### Fixed
- Prevented null reference when listing users without associated companies by eager loading company data and guarding DTO mapping

## [0.0.7] - 2025-12-02T13:36:00+03:00

### Added
- Companies hub page with Tailwind-styled directory and quick access from main navigation

### Changed
- Moved Trip Types access into Trips page via dedicated Manage Trip Types button to declutter navigation

## [0.0.6] - 2025-12-02T13:27:00+03:00

### Added
- Introduced CompaniesController with secured GET /api/companies endpoint

### Fixed
- Updated Blazor components to inject IApiService interface consistently
- Expanded IApiService contract to cover all ApiService features for DI resolution

## [0.0.5] - 2025-12-02T12:48:00+03:00

### Fixed
- Fixed 500 Internal Server Error on company dashboard stats endpoint when no trips with prices exist
- CompanyStatsService now properly handles empty collections when calculating average trip price
- Fixed DashboardController dependency injection to use ICompanyStatsService interface instead of concrete class
- Fixed JSON deserialization error by making UserDto.CompanyId nullable to handle users without assigned companies

## [0.0.4] - 2025-12-02T12:35:00+03:00

### Changed
- Invoice system temporarily disabled pending proper migration setup (requires dotnet-ef tools)
- Trip IsPaid and PaidDate fields temporarily removed

### Added (Pending Migration)
- Comprehensive invoice system with proforma and final invoice types (code ready, migration pending)
- Dedicated invoice creation page with smart filtering
- Range selection (This Week, This Month, Custom)
- Automatic company discovery with unpaid trips
- Quick summary showing trips count, subtotal, tax, and total per company
- Invoice generation with PDF and Excel export capabilities
- Payment tracking with ability to mark invoices as paid
- Automatic trip payment status update when invoice is marked as paid
- Invoice filtering by company, type, status, and date range
- Invoice detail view with complete trip breakdown
- Download options for PDF, Excel, or both files as ZIP
- Invoice menu item in navigation for Admin and Dispatcher roles
- Database migration for Invoice and InvoiceTrip tables
- IsPaid and PaidDate fields added to Trip model

### Technical Details
- Added QuestPDF library for professional PDF generation
- Added ClosedXML library for Excel spreadsheet generation
- Created InvoiceService with comprehensive business logic
- Created InvoiceController with RESTful API endpoints
- Created Blazor UI components for invoice management (list, create, detail)
- Invoice numbering system with automatic sequential generation
- Smart company filtering showing only those with unpaid trips
- Invoice filtering based on trip creation date and creator's company
- Moved all migrations from Data/Migrations to Migrations folder

## [0.0.3] - 2025-12-02T10:35:00+03:00

### Changed
- Updated UI branding to use the Global Express logo and icon assets.

## [0.0.2] - 2025-12-02

### Fixed
- Redirect signed-out users to the login page when API responses return HTTP 401 across Blazor UI pages.

## [0.0.1] - 2025-11-27

### Added
- Migration to assign Admin role to demo@gmail.com
- Migration to assign User role to all existing users without roles
- Added dotnet-ef tools to API container runtime image

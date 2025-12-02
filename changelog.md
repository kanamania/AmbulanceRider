# Changelog

## [0.0.5] - 2025-12-02T12:48:00+03:00

### Fixed
- Fixed 500 Internal Server Error on company dashboard stats endpoint when no trips with prices exist
- CompanyStatsService now properly handles empty collections when calculating average trip price
- Fixed DashboardController dependency injection to use ICompanyStatsService interface instead of concrete class

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

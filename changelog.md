# Changelog

## [0.0.24] - 2025-12-10T04:12:00+03:00

### Fixed
- Resolved Docker build failures caused by invalid Debian repository signatures by forcing all Docker images (API, Blazor client, root build image) to switch `deb.debian.org` sources to HTTPS when present
- Added guards so builds gracefully skip sed replacements when `/etc/apt/sources.list` does not exist, avoiding runtime errors on slim images
- Ensured curl/libpq installs use `--no-install-recommends` and clean apt lists for reproducible image layers

### DevOps
- Documented the hardened Docker process across README and troubleshooting guides to highlight the new build requirement

## [0.0.23] - 2025-12-07T13:45:00+03:00

### Changed
- Improved select options UI for TripType attributes - replaced JSON textarea with dynamic add/remove option fields
- Select options are now editable inline with numbered list, individual edit/delete buttons per option
- Added "Add" button with Enter key support for quick option entry
- Options are automatically serialized to/from JSON when saving/loading attributes

### UI Improvements
- Better user experience for managing dropdown options in trip type attributes
- Visual feedback with numbered options list
- Consistent UI between Add Attribute and Edit Attribute modals

## [0.0.22] - 2025-12-06T18:55:00+03:00

### Added
- Edit functionality for TripType attributes - all fields including DataType and Options are now editable
- Edit button (pencil icon) next to delete button for each attribute
- Edit Attribute modal with full form for modifying all attribute properties

### Changed
- Attribute list now shows edit and delete buttons in a button group
- Users can now change DataType, Options, Label, Name, Description, Placeholder, Default Value, Required, and Active status

## [0.0.21] - 2025-12-06T18:45:00+03:00

### Added
- PricingMatrix DataType for TripType attributes - dynamically fetches all PricingMatrix entries as dropdown options
- New "Pricing Matrix (dropdown)" option in TripType attribute creation form

### Changed
- TripTypeService now injects IPricingMatrixRepository to populate PricingMatrix options dynamically
- TripType attributes with DataType "PricingMatrix" return options as JSON array with value/label pairs

## [0.0.20] - 2025-12-06T18:30:00+03:00

### Added
- Region detection from OpenStreetMap Nominatim API during trip creation
- `FromRegion` and `ToRegion` fields on Trip model to store detected regions
- `IGeocodingService` and `GeocodingService` for reverse geocoding coordinates to region names
- Region-aware pricing logic: uses regional pricing when from/to regions match and region has active pricing, otherwise falls back to default pricing
- `GetByRegionAndDimensionsAsync` and `GetDefaultByDimensionsAsync` methods in `IPricingMatrixRepository`

### Changed
- `TripService.CreateTripAsync` now fetches regions from OpenStreetMap before saving trip
- Trip creation response now includes `fromRegion` and `toRegion` fields
- Pricing selection logic updated to prioritize regional pricing for same-region trips

### Database
- Added migration `AddTripRegionColumns` with `FromRegion` and `ToRegion` columns on trips table

## [0.0.19] - 2025-12-04T08:56:00+03:00

### Changed
- Invoice PDF: Removed Route and Driver columns from trip table for cleaner layout
- Invoice PDF: Added ge-letterhead.png image before system name in header
- Invoice PDF: Aligned Invoice Period section hard right for better visual balance

### Added
- Copied ge-letterhead.png to API wwwroot for PDF generation access

## [0.0.18] - 2025-12-04T08:45:00+03:00

### Added
- Created `planning-implementation.md` with project roadmap, identified weaknesses, and recommendations
- Created `end-user-documentation.md` with comprehensive user guides for all modules
- Created `testing.md` with testing strategy, manual test procedures, and planned test implementation
- Created `api-specifications.md` with complete API endpoint documentation

### Documentation
- Consolidated documentation strategy into 6 core files
- Identified 45+ redundant .md files for archival
- Added technical debt register with 10 identified items
- Added implementation roadmap with 6 phases

### Identified Weaknesses
- No automated testing (0% coverage)
- Hardcoded secrets in docker-compose.yaml
- Duplicate service registrations in Program.cs
- Documentation sprawl (60+ scattered .md files)
- Missing input validation on some endpoints
- Inconsistent error handling across controllers
- No rate limiting implementation
- No API versioning strategy
- No caching strategy

## [0.0.17] - 2025-12-04T08:30:00+03:00

### Fixed
- Fixed invoice download endpoints returning 401 Unauthorized by implementing authenticated HTTP requests with JavaScript interop for file downloads
- Fixed send-email API URL construction to prevent double `/api/api/` path issue
- Removed trailing slash from production ApiBaseUrl configuration

### Added
- `GetFileBytesAsync` method in ApiService for authenticated file downloads
- JavaScript helper `fileDownload.js` for triggering browser file downloads from byte arrays
- JSRuntime injection in InvoiceDetail.razor and Invoices.razor for download functionality

### Changed
- Invoice download methods now use authenticated HttpClient instead of NavigationManager.NavigateTo
- Download buttons trigger proper file downloads with correct authentication headers

## [0.0.16] - 2025-12-04T01:38:00+03:00

### Added
- Pricing matrix seeding for Dar es Salaam region and default pricing with exact dimensions and pricing values
  - Small Parcel/Box (28x34x37)
  - Medium Box (30x47x49)
  - Large Box (38x47x65)
  - Extra Large Box (60x61x70)
### Changed
- Navigation menu updates:
  - Added Settings dropdown with Vehicles, Locations, Pricing, and Telemetry
  - Updated active menu item styling (color: #c52227)

## [0.0.15] - 2025-12-04T01:15:00+03:00

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

## [0.0.14] - 2025-12-04T01:10:00+03:00

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

## [0.0.13] - 2025-12-04T00:55:00+03:00

### Added
- Pricing Matrix management UI with full CRUD operations
- Pricing Matrix list page with dimension ranges, pricing details, and filters
- Create/Edit Pricing Matrix form with dimension inputs, pricing configuration, and optional filters (Company, Vehicle Type, Trip Type)
- Pricing menu link in main navigation (Admin only)
- PricingController expanded with Create, Update, Delete endpoints (Admin role required)
- Client-side and API-side DTOs for Pricing Matrix operations

## [0.0.12] - 2025-12-04T00:50:00+03:00

### Fixed
- Fixed JSON deserialization error in Blazor client by making PricingMatrixId, BasePrice, TaxAmount, and TotalPrice nullable in client-side TripDto to match API DTOs

## [0.0.11] - 2025-12-02T21:11:00+03:00

### Added
- Seeded baseline driver-to-vehicle assignments to support trip auto-fill scenarios

## [0.0.10] - 2025-12-02T20:44:00+03:00

### Changed
- Restyled companies administration table to match modern dashboard visuals and align with Tailwind patterns

## [0.0.9] - 2025-12-02T20:40:00+03:00

### Fixed
- Prevented null reference when listing users without associated companies by eager loading company data and guarding DTO mapping

## [0.0.8] - 2025-12-02T13:36:00+03:00

### Added
- Companies hub page with Tailwind-styled directory and quick access from main navigation

### Changed
- Moved Trip Types access into Trips page via dedicated Manage Trip Types button to declutter navigation

## [0.0.7] - 2025-12-02T13:27:00+03:00

### Added
- Introduced CompaniesController with secured GET /api/companies endpoint

### Fixed
- Updated Blazor components to inject IApiService interface consistently
- Expanded IApiService contract to cover all ApiService features for DI resolution

## [0.0.6] - 2025-12-02T12:48:00+03:00

### Fixed
- Fixed 500 Internal Server Error on company dashboard stats endpoint when no trips with prices exist
- CompanyStatsService now properly handles empty collections when calculating average trip price
- Fixed DashboardController dependency injection to use ICompanyStatsService interface instead of concrete class
- Fixed JSON deserialization error by making UserDto.CompanyId nullable to handle users without assigned companies

## [0.0.5] - 2025-12-02T12:35:00+03:00

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

## [0.0.4] - 2025-12-02T10:35:00+03:00

### Changed
- Updated UI branding to use the Global Express logo and icon assets.

## [0.0.3] - 2025-12-02

### Fixed
- Redirect signed-out users to the login page when API responses return HTTP 401 across Blazor UI pages.

## [0.0.2] - 2025-11-27

### Added
- Migration to assign Admin role to demo@gmail.com
- Migration to assign User role to all existing users without roles
- Added dotnet-ef tools to API container runtime image

## [0.0.1] - 2025-12-04
### Added
- Pricing matrix integration to test invoice generation
- Implemented dimension-based pricing for test trips
- Updated test data to include realistic location coordinates

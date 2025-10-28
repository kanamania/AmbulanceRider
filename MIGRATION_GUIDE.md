# Database Migration Guide for Advanced Features

This guide provides step-by-step instructions for creating and applying the database migration for the new advanced features.

## Overview

The advanced features implementation requires a new database table:
- **performance_logs** - Stores API performance monitoring data

## Prerequisites

- .NET 9.0 SDK installed
- PostgreSQL database running and accessible
- Entity Framework Core tools installed

## Install EF Core Tools (if not already installed)

```bash
dotnet tool install --global dotnet-ef
# Or update existing installation
dotnet tool update --global dotnet-ef
```

## Step-by-Step Migration Process

### Step 1: Navigate to API Project

```bash
cd AmbulanceRider.API
```

### Step 2: Create Migration

```bash
dotnet ef migrations add AddPerformanceMonitoring
```

**Expected Output:**
```
Build started...
Build succeeded.
Done. To undo this action, use 'ef migrations remove'
```

This will create a new migration file in the `Migrations/` folder with a timestamp prefix.

### Step 3: Review Migration

Open the generated migration file in `Migrations/` folder and verify it contains:

```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.CreateTable(
        name: "performance_logs",
        columns: table => new
        {
            Id = table.Column<int>(type: "integer", nullable: false)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            Endpoint = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
            HttpMethod = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
            StatusCode = table.Column<int>(type: "integer", nullable: false),
            ResponseTimeMs = table.Column<double>(type: "double precision", nullable: false),
            Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            UserId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
            IpAddress = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
            UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
            RequestSize = table.Column<long>(type: "bigint", nullable: true),
            ResponseSize = table.Column<long>(type: "bigint", nullable: true),
            ErrorMessage = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
        },
        constraints: table =>
        {
            table.PrimaryKey("PK_performance_logs", x => x.Id);
        });

    migrationBuilder.CreateIndex(
        name: "IX_performance_logs_Endpoint",
        table: "performance_logs",
        column: "Endpoint");

    migrationBuilder.CreateIndex(
        name: "IX_performance_logs_StatusCode",
        table: "performance_logs",
        column: "StatusCode");

    migrationBuilder.CreateIndex(
        name: "IX_performance_logs_Timestamp",
        table: "performance_logs",
        column: "Timestamp");
}
```

### Step 4: Apply Migration to Database

```bash
dotnet ef database update
```

**Expected Output:**
```
Build started...
Build succeeded.
Applying migration '20251028_AddPerformanceMonitoring'.
Done.
```

### Step 5: Verify Migration

Connect to your PostgreSQL database and verify the table was created:

```sql
-- Check if table exists
SELECT table_name 
FROM information_schema.tables 
WHERE table_schema = 'public' 
AND table_name = 'performance_logs';

-- Check table structure
\d performance_logs

-- Check indexes
SELECT indexname, indexdef 
FROM pg_indexes 
WHERE tablename = 'performance_logs';
```

## Rollback Instructions (if needed)

If you need to rollback the migration:

```bash
# Remove the last migration
dotnet ef migrations remove

# Or rollback to a specific migration
dotnet ef database update PreviousMigrationName
```

## Troubleshooting

### Issue: "Build failed"

**Solution:**
```bash
# Clean and rebuild
dotnet clean
dotnet build
# Then try migration again
dotnet ef migrations add AddPerformanceMonitoring
```

### Issue: "Unable to create migration"

**Solution:**
1. Ensure you're in the correct directory (`AmbulanceRider.API`)
2. Check that `ApplicationDbContext.cs` has been updated with `DbSet<PerformanceLog>`
3. Verify the connection string in `appsettings.json`

### Issue: "Database update failed"

**Solution:**
1. Check PostgreSQL is running: `pg_isready`
2. Verify connection string in `appsettings.json`
3. Ensure database user has CREATE TABLE permissions
4. Check database logs for detailed error messages

### Issue: "Migration already exists"

**Solution:**
```bash
# Remove the existing migration
dotnet ef migrations remove
# Create a new one
dotnet ef migrations add AddPerformanceMonitoring
```

## Production Deployment

### Option 1: Automatic Migration (Development/Staging)

The application automatically applies migrations on startup (already configured in `Program.cs`):

```csharp
context.Database.Migrate();
```

### Option 2: Manual Migration (Production - Recommended)

For production environments, apply migrations manually:

```bash
# Generate SQL script
dotnet ef migrations script --output migration.sql

# Review the script
cat migration.sql

# Apply manually to production database
psql -h production-host -U username -d database_name -f migration.sql
```

### Option 3: Using Docker

If using Docker Compose, migrations run automatically on container startup.

## Verification Checklist

After migration, verify:

- [ ] Table `performance_logs` exists
- [ ] All columns are present with correct types
- [ ] Indexes are created (Endpoint, StatusCode, Timestamp)
- [ ] Application starts without errors
- [ ] Performance monitoring middleware is capturing data
- [ ] API endpoint `/api/performance/metrics` returns data

## Testing the Migration

### Test 1: Verify Table Structure

```sql
SELECT column_name, data_type, character_maximum_length, is_nullable
FROM information_schema.columns
WHERE table_name = 'performance_logs'
ORDER BY ordinal_position;
```

### Test 2: Insert Test Data

```sql
INSERT INTO performance_logs 
(endpoint, http_method, status_code, response_time_ms, timestamp)
VALUES 
('/api/test', 'GET', 200, 125.5, NOW());

SELECT * FROM performance_logs;
```

### Test 3: Test API Endpoint

```bash
# Make some API requests
curl -X GET "https://localhost:5001/api/trips" \
  -H "Authorization: Bearer YOUR_TOKEN"

# Check performance logs
curl -X GET "https://localhost:5001/api/performance/metrics" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

## Data Retention Strategy

Consider implementing a data retention policy for performance logs:

```sql
-- Delete logs older than 30 days
DELETE FROM performance_logs 
WHERE timestamp < NOW() - INTERVAL '30 days';

-- Or create a scheduled job (PostgreSQL)
CREATE OR REPLACE FUNCTION cleanup_old_performance_logs()
RETURNS void AS $$
BEGIN
    DELETE FROM performance_logs 
    WHERE timestamp < NOW() - INTERVAL '30 days';
END;
$$ LANGUAGE plpgsql;

-- Schedule to run daily (requires pg_cron extension)
SELECT cron.schedule('cleanup-performance-logs', '0 2 * * *', 
    'SELECT cleanup_old_performance_logs()');
```

## Performance Optimization

After migration, consider these optimizations:

### 1. Partitioning (for large datasets)

```sql
-- Create partitioned table (PostgreSQL 10+)
CREATE TABLE performance_logs_partitioned (
    LIKE performance_logs INCLUDING ALL
) PARTITION BY RANGE (timestamp);

-- Create monthly partitions
CREATE TABLE performance_logs_2025_10 
PARTITION OF performance_logs_partitioned
FOR VALUES FROM ('2025-10-01') TO ('2025-11-01');
```

### 2. Additional Indexes

```sql
-- Composite index for common queries
CREATE INDEX idx_performance_logs_timestamp_endpoint 
ON performance_logs(timestamp DESC, endpoint);

-- Index for slow request queries
CREATE INDEX idx_performance_logs_response_time 
ON performance_logs(response_time_ms DESC) 
WHERE response_time_ms > 1000;
```

### 3. Analyze Table

```sql
-- Update statistics for query optimizer
ANALYZE performance_logs;
```

## Backup Recommendations

Before applying migrations in production:

```bash
# Backup database
pg_dump -h localhost -U username -d ambulance_rider > backup_before_migration.sql

# Backup specific table
pg_dump -h localhost -U username -d ambulance_rider -t performance_logs > performance_logs_backup.sql
```

## Next Steps

After successful migration:

1. ✅ Verify performance monitoring is working
2. ✅ Test all new API endpoints
3. ✅ Monitor database performance
4. ✅ Set up data retention policies
5. ✅ Configure monitoring alerts
6. ✅ Update documentation

## Support

If you encounter issues:

1. Check application logs: `AmbulanceRider.API/logs/`
2. Check PostgreSQL logs
3. Review migration files in `Migrations/` folder
4. Refer to Entity Framework Core documentation

---

**Migration Complete! 🎉**

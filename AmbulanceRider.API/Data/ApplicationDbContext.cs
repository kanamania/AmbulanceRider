using AmbulanceRider.API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AmbulanceRider.API.Data;

public class ApplicationDbContext : IdentityDbContext<User, Role, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public new DbSet<User> Users { get; set; }
    public new DbSet<Role> Roles { get; set; }
    public new DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<VehicleType> VehicleTypes { get; set; }
    public DbSet<VehicleDriver> VehicleDrivers { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Trip> Trips { get; set; }
    public DbSet<TripStatusLog> TripStatusLogs { get; set; }
    public DbSet<TripType> TripTypes { get; set; }
    public DbSet<TripTypeAttribute> TripTypeAttributes { get; set; }
    public DbSet<TripAttributeValue> TripAttributeValues { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Telemetry> Telemetries { get; set; }
    public DbSet<PerformanceLog> PerformanceLogs { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply global query filter for soft delete
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            if (typeof(BaseModel).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, "DeletedAt");
                var nullCheck = Expression.Equal(property, Expression.Constant(null, typeof(DateTime?)));
                var lambda = Expression.Lambda(nullCheck, parameter);
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }

        // Configure Identity tables with custom names
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
            
            // Ignore computed property
            entity.Ignore(u => u.FullName);
            
            // Relationship with RefreshTokens
            entity.HasMany(u => u.RefreshTokens)
                .WithOne(rt => rt.User)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
                
            // Relationship with Trips as Driver
            entity.HasMany(u => u.DriverTrips)
                .WithOne(t => t.Driver)
                .HasForeignKey(t => t.DriverId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("roles");
        });

        // Configure UserRole to use Identity's built-in properties
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("user_roles");
            
            entity.HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();
                
            entity.HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();
        });

        // RefreshToken configuration
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("refresh_tokens");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Token).IsRequired();
            entity.Property(e => e.Expires).IsRequired();
            entity.Property(e => e.Created).IsRequired();
            
            // Relationship with User
            entity.HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Vehicle configuration
        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.ToTable("vehicles");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PlateNumber).IsRequired().HasMaxLength(50);
            
            entity.HasOne(v => v.VehicleType)
                .WithMany(vt => vt.Vehicles)
                .HasForeignKey(v => v.VehicleTypeId)
                .OnDelete(DeleteBehavior.Restrict);
                
            // Trips relationship is configured in Trip entity
        });

        // VehicleType configuration
        modelBuilder.Entity<VehicleType>(entity =>
        {
            entity.ToTable("vehicle_types");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
        });
        // VehicleDriver configuration
        modelBuilder.Entity<VehicleDriver>(entity =>
        {
            entity.ToTable("vehicle_drivers");
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Vehicle).WithMany(v => v.VehicleDrivers).HasForeignKey(e => e.VehicleId);
            entity.HasOne(e => e.User).WithMany(u => u.VehicleDrivers).HasForeignKey(e => e.UserId);
            entity.HasQueryFilter(e => e.DeletedAt == null);
        });

        // Location configuration
        modelBuilder.Entity<Location>(entity =>
        {
            entity.ToTable("locations");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ImagePath).HasMaxLength(255);
            entity.Property(e => e.ImageUrl).HasMaxLength(512);
            
            entity.HasQueryFilter(e => e.DeletedAt == null);
        });

        // Trip configuration
        modelBuilder.Entity<Trip>(entity =>
        {
            entity.ToTable("trips");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.RejectionReason).HasMaxLength(500);
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.ScheduledStartTime).IsRequired();
            
            // Coordinate properties
            entity.Property(e => e.FromLatitude).IsRequired();
            entity.Property(e => e.FromLongitude).IsRequired();
            entity.Property(e => e.ToLatitude).IsRequired();
            entity.Property(e => e.ToLongitude).IsRequired();
            entity.Property(e => e.FromLocationName).HasMaxLength(200);
            entity.Property(e => e.ToLocationName).HasMaxLength(200);
            
            // Relationships
            entity.HasOne(t => t.Vehicle)
                .WithMany(v => v.Trips)
                .HasForeignKey(t => t.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);
                
            // Driver relationship is configured in User entity
                
            entity.HasOne(t => t.Approver)
                .WithMany()
                .HasForeignKey(t => t.ApprovedBy)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(t => t.TripType)
                .WithMany(tt => tt.Trips)
                .HasForeignKey(t => t.TripTypeId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasQueryFilter(e => e.DeletedAt == null);
        });

        // TripType configuration
        modelBuilder.Entity<TripType>(entity =>
        {
            entity.ToTable("trip_types");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Color).HasMaxLength(50);
            entity.Property(e => e.Icon).HasMaxLength(50);
            entity.Property(e => e.IsActive).IsRequired();
            entity.Property(e => e.DisplayOrder).IsRequired();
            
            entity.HasQueryFilter(e => e.DeletedAt == null);
        });

        // TripTypeAttribute configuration
        modelBuilder.Entity<TripTypeAttribute>(entity =>
        {
            entity.ToTable("trip_type_attributes");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Label).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.DataType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Options).HasMaxLength(2000);
            entity.Property(e => e.DefaultValue).HasMaxLength(500);
            entity.Property(e => e.ValidationRules).HasMaxLength(500);
            entity.Property(e => e.Placeholder).HasMaxLength(200);
            entity.Property(e => e.IsRequired).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
            entity.Property(e => e.DisplayOrder).IsRequired();
            
            entity.HasOne(a => a.TripType)
                .WithMany(tt => tt.Attributes)
                .HasForeignKey(a => a.TripTypeId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasIndex(e => e.TripTypeId);
            entity.HasQueryFilter(e => e.DeletedAt == null);
        });

        // TripAttributeValue configuration
        modelBuilder.Entity<TripAttributeValue>(entity =>
        {
            entity.ToTable("trip_attribute_values");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Value).HasMaxLength(2000);
            
            entity.HasOne(v => v.Trip)
                .WithMany(t => t.AttributeValues)
                .HasForeignKey(v => v.TripId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(v => v.TripTypeAttribute)
                .WithMany(a => a.Values)
                .HasForeignKey(v => v.TripTypeAttributeId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasIndex(e => e.TripId);
            entity.HasIndex(e => e.TripTypeAttributeId);
            entity.HasQueryFilter(e => e.DeletedAt == null);
        });

        // TripStatusLog configuration
        modelBuilder.Entity<TripStatusLog>(entity =>
        {
            entity.ToTable("trip_status_logs");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FromStatus).IsRequired();
            entity.Property(e => e.ToStatus).IsRequired();
            entity.Property(e => e.ChangedAt).IsRequired();
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.RejectionReason).HasMaxLength(500);
            entity.Property(e => e.UserRole).HasMaxLength(100);
            entity.Property(e => e.UserName).HasMaxLength(255);
            
            // Relationships
            entity.HasOne(tsl => tsl.Trip)
                .WithMany()
                .HasForeignKey(tsl => tsl.TripId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(tsl => tsl.User)
                .WithMany()
                .HasForeignKey(tsl => tsl.ChangedBy)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Index for faster queries
            entity.HasIndex(e => e.TripId);
            entity.HasIndex(e => e.ChangedAt);
            
            entity.HasQueryFilter(e => e.DeletedAt == null);
        });

        // Telemetry configuration
        modelBuilder.Entity<Telemetry>(entity =>
        {
            entity.ToTable("telemetries");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EventType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.EventDetails).HasMaxLength(2000);
            entity.Property(e => e.DeviceType).HasMaxLength(50);
            entity.Property(e => e.DeviceModel).HasMaxLength(100);
            entity.Property(e => e.OperatingSystem).HasMaxLength(50);
            entity.Property(e => e.OsVersion).HasMaxLength(50);
            entity.Property(e => e.Browser).HasMaxLength(50);
            entity.Property(e => e.BrowserVersion).HasMaxLength(50);
            entity.Property(e => e.AppVersion).HasMaxLength(50);
            entity.Property(e => e.GoogleAccount).HasMaxLength(255);
            entity.Property(e => e.AppleAccount).HasMaxLength(255);
            entity.Property(e => e.AccountType).HasMaxLength(50);
            entity.Property(e => e.InstalledApps).HasMaxLength(4000);
            entity.Property(e => e.IpAddress).HasMaxLength(50);
            entity.Property(e => e.ConnectionType).HasMaxLength(50);
            entity.Property(e => e.Orientation).HasMaxLength(20);
            
            // Relationship with User (optional)
            entity.HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.SetNull);
            
            // Indexes for common queries
            entity.HasIndex(e => e.EventType);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.CreatedAt);
        });

        // PerformanceLog configuration
        modelBuilder.Entity<PerformanceLog>(entity =>
        {
            entity.ToTable("performance_logs");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Endpoint).IsRequired().HasMaxLength(500);
            entity.Property(e => e.HttpMethod).IsRequired().HasMaxLength(10);
            entity.Property(e => e.StatusCode).IsRequired();
            entity.Property(e => e.ResponseTimeMs).IsRequired();
            entity.Property(e => e.Timestamp).IsRequired();
            entity.Property(e => e.UserId).HasMaxLength(50);
            entity.Property(e => e.IpAddress).HasMaxLength(50);
            entity.Property(e => e.UserAgent).HasMaxLength(500);
            entity.Property(e => e.ErrorMessage).HasMaxLength(2000);
            
            // Indexes for common queries
            entity.HasIndex(e => e.Endpoint);
            entity.HasIndex(e => e.Timestamp);
            entity.HasIndex(e => e.StatusCode);
        });

        // AuditLog configuration
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.ToTable("audit_logs");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EntityType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.EntityId).IsRequired();
            entity.Property(e => e.Action).IsRequired().HasMaxLength(100);
            entity.Property(e => e.OldValues).HasMaxLength(4000);
            entity.Property(e => e.NewValues).HasMaxLength(4000);
            entity.Property(e => e.AffectedProperties).HasMaxLength(1000);
            entity.Property(e => e.UserName).HasMaxLength(255);
            entity.Property(e => e.UserRole).HasMaxLength(100);
            entity.Property(e => e.IpAddress).HasMaxLength(50);
            entity.Property(e => e.UserAgent).HasMaxLength(500);
            entity.Property(e => e.Notes).HasMaxLength(2000);
            entity.Property(e => e.Timestamp).IsRequired();
            
            // Indexes for common queries
            entity.HasIndex(e => e.EntityType);
            entity.HasIndex(e => e.EntityId);
            entity.HasIndex(e => e.Action);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Timestamp);
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseModel>();
        
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }
        
        return base.SaveChangesAsync(cancellationToken);
    }
}

using System.Globalization;
using System.Text;
using AmbulanceRider.API.Data;
using AmbulanceRider.API.Filters;
using AmbulanceRider.API.Hubs;
using AmbulanceRider.API.Models;
using AmbulanceRider.API.Repositories;
using AmbulanceRider.API.Services;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configure file upload settings
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    // Increase the request size limit to 30MB (default is 30MB, but we'll set it explicitly)
    serverOptions.Limits.MaxRequestBodySize = 30 * 1024 * 1024;
});

// Add Data Protection
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "DataProtection-Keys")))
    .SetApplicationName("AmbulanceRider");

// Add Database Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.ConfigureWarnings(warnings =>
        warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
});

// Add Health Checks
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Database connection string not configured"),
        name: "database",
        timeout: TimeSpan.FromSeconds(3),
        tags: new[] { "db", "sql", "postgresql" });

// Add Repositories
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

// Add Repositories
builder.Services.AddScoped<ITripRepository, TripRepository>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<ITripStatusLogRepository, TripStatusLogRepository>();
builder.Services.AddScoped<ITripAttributeValueRepository, TripAttributeValueRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();

// Add Services
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuditService, AuditService>();

// Add HttpClient
builder.Services.AddHttpClient();

// Add Trip Management Services
builder.Services.AddScoped<ITripManagementService, TripManagementService>();
// Choose routing provider via configuration
builder.Services.AddScoped<IRouteOptimizationService>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var provider = config.GetValue<string>("Routing:Provider") ?? "OSRM";
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();

    if (string.Equals(provider, "Mapbox", StringComparison.OrdinalIgnoreCase))
    {
        var logger = sp.GetRequiredService<ILogger<MapboxRouteOptimizationService>>();
        var options = sp.GetRequiredService<IOptions<MapboxSettings>>();
        return new MapboxRouteOptimizationService(httpClientFactory.CreateClient(), options, logger);
    }
    else
    {
        var logger = sp.GetRequiredService<ILogger<OsrmRouteOptimizationService>>();
        return new OsrmRouteOptimizationService(httpClientFactory.CreateClient(), logger);
    }
});
builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();

// Add Background Services
builder.Services.AddHostedService<ScheduledTasksService>();

// Add Configuration
builder.Services.Configure<MapboxSettings>(builder.Configuration.GetSection("Mapbox"));
builder.Services.Configure<RoutingSettings>(builder.Configuration.GetSection("Routing"));
builder.Services.Configure<FileStorageSettings>(builder.Configuration.GetSection("FileStorage"));

// Add AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Add SignalR
builder.Services.AddSignalR();

// Add Notification Service
builder.Services.AddScoped<INotificationService, SignalRNotificationService>();

// Add Identity
builder.Services.AddIdentity<User, Role>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Configure Identity options
builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1; // Explicitly set to 1 (default)
    
    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    
    // User settings
    options.User.RequireUniqueEmail = true;
});

// Add CORS
var corsSettings = builder.Configuration.GetSection("Cors").Get<CorsSettings>();
if (corsSettings?.AllowedOrigins?.Any() == true)
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowSpecificOrigins", policy =>
        {
            policy.WithOrigins(corsSettings.AllowedOrigins.ToArray())
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
    });
}
else
{
    // Fallback: Allow all origins with credentials support for development
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.SetIsOriginAllowed(_ => true) // Allow any origin
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials(); // Required for SignalR
        });
    });
}

// Configure JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer not configured");
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience not configured");
var jwtExpiresInMinutes = builder.Configuration["Jwt:ExpiresInMinutes"] ?? "1440";

// Add JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero
    };
    
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

// Add Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireUserRole", policy => policy.RequireRole("User"));
});

builder.Services.AddAuthorization();

// Register Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<ITripRepository, TripRepository>();
builder.Services.AddScoped<ITripStatusLogRepository, TripStatusLogRepository>();
builder.Services.AddScoped<ITripTypeRepository, TripTypeRepository>();
builder.Services.AddScoped<ITripTypeAttributeRepository, TripTypeAttributeRepository>();
builder.Services.AddScoped<ITripAttributeValueRepository, TripAttributeValueRepository>();
builder.Services.AddScoped<ITelemetryRepository, TelemetryRepository>();
builder.Services.AddScoped<IPricingMatrixRepository, PricingMatrixRepository>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();

// Register Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<ITripService, TripService>();
builder.Services.AddScoped<ITripTypeService, TripTypeService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ITelemetryService, TelemetryService>();
builder.Services.AddSingleton<ILocalizationService, LocalizationService>();
builder.Services.AddScoped<IDataHashService, DataHashService>();
builder.Services.AddScoped<IDataHashNotificationService, DataHashNotificationService>();
builder.Services.AddScoped<IPricingService, PricingService>();
builder.Services.AddScoped<ICompanyStatsService, CompanyStatsService>();
builder.Services.AddScoped<InvoiceService>();

// Add SignalR
builder.Services.AddSignalR();

// Add services to the container.
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// Configure supported cultures
var supportedCultures = new[]
{
    new CultureInfo("en"),
    new CultureInfo("sw-KE")
};

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("en");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
    options.RequestCultureProviders = new List<IRequestCultureProvider>
    {
        new QueryStringRequestCultureProvider(),
        new CookieRequestCultureProvider(),
        new AcceptLanguageHeaderRequestCultureProvider()
    };
});

builder.Services.AddControllers();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AmbulanceRider API",
        Version = "v1.0.0",
        Description = @"
# AmbulanceRider Emergency Medical Dispatch System API

## Overview
This API provides comprehensive endpoints for managing emergency medical dispatch operations including:
- **Authentication & Authorization**: User registration, login, JWT token management
- **Trip Management**: Create, track, and manage emergency trips
- **Vehicle Management**: Monitor and manage ambulance fleet
- **Location Services**: Track locations and manage facilities
- **Analytics**: Real-time dashboards and performance metrics
- **Telemetry**: Device tracking and location services
- **Notifications**: Real-time updates via SignalR

## Authentication
Most endpoints require JWT Bearer token authentication. To authenticate:
1. Register or login via `/api/auth/register` or `/api/auth/login`
2. Copy the `accessToken` from the response
3. Click the 'Authorize' button above and enter: `Bearer {your-token}`
4. All subsequent requests will include the authentication header

## Rate Limiting
API requests are monitored for performance. Excessive requests may be throttled.

## Support
For issues or questions, contact: support@ambulancerider.com
",
        Contact = new OpenApiContact
        {
            Name = "AmbulanceRider Support Team",
            Email = "support@ambulancerider.com",
            Url = new Uri("https://app.globalexpress.co.tz/support")
        },
        License = new OpenApiLicense
        {
            Name = "Proprietary License",
            Url = new Uri("https://app.globalexpress.co.tz/license")
        },
        TermsOfService = new Uri("https://app.globalexpress.co.tz/terms")
    });

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. 
                      
Enter 'Bearer' [space] and then your token in the text input below.
                      
Example: 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });

    // Include XML comments for better documentation
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
    }

    // Enable annotations for better documentation
    c.EnableAnnotations();
    
    // Order actions by relative path
    c.OrderActionsBy(apiDesc => $"{apiDesc.RelativePath}");
    
    // Use simple type names for schema IDs
    c.CustomSchemaIds(type => type.Name);
    
    // Add operation filters for better documentation
    c.OperationFilter<SwaggerDefaultValues>();
    
    // Add example filters
    c.SchemaFilter<SwaggerExampleSchemaFilter>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Map SignalR hub
    app.MapHub<TripHub>("/tripHub");
    
    app.UseSwagger(options =>
    {
        options.SerializeAsV2 = false;
    });
    
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AmbulanceRider API V1.0.0");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
        
        // Enhanced UI settings
        c.DocumentTitle = "AmbulanceRider API Documentation";
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
        c.DefaultModelsExpandDepth(2);
        c.DefaultModelExpandDepth(2);
        c.DisplayRequestDuration();
        c.EnableDeepLinking();
        c.EnableFilter();
        c.ShowExtensions();
        c.EnableValidator();
        c.SupportedSubmitMethods(
            Swashbuckle.AspNetCore.SwaggerUI.SubmitMethod.Get,
            Swashbuckle.AspNetCore.SwaggerUI.SubmitMethod.Post,
            Swashbuckle.AspNetCore.SwaggerUI.SubmitMethod.Put,
            Swashbuckle.AspNetCore.SwaggerUI.SubmitMethod.Delete,
            Swashbuckle.AspNetCore.SwaggerUI.SubmitMethod.Patch
        );
        
        // Inject custom CSS for better appearance
        c.InjectStylesheet("/swagger-ui/custom.css");
    });
}

// Enable static files
var webRootPath = app.Environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
if (!Directory.Exists(webRootPath))
{
    Directory.CreateDirectory(webRootPath);
}

var uploadsPath = Path.Combine(webRootPath, "uploads");
if (!Directory.Exists(uploadsPath))
{
    Directory.CreateDirectory(uploadsPath);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads"
});

app.UseStaticFiles();

// Configure CORS with the appropriate policy
if (corsSettings?.AllowedOrigins?.Any() == true)
{
    app.UseCors("AllowSpecificOrigins");
}
else
{
    app.UseCors("AllowAll");
}

// Apply migrations and seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<User>>();
        var roleManager = services.GetRequiredService<RoleManager<Role>>();
        var logger = services.GetRequiredService<ILogger<Program>>();
        var configuration = services.GetRequiredService<IConfiguration>();
        
        // Apply any pending migrations
        logger.LogInformation("Applying database migrations...");
        context.Database.Migrate();
        
        // Seed initial data if enabled via environment variable
        var enableSeeding = configuration.GetValue<bool>("EnableSeeding", false);
        if (enableSeeding)
        {
            logger.LogInformation("Seeding is enabled. Checking for data to seed...");
            await DataSeeder.SeedData(context, userManager, roleManager, configuration);
            logger.LogInformation("Database seeding completed.");
        }
        else
        {
            logger.LogInformation("Seeding is disabled. Set EnableSeeding=true to enable.");
        }
        
        // Seed trip types if needed
        logger.LogInformation("Checking trip types...");
        await TripTypeSeedData.SeedTripTypesAsync(context);
        logger.LogInformation("Trip types seeding completed.");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
        throw; // Re-throw to prevent the app from starting with a broken database
    }
}

app.UseHttpsRedirection();

// Add performance monitoring middleware
app.UseMiddleware<AmbulanceRider.API.Middleware.PerformanceMonitoringMiddleware>();

// Add request logging for debugging
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation($"Request: {context.Request.Method} {context.Request.Path}");
    await next();
});

// Authentication & Authorization must be in this order
app.UseAuthentication();
app.UseAuthorization();

// Map controllers
app.MapControllers();

// Map SignalR hubs
app.MapHub<AmbulanceRider.API.Hubs.NotificationHub>("/hubs/notifications");
app.MapHub<AmbulanceRider.API.Hubs.TripHub>("/hubs/trips");

// Map health check endpoints
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready");
app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = _ => false
});

app.Run();

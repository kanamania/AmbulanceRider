using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using AmbulanceRider;
using AmbulanceRider.Services;
using Microsoft.JSInterop;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Configure the root components
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HTTP client with error handling
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "/api/";
builder.Services.AddScoped(sp => 
{
    var client = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
    return client;
});

// Add configuration
var configuration = builder.Configuration;

// Add logging
builder.Logging.SetMinimumLevel(LogLevel.Debug);

builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>(sp => 
    new CustomAuthStateProvider(
        sp.GetRequiredService<IJSRuntime>(),
        sp.GetRequiredService<HttpClient>()
    )
);
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ApiService>(sp => 
    new ApiService(
        sp.GetRequiredService<HttpClient>(),
        configuration
    )
);

try
{
    // Build and run the application
    var app = builder.Build();
    
    // Log environment information
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Application starting in {Environment} environment", 
        builder.HostEnvironment.Environment);
    
    await app.RunAsync();
}
catch (Exception ex)
{
    // This will be displayed in the browser's console
    Console.Error.WriteLine($"Error starting application: {ex}");
    throw;
}
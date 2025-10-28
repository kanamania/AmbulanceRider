using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AmbulanceRider.API.Services;

public class ScheduledTasksService : BackgroundService
{
    private readonly ILogger<ScheduledTasksService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1); // Check every minute

    public ScheduledTasksService(ILogger<ScheduledTasksService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Scheduled Tasks Service is starting");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Running scheduled tasks...");
                
                // Use a scope to resolve scoped services
                using (var scope = _serviceProvider.CreateScope())
                {
                    var tripManagementService = scope.ServiceProvider.GetRequiredService<ITripManagementService>();
                    
                    // Check and start scheduled trips
                    await tripManagementService.CheckAndStartScheduledTripsAsync();
                    
                    // Add other periodic tasks here
                }
                
                _logger.LogInformation("Scheduled tasks completed. Next run in {interval} minutes", _checkInterval.TotalMinutes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing scheduled tasks");
            }
            
            // Wait for the next interval
            await Task.Delay(_checkInterval, stoppingToken);
        }
        
        _logger.LogInformation("Scheduled Tasks Service is stopping");
    }
}

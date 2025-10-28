# Advanced Features Quick Start Guide

This guide will help you quickly get started with the newly implemented advanced features.

## Prerequisites

- .NET 9.0 SDK installed
- PostgreSQL database running
- Node.js (for frontend charting libraries)

## Step 1: Database Migration

Run the following commands to create the new database tables:

```bash
cd AmbulanceRider.API
dotnet ef migrations add AddAdvancedFeatures
dotnet ef database update
```

## Step 2: Configure Email (Optional)

Edit `AmbulanceRider.API/appsettings.json`:

```json
{
  "Email": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": "587",
    "SmtpUsername": "your-email@gmail.com",
    "SmtpPassword": "your-app-password",
    "FromEmail": "noreply@ambulancerider.com",
    "FromName": "AmbulanceRider",
    "EnableSsl": "true"
  }
}
```

**Note:** For Gmail, use an [App Password](https://support.google.com/accounts/answer/185833).

## Step 3: Configure CORS for SignalR

Update `appsettings.json` to allow your frontend origin:

```json
{
  "Cors": {
    "AllowedOrigins": ["https://localhost:5002", "http://localhost:5002"]
  }
}
```

## Step 4: Restore Packages

```bash
# API
cd AmbulanceRider.API
dotnet restore

# Blazor
cd ../AmbulanceRider
dotnet restore
```

## Step 5: Run the Application

```bash
# Terminal 1 - API
cd AmbulanceRider.API
dotnet run

# Terminal 2 - Blazor
cd AmbulanceRider
dotnet run
```

## Quick Feature Tests

### Test 1: Analytics Dashboard

1. Navigate to `/analytics` in your browser
2. You should see dashboard statistics
3. Try changing the date range

**API Test:**
```bash
curl -X GET "https://localhost:5001/api/analytics/dashboard" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

### Test 2: SignalR Real-time Notifications

**JavaScript Console Test:**
```javascript
// In browser console
const connection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:5001/hubs/notifications", {
        accessTokenFactory: () => "YOUR_JWT_TOKEN"
    })
    .build();

connection.on("ReceiveNotification", (notification) => {
    console.log("Notification:", notification);
});

await connection.start();
console.log("Connected to SignalR!");
```

### Test 3: Telemetry Export

**Export to CSV:**
```bash
curl -X GET "https://localhost:5001/api/telemetryanalytics/export/csv" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -o telemetry.csv
```

**Export to JSON:**
```bash
curl -X GET "https://localhost:5001/api/telemetryanalytics/export/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -o telemetry.json
```

### Test 4: Multi-language Support

**Get Spanish translations:**
```bash
curl -X GET "https://localhost:5001/api/localization/es"
```

**Get supported cultures:**
```bash
curl -X GET "https://localhost:5001/api/localization/cultures"
```

### Test 5: Performance Monitoring

**Get performance metrics:**
```bash
curl -X GET "https://localhost:5001/api/performance/metrics" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

**Get slow requests:**
```bash
curl -X GET "https://localhost:5001/api/performance/slow-requests?threshold=500" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

### Test 6: Email Notifications

**C# Test Code:**
```csharp
// In a controller or service
await _emailService.SendEmailAsync(
    "test@example.com",
    "Test Email",
    "<h1>Hello!</h1><p>This is a test email.</p>"
);
```

## Frontend Integration Examples

### Example 1: Connect to SignalR in Blazor

```csharp
@inject SignalRService SignalRService

@code {
    protected override async Task OnInitializedAsync()
    {
        // Start notification hub
        await SignalRService.StartNotificationHubAsync();
        
        // Subscribe to notifications
        SignalRService.OnNotificationReceived += HandleNotification;
        
        // Start trip hub
        await SignalRService.StartTripHubAsync();
        
        // Subscribe to trip updates
        SignalRService.OnTripUpdateReceived += HandleTripUpdate;
    }
    
    private void HandleNotification(SignalRService.NotificationMessage notification)
    {
        // Show toast notification
        Console.WriteLine($"Notification: {notification.Title} - {notification.Message}");
        StateHasChanged();
    }
    
    private void HandleTripUpdate(SignalRService.TripUpdate update)
    {
        // Update trip display
        Console.WriteLine($"Trip {update.TripId} updated: {update.UpdateType}");
        StateHasChanged();
    }
}
```

### Example 2: Display Analytics Chart

```razor
@page "/analytics"
@inject ApiService ApiService

<canvas id="tripChart"></canvas>

@code {
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var stats = await ApiService.GetAsync<List<TripStatusStatsDto>>(
                "analytics/trips/by-status"
            );
            
            // Use Chart.js or similar library
            await JS.InvokeVoidAsync("createChart", "tripChart", stats);
        }
    }
}
```

### Example 3: Language Switcher

```razor
@inject ILocalizationService LocalizationService

<select @onchange="ChangeLanguage">
    <option value="en">English</option>
    <option value="es">Español</option>
    <option value="fr">Français</option>
</select>

@code {
    private string currentLanguage = "en";
    
    private async Task ChangeLanguage(ChangeEventArgs e)
    {
        currentLanguage = e.Value?.ToString() ?? "en";
        // Reload translations
        await LoadTranslations();
        StateHasChanged();
    }
}
```

## Adding Chart.js for Visualizations

### Install via CDN

Add to `wwwroot/index.html`:

```html
<script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
```

### Create Chart Helper

Create `wwwroot/js/charts.js`:

```javascript
window.createPieChart = (canvasId, data) => {
    const ctx = document.getElementById(canvasId).getContext('2d');
    new Chart(ctx, {
        type: 'pie',
        data: {
            labels: data.map(d => d.status),
            datasets: [{
                data: data.map(d => d.count),
                backgroundColor: [
                    '#FF6384',
                    '#36A2EB',
                    '#FFCE56',
                    '#4BC0C0',
                    '#9966FF',
                    '#FF9F40'
                ]
            }]
        },
        options: {
            responsive: true,
            plugins: {
                legend: {
                    position: 'bottom'
                }
            }
        }
    });
};

window.createLineChart = (canvasId, data) => {
    const ctx = document.getElementById(canvasId).getContext('2d');
    new Chart(ctx, {
        type: 'line',
        data: {
            labels: data.map(d => d.label),
            datasets: [{
                label: 'Trips',
                data: data.map(d => d.count),
                borderColor: '#36A2EB',
                backgroundColor: 'rgba(54, 162, 235, 0.2)',
                tension: 0.1
            }]
        },
        options: {
            responsive: true,
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    });
};
```

## Troubleshooting

### Issue: SignalR connection fails

**Solution:**
1. Check CORS configuration
2. Verify JWT token is valid
3. Check browser console for errors
4. Ensure WebSocket is not blocked by firewall

### Issue: Email not sending

**Solution:**
1. Verify SMTP credentials
2. Check if using app-specific password (Gmail)
3. Review logs for error messages
4. Test SMTP connection separately

### Issue: Analytics showing no data

**Solution:**
1. Ensure database has trip data
2. Check date range filters
3. Verify user has proper permissions
4. Check API logs for errors

### Issue: Translations not loading

**Solution:**
1. Verify JSON files are in `Resources/` folder
2. Check JSON syntax is valid
3. Restart the application
4. Check file encoding (UTF-8)

### Issue: Performance logs not appearing

**Solution:**
1. Run database migration
2. Verify middleware is registered
3. Check database connection
4. Review application logs

## Performance Tips

1. **Enable Response Compression:**
```csharp
builder.Services.AddResponseCompression();
app.UseResponseCompression();
```

2. **Add Caching:**
```csharp
builder.Services.AddMemoryCache();
builder.Services.AddResponseCaching();
```

3. **Optimize Database Queries:**
- Use `.AsNoTracking()` for read-only queries
- Implement pagination for large datasets
- Use indexes on frequently queried columns

4. **SignalR Scaling:**
- Consider Azure SignalR Service for production
- Implement Redis backplane for multiple servers

## Next Steps

1. ✅ Run database migrations
2. ✅ Configure email settings
3. ✅ Test SignalR connections
4. ✅ Add charting library
5. ✅ Create dashboard visualizations
6. ✅ Implement notification UI
7. ✅ Add language selector
8. ✅ Test all endpoints
9. ✅ Deploy to production

## Resources

- [SignalR Documentation](https://docs.microsoft.com/en-us/aspnet/core/signalr/)
- [Chart.js Documentation](https://www.chartjs.org/docs/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [Blazor Documentation](https://docs.microsoft.com/en-us/aspnet/core/blazor/)

## Support

For issues or questions:
1. Check the logs in `AmbulanceRider.API/logs/`
2. Review browser console for frontend errors
3. Check database connectivity
4. Refer to `ADVANCED_FEATURES_IMPLEMENTATION.md` for detailed documentation

---

**Happy Coding! 🚀**

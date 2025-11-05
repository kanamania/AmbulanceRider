using AmbulanceRider.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace AmbulanceRider.Services
{
    public class TelemetryDashboardService : ITelemetryService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "api/telemetry";

        public TelemetryDashboardService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<TelemetrySummaryDto> GetSummaryAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var url = $"{BaseUrl}/summary";
                var query = new List<string>();

                if (startDate.HasValue)
                    query.Add($"startDate={Uri.EscapeDataString(startDate.Value.ToString("o"))}");
                
                if (endDate.HasValue)
                    query.Add($"endDate={Uri.EscapeDataString(endDate.Value.ToString("o"))}");

                if (query.Count > 0)
                    url += "?" + string.Join("&", query);

                var result = await _httpClient.GetFromJsonAsync<TelemetrySummaryDto>(url);
                return result ?? new TelemetrySummaryDto();
            }
            catch (Exception)
            {
                // Return an empty summary in case of error
                return new TelemetrySummaryDto();
            }
        }

        public async Task<List<TelemetryEventDto>> GetEventsAsync(int page = 1, int pageSize = 20, string? eventType = null, string? userId = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var url = $"{BaseUrl}/events?page={page}&pageSize={pageSize}";
                
                if (!string.IsNullOrEmpty(eventType))
                    url += $"&eventType={Uri.EscapeDataString(eventType)}";
                    
                if (!string.IsNullOrEmpty(userId))
                    url += $"&userId={Uri.EscapeDataString(userId)}";
                    
                if (startDate.HasValue)
                    url += $"&startDate={Uri.EscapeDataString(startDate.Value.ToString("o"))}";
                    
                if (endDate.HasValue)
                    url += $"&endDate={Uri.EscapeDataString(endDate.Value.ToString("o"))}";

                var result = await _httpClient.GetFromJsonAsync<List<TelemetryEventDto>>(url);
                return result ?? new List<TelemetryEventDto>();
            }
            catch (Exception)
            {
                return new List<TelemetryEventDto>();
            }
        }

        public async Task<Dictionary<string, int>> GetEventTypeDistributionAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var url = $"{BaseUrl}/events/distribution/type";
                var query = new List<string>();

                if (startDate.HasValue)
                    query.Add($"startDate={Uri.EscapeDataString(startDate.Value.ToString("o"))}");
                    
                if (endDate.HasValue)
                    query.Add($"endDate={Uri.EscapeDataString(endDate.Value.ToString("o"))}");

                if (query.Count > 0)
                    url += "?" + string.Join("&", query);

                var result = await _httpClient.GetFromJsonAsync<Dictionary<string, int>>(url);
                return result ?? new Dictionary<string, int>();
            }
            catch (Exception)
            {
                return new Dictionary<string, int>();
            }
        }

        public async Task<Dictionary<string, int>> GetDeviceTypeDistributionAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var url = $"{BaseUrl}/events/distribution/device";
                var query = new List<string>();

                if (startDate.HasValue)
                    query.Add($"startDate={Uri.EscapeDataString(startDate.Value.ToString("o"))}");
                    
                if (endDate.HasValue)
                    query.Add($"endDate={Uri.EscapeDataString(endDate.Value.ToString("o"))}");

                if (query.Count > 0)
                    url += "?" + string.Join("&", query);

                var result = await _httpClient.GetFromJsonAsync<Dictionary<string, int>>(url);
                return result ?? new Dictionary<string, int>();
            }
            catch (Exception)
            {
                return new Dictionary<string, int>();
            }
        }

        public async Task<List<TelemetryEventDto>> GetUserActivityAsync(string userId, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    return new List<TelemetryEventDto>();
                    
                var url = $"{BaseUrl}/users/{Uri.EscapeDataString(userId)}/activity";
                var query = new List<string>();

                if (startDate.HasValue)
                    query.Add($"startDate={Uri.EscapeDataString(startDate.Value.ToString("o"))}");
                    
                if (endDate.HasValue)
                    query.Add($"endDate={Uri.EscapeDataString(endDate.Value.ToString("o"))}");

                if (query.Count > 0)
                    url += "?" + string.Join("&", query);

                var result = await _httpClient.GetFromJsonAsync<List<TelemetryEventDto>>(url);
                return result ?? new List<TelemetryEventDto>();
            }
            catch (Exception)
            {
                return new List<TelemetryEventDto>();
            }
        }
    }
}

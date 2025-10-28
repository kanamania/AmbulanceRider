using System.Text.Json;

namespace AmbulanceRider.API.Services;

public class LocalizationService : ILocalizationService
{
    private readonly ILogger<LocalizationService> _logger;
    private readonly Dictionary<string, Dictionary<string, object>> _resources;
    private readonly string _defaultCulture = "en";

    public LocalizationService(ILogger<LocalizationService> logger, IWebHostEnvironment environment)
    {
        _logger = logger;
        _resources = new Dictionary<string, Dictionary<string, object>>();
        LoadResources(environment);
    }

    private void LoadResources(IWebHostEnvironment environment)
    {
        try
        {
            var resourcesPath = Path.Combine(environment.ContentRootPath, "Resources");
            
            if (!Directory.Exists(resourcesPath))
            {
                _logger.LogWarning($"Resources directory not found: {resourcesPath}");
                return;
            }

            var resourceFiles = Directory.GetFiles(resourcesPath, "SharedResources.*.json");

            foreach (var file in resourceFiles)
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                var culture = fileName.Split('.').Last();

                var json = File.ReadAllText(file);
                var resources = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

                if (resources != null)
                {
                    _resources[culture] = resources;
                    _logger.LogInformation($"Loaded resources for culture: {culture}");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading localization resources");
        }
    }

    public string GetString(string key, string? culture = null)
    {
        var currentCulture = culture ?? _defaultCulture;

        if (!_resources.ContainsKey(currentCulture))
        {
            currentCulture = _defaultCulture;
        }

        if (_resources.ContainsKey(currentCulture))
        {
            var resource = _resources[currentCulture];
            
            // Try to find the key in the flat structure
            if (resource.ContainsKey(key))
            {
                return resource[key]?.ToString() ?? key;
            }

            // Try to find in nested structure
            var parts = key.Split('.');
            if (parts.Length == 2 && resource.ContainsKey(parts[0]))
            {
                var section = resource[parts[0]];
                if (section is JsonElement jsonElement && jsonElement.ValueKind == JsonValueKind.Object)
                {
                    if (jsonElement.TryGetProperty(parts[1], out var value))
                    {
                        return value.GetString() ?? key;
                    }
                }
            }
        }

        return key;
    }

    public string GetString(string section, string key, string? culture = null)
    {
        return GetString($"{section}.{key}", culture);
    }

    public Dictionary<string, object> GetAllStrings(string? culture = null)
    {
        var currentCulture = culture ?? _defaultCulture;

        if (!_resources.ContainsKey(currentCulture))
        {
            currentCulture = _defaultCulture;
        }

        return _resources.ContainsKey(currentCulture) 
            ? _resources[currentCulture] 
            : new Dictionary<string, object>();
    }

    public List<string> GetSupportedCultures()
    {
        return _resources.Keys.ToList();
    }
}

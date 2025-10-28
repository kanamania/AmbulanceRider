namespace AmbulanceRider.API.Services;

public interface ILocalizationService
{
    string GetString(string key, string? culture = null);
    string GetString(string section, string key, string? culture = null);
    Dictionary<string, object> GetAllStrings(string? culture = null);
    List<string> GetSupportedCultures();
}

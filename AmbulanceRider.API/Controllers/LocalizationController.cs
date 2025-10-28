using AmbulanceRider.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace AmbulanceRider.API.Controllers;

/// <summary>
/// Controller for localization and translations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class LocalizationController : ControllerBase
{
    private readonly ILocalizationService _localizationService;
    private readonly ILogger<LocalizationController> _logger;

    public LocalizationController(ILocalizationService localizationService, ILogger<LocalizationController> logger)
    {
        _localizationService = localizationService;
        _logger = logger;
    }

    /// <summary>
    /// Get all translations for a specific culture
    /// </summary>
    [HttpGet("{culture}")]
    public ActionResult<Dictionary<string, object>> GetTranslations(string culture)
    {
        try
        {
            var translations = _localizationService.GetAllStrings(culture);
            return Ok(translations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting translations for culture: {culture}");
            return StatusCode(500, "An error occurred while retrieving translations");
        }
    }

    /// <summary>
    /// Get a specific translation
    /// </summary>
    [HttpGet("{culture}/{key}")]
    public ActionResult<string> GetTranslation(string culture, string key)
    {
        try
        {
            var translation = _localizationService.GetString(key, culture);
            return Ok(new { Key = key, Value = translation });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting translation for key: {key}");
            return StatusCode(500, "An error occurred while retrieving translation");
        }
    }

    /// <summary>
    /// Get supported cultures
    /// </summary>
    [HttpGet("cultures")]
    public ActionResult<List<string>> GetSupportedCultures()
    {
        try
        {
            var cultures = _localizationService.GetSupportedCultures();
            return Ok(cultures);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting supported cultures");
            return StatusCode(500, "An error occurred while retrieving supported cultures");
        }
    }
}

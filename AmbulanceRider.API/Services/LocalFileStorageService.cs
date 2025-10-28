using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace AmbulanceRider.API.Services;

public class FileStorageSettings
{
    public string BasePath { get; set; } = "wwwroot/uploads";
    public string BaseUrl { get; set; } = "/uploads";
    public long MaxFileSize { get; set; } = 10 * 1024 * 1024; // 10MB
    public string[] AllowedExtensions { get; set; } = new[] { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".doc", ".docx" };
}

public class LocalFileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _env;
    private readonly FileStorageSettings _settings;
    private readonly ILogger<LocalFileStorageService> _logger;

    public LocalFileStorageService(
        IWebHostEnvironment env,
        IOptions<FileStorageSettings> settings,
        ILogger<LocalFileStorageService> logger)
    {
        _env = env;
        _settings = settings.Value;
        _logger = logger;
        
        // Ensure the base directory exists
        var fullPath = Path.Combine(_env.ContentRootPath, _settings.BasePath);
        if (!Directory.Exists(fullPath))
        {
            Directory.CreateDirectory(fullPath);
        }
    }

    public async Task<string> SaveFileAsync(IFormFile file, string containerName, string fileName)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("No file uploaded");
        }

        if (file.Length > _settings.MaxFileSize)
        {
            throw new ArgumentException($"File size exceeds the maximum allowed size of {_settings.MaxFileSize / (1024 * 1024)}MB");
        }

        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!_settings.AllowedExtensions.Contains(fileExtension))
        {
            throw new ArgumentException($"File type {fileExtension} is not allowed");
        }

        // Create container directory if it doesn't exist
        var containerPath = Path.Combine(_settings.BasePath, containerName);
        var fullContainerPath = Path.Combine(_env.ContentRootPath, containerPath);
        
        if (!Directory.Exists(fullContainerPath))
        {
            Directory.CreateDirectory(fullContainerPath);
        }

        // Save the file
        var filePath = Path.Combine(containerPath, fileName);
        var fullFilePath = Path.Combine(_env.ContentRootPath, filePath);

        using (var stream = new FileStream(fullFilePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Return the relative URL to access the file
        return $"{_settings.BaseUrl.TrimEnd('/')}/{containerName}/{fileName}";
    }

    public async Task<byte[]> GetFileAsync(string filePath)
    {
        if (string.IsNullOrEmpty(filePath) || !filePath.StartsWith(_settings.BasePath))
        {
            throw new ArgumentException("Invalid file path");
        }

        var fullPath = Path.Combine(_env.ContentRootPath, filePath);
        
        if (!System.IO.File.Exists(fullPath))
        {
            return null;
        }

        return await System.IO.File.ReadAllBytesAsync(fullPath);
    }

    public async Task<bool> DeleteFileAsync(string filePath)
    {
        if (string.IsNullOrEmpty(filePath) || !filePath.StartsWith(_settings.BasePath))
        {
            return false;
        }

        var fullPath = Path.Combine(_env.ContentRootPath, filePath);
        
        if (!System.IO.File.Exists(fullPath))
        {
            return false;
        }

        try
        {
            System.IO.File.Delete(fullPath);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting file {filePath}");
            return false;
        }
    }

    public string GetContentType(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            return "application/octet-stream";
        }

        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        return extension switch
        {
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".txt" => "text/plain",
            ".csv" => "text/csv",
            _ => "application/octet-stream"
        };
    }
}

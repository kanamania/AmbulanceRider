using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace AmbulanceRider.API.Services;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(IFormFile file, string containerName, string fileName);
    Task<byte[]> GetFileAsync(string filePath);
    Task<bool> DeleteFileAsync(string filePath);
    string GetContentType(string filePath);
}

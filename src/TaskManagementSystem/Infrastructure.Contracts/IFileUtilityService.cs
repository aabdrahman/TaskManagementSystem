using Microsoft.AspNetCore.Http;

namespace Infrastructure.Contracts;

public interface IFileUtilityService
{
    Task<string> UploadFileAsync(string taskId, IFormFile uploadedFile);
    Task<string> RemoveFileAsync(string taskId, string fileName);
}

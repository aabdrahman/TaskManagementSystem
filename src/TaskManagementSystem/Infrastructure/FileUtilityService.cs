using Contracts.Infrastructure;
using Entities.ConfigurationModels;
using Infrastructure.Contracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Infrastructure;

public sealed class FileUtilityService : IFileUtilityService
{
    private readonly ILoggerManager _loggerManager;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly UploadConfig _uploadConfig;

    private string _contentFilePath = string.Empty;

    public FileUtilityService(ILoggerManager loggerManager, IWebHostEnvironment webHostEnvironment, IOptionsMonitor<UploadConfig> uploadConfigOptionsMonitor)
    {
        _loggerManager = loggerManager;
        _webHostEnvironment = webHostEnvironment;
        _uploadConfig = uploadConfigOptionsMonitor.CurrentValue as UploadConfig;
 
    }

    public async Task<string> RemoveFileAsync(string taskId, string fileName)
    {
        try
        {
            await _loggerManager.LogInfo($"Deleting File for task Id: {taskId} - {fileName}");

            _contentFilePath = _uploadConfig.UploadFilepath;

            var fullPath = Path.Combine(_contentFilePath, taskId, fileName);

            if (!File.Exists(fullPath))
            {
                await _loggerManager.LogInfo($"File not found: {fullPath}");
                return "FileNotFound";
            }

            // Remove read-only if set
            var fileInfo = new FileInfo(fullPath);
            if (fileInfo.IsReadOnly)
            {
                fileInfo.IsReadOnly = false;
            }
            await _loggerManager.LogInfo($"Deleting File for task Id: {taskId} - {fileName} Path: {fullPath}");
            File.Delete(fullPath);

            return "success";
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, $"Error removing file: {taskId}/{fileName}");
            return "Failed";
        }
    }


    public async Task<string> UploadFileAsync(string taskId, IFormFile uploadedFile)
    {
        try
        {
            _contentFilePath = Path.Combine(_uploadConfig.UploadFilepath);
            if (!Directory.Exists(Path.Combine(_contentFilePath, taskId)))
            {
                Directory.CreateDirectory(Path.Combine(_contentFilePath, taskId));
            }

            using(var memoryFileStream = new MemoryStream())
            {
                await uploadedFile.CopyToAsync(memoryFileStream);
                await File.WriteAllBytesAsync(Path.Combine(_contentFilePath, taskId, uploadedFile.FileName), memoryFileStream.ToArray());
            }

            return $"Success-{Path.Combine(_contentFilePath, taskId, uploadedFile.FileName)}";
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, $"Error Occurred Uploading file for: {taskId}");
            return "Failed";
        }
    }
}

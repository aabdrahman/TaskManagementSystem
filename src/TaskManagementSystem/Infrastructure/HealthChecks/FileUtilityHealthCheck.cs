using Entities.ConfigurationModels;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Infrastructure.HealthChecks;

public sealed class FileUtilityHealthCheck : IHealthCheck
{
    private UploadConfig _uploadConfig;
    public FileUtilityHealthCheck(IOptionsMonitor<UploadConfig> uploadConfigOptionsMonitor)
    {
        _uploadConfig = uploadConfigOptionsMonitor.CurrentValue;
    }
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var filePath = Path.Combine(_uploadConfig.UploadFilepath);

            if(!Directory.Exists(filePath) || string.IsNullOrEmpty(_uploadConfig.UploadFilepath))
            {
                return HealthCheckResult.Unhealthy($"The Upload File Path is not yet provided in configuration file.");
            }

            try
            {
                if(File.Exists(Path.Combine(_uploadConfig.UploadFilepath, "testFile.txt")))
                {
                    File.Delete(Path.Combine(_uploadConfig.UploadFilepath, "testFile.txt"));
                }

                File.WriteAllText(Path.Combine(_uploadConfig.UploadFilepath, "testFile.txt"), "This is a sample text file");

                return HealthCheckResult.Healthy("File Utility Path available.");
            }
            catch (Exception ex)
            {

                return HealthCheckResult.Unhealthy($"Cannot perform file operations in provided file path.");
            }
            
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("File Utility Validation Failed.");
        }
    }
}
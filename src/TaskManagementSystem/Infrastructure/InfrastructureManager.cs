using Contracts.Infrastructure;
using Entities.ConfigurationModels;
using Infrastructure.Contracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace Infrastructure;

public sealed class InfrastructureManager : IInfrastructureManager
{
    private readonly ILoggerManager _loggerManager;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IOptionsMonitor<UploadConfig> _uploadConfigOptionsMonitor;

    private readonly Lazy<IFileUtilityService> _fileUtilityService;
    public InfrastructureManager(ILoggerManager loggerManager, IWebHostEnvironment webHostEnvironment, IOptionsMonitor<UploadConfig> uploadConfigOptionsMonitor)
    {
        _loggerManager = loggerManager;
        _webHostEnvironment = webHostEnvironment;
        _uploadConfigOptionsMonitor = uploadConfigOptionsMonitor;

        _fileUtilityService = new Lazy<IFileUtilityService>(() => new FileUtilityService(_loggerManager, _webHostEnvironment, _uploadConfigOptionsMonitor));

    }
    public IFileUtilityService FileUtilityService => _fileUtilityService.Value;
}

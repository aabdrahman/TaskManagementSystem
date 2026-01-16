using Contracts.Infrastructure;
using Entities.ConfigurationModels;
using Infrastructure.Contracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Options;
using ZiggyCreatures.Caching.Fusion;

namespace Infrastructure;

public sealed class InfrastructureManager : IInfrastructureManager
{
    private readonly ILoggerManager _loggerManager;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IOptionsMonitor<UploadConfig> _uploadConfigOptionsMonitor;
    private readonly HybridCache _hybridCache;
    private readonly IFusionCache _fusionCache;

    private readonly Lazy<IFileUtilityService> _fileUtilityService;
    private readonly Lazy<ICacheService> _cacheService;
    public InfrastructureManager(ILoggerManager loggerManager, IWebHostEnvironment webHostEnvironment, IOptionsMonitor<UploadConfig> uploadConfigOptionsMonitor, HybridCache hybridCache, IFusionCache fusionCache)
    {
        _loggerManager = loggerManager;
        _webHostEnvironment = webHostEnvironment;
        _uploadConfigOptionsMonitor = uploadConfigOptionsMonitor;
        _hybridCache = hybridCache;
        _fusionCache = fusionCache;


        _fileUtilityService = new Lazy<IFileUtilityService>(() => new FileUtilityService(_loggerManager, _webHostEnvironment, _uploadConfigOptionsMonitor));
        _cacheService = new Lazy<ICacheService>(() => new CacheService(_loggerManager, _hybridCache, _fusionCache));

    }
    public IFileUtilityService FileUtilityService => _fileUtilityService.Value;

    public ICacheService CacheService => _cacheService.Value;
}

using Contracts;
using Contracts.Infrastructure;
using Entities.ConfigurationModels;
using Infrastructure.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Service.Contract;

namespace Services;

public sealed class ServiceManager : IServiceManager
{
    private readonly ILoggerManager _loggerManager;
    private readonly IRepositoryManager _repositoryManager;
    private readonly IInfrastructureManager _infrastructureManager;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IOptionsMonitor<UploadConfig> _uploadConfigOptionsMonitor;
    private readonly IConfiguration _configuration;
    private readonly IOptionsMonitor<JwtConfiguration> _jwtConfigurationOptionsMonitor;

    private readonly Lazy<IUnitService> _unitService;
    private readonly Lazy<IRoleService> _roleService;
    private readonly Lazy<IUserService> _userService;
    private readonly Lazy<ICreatedTaskService> _createdTaskService;
    private readonly Lazy<ITaskUserService> _taskUserService;
    private readonly Lazy<IAttachmentService> _attachmentService;
    private readonly Lazy<IAnalyticsReportingService> _analyticsReportingService;

    public ServiceManager(ILoggerManager loggerManager, IRepositoryManager repositoryManager, 
                            IInfrastructureManager infrastructureManager, IOptionsMonitor<UploadConfig> uploadConfigOptionsMonitor, 
                            IHttpContextAccessor httpContextAccessor, IConfiguration configuration,
                            IOptionsMonitor<JwtConfiguration> jwtConfigurationOptionsMonitor)
    {
        _loggerManager = loggerManager;
        _repositoryManager = repositoryManager;
        _infrastructureManager = infrastructureManager;
        _uploadConfigOptionsMonitor = uploadConfigOptionsMonitor;
        _contextAccessor = httpContextAccessor;
        _configuration = configuration;
        _jwtConfigurationOptionsMonitor = jwtConfigurationOptionsMonitor;

        _unitService = new Lazy<IUnitService>(() => new UnitService(_loggerManager, _repositoryManager, _contextAccessor, _infrastructureManager));
        _roleService = new Lazy<IRoleService>(() => new RoleService(_repositoryManager, _loggerManager, _infrastructureManager, _contextAccessor));
        _userService = new Lazy<IUserService>(() => new UserService(_repositoryManager, _loggerManager, _configuration, _jwtConfigurationOptionsMonitor, _contextAccessor));
        _createdTaskService = new Lazy<ICreatedTaskService>(() => new CreatedTaskService(_repositoryManager, _loggerManager, _contextAccessor));
        _taskUserService = new Lazy<ITaskUserService>(() => new TaskUserService(_repositoryManager, _loggerManager, _contextAccessor, _infrastructureManager));
        _attachmentService = new Lazy<IAttachmentService>(() => new AttachmentService(_repositoryManager, _loggerManager, _infrastructureManager, _uploadConfigOptionsMonitor, _contextAccessor));
        _analyticsReportingService = new Lazy<IAnalyticsReportingService>(() => new AnalyticsReportingService(_loggerManager, _repositoryManager, _contextAccessor));

    }


    public IUnitService UnitService => _unitService.Value;

    public IRoleService RoleService => _roleService.Value;

    public IUserService UserService => _userService.Value;

    public ICreatedTaskService CreatedTaskService => _createdTaskService.Value;

    public ITaskUserService TaskUserService => _taskUserService.Value;

    public IAttachmentService AttachmentService => _attachmentService.Value;

    public IAnalyticsReportingService AnalyticsReportingService => _analyticsReportingService.Value;
}

using Contracts;
using Contracts.Infrastructure;
using Service.Contract;

namespace Services;

public sealed class ServiceManager : IServiceManager
{
    private readonly ILoggerManager _loggerManager;
    private readonly IRepositoryManager _repositoryManager;

    private readonly Lazy<IUnitService> _unitService;
    private readonly Lazy<IRoleService> _roleService;
    private readonly Lazy<IUserService> _userService;

    public ServiceManager(ILoggerManager loggerManager, IRepositoryManager repositoryManager)
    {
        _loggerManager = loggerManager;
        _repositoryManager = repositoryManager;
        

        _unitService = new Lazy<IUnitService>(() => new UnitService(_loggerManager, _repositoryManager));
        _roleService = new Lazy<IRoleService>(() => new RoleService(_repositoryManager, _loggerManager));
        _userService = new Lazy<IUserService>(() => new UserService(_repositoryManager, _loggerManager));
    }


    public IUnitService UnitService => _unitService.Value;

    public IRoleService RoleService => _roleService.Value;

    public IUserService UserService => _userService.Value;
}

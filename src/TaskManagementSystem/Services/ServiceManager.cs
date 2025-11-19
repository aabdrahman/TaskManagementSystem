using Contracts;
using Contracts.Infrastructure;
using Service.Contract;

namespace Services;

public sealed class ServiceManager : IServiceManager
{
    private readonly ILoggerManager _loggerManager;
    private readonly IRepositoryManager _repositoryManager;

    private readonly Lazy<IUnitService> _unitService;

    public ServiceManager(ILoggerManager loggerManager, IRepositoryManager repositoryManager)
    {
        _loggerManager = loggerManager;
        _repositoryManager = repositoryManager;
        

        _unitService = new Lazy<IUnitService>(() => new UnitService(_loggerManager, _repositoryManager));
    }


    public IUnitService UnitService => _unitService.Value;
}

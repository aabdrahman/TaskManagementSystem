using Contracts;
using Contracts.Infrastructure;
using Service.Contract;
using Shared.DataTransferObjects.Unit;

namespace Services;

public sealed class UnitService : IUnitService
{
    private readonly ILoggerManager _loggerManager;
    private readonly IRepositoryManager _repositoryManager;
    public UnitService(ILoggerManager loggerManager, IRepositoryManager repositoryManager)
    {
        _loggerManager = loggerManager;
        _repositoryManager = repositoryManager;
    }

    public Task<UnitDto> CreateAsync(CreateUnitDto nwwUnitToCreate)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(int UnitId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<UnitDto>> GetAllUnitsAsync(bool trackChanges, bool hasQueryFilter)
    {
        throw new NotImplementedException();
    }

    public Task<UnitDto> GetByIdAsuync(int UnitId)
    {
        throw new NotImplementedException();
    }
}

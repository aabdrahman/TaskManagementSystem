using Shared.DataTransferObjects.Unit;

namespace Service.Contract;

public interface IUnitService
{
    Task<IEnumerable<UnitDto>> GetAllUnitsAsync(bool trackChanges, bool hasQueryFilter);
    Task<UnitDto> CreateAsync(CreateUnitDto nwwUnitToCreate);
    Task DeleteAsync(int UnitId);
    Task<UnitDto> GetByIdAsuync(int UnitId);
}

using Shared.ApiResponse;
using Shared.DataTransferObjects.Unit;

namespace Service.Contract;

public interface IUnitService
{
    Task<GenericResponse<IEnumerable<UnitDto>>> GetAllUnitsAsync(bool trackChanges, bool hasQueryFilter);
    Task<GenericResponse<UnitDto>> CreateAsync(CreateUnitDto newUnitToCreate);
    Task<GenericResponse<string>> DeleteAsync(int UnitId, bool isSoftDelete = true);
    Task<GenericResponse<UnitDto>> GetByIdAsuync(int UnitId);
    Task<GenericResponse<UnitDto>> UpdateUnitAsync(UpdateUnitDto updatedUnit);
}

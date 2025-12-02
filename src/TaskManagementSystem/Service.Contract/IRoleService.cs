using Shared.ApiResponse;
using Shared.DataTransferObjects.Role;

namespace Service.Contract;

public interface IRoleService
{
    Task<GenericResponse<RoleDto>> GetByIdAsync(int Id, bool trackChanges = false, bool hasQueryFilter = false);
    Task<GenericResponse<RoleDto>> CreateAsync(CreateRoleDto createRole);
    Task<GenericResponse<IEnumerable<RoleDto>>> GetAllAsync(bool trackChnages, bool hasQueryFilter);
    Task<GenericResponse<string>> DeleteAsync(int Id, bool isSofDelete = true);
}

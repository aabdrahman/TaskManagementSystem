using Entities.Models;
using Shared.DataTransferObjects.Role;

namespace Shared.Mapper;

public static class RoleMapper
{
    public static RoleDto ToDto(this Role role)
    {
        return new RoleDto()
        {
            RoleId = role.Id,
            RoleName = role.NormalizedName
        };
    }

    public static Role ToEntity(this CreateRoleDto createRole)
    {
        return new Role()
        {
            CreatedBy = "",
            Name = createRole.Name
        };
    }
}

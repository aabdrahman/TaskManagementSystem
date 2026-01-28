using Entities.Models;
using Shared.DataTransferObjects.Role;
using System.Linq.Expressions;

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

    public static Expression<Func<Role, RoleDto>> ToDtoExpression()
    {
        return role => new RoleDto()
        {
            RoleId = role.Id,
            RoleName = role.NormalizedName
        };
    }
}

using Entities.Models;

namespace Contracts;

public interface IRoleRepository
{
    IQueryable<Role> GetAllRoles(bool trackChanges = true, bool hasQueryFilter = true);
    IQueryable<Role> GetById(int Id, bool trackChanges = true, bool hasQueryFilter = true);
    Task CreateRole(Role newRole);
    void UpdateRole(Role updatedRole);
    void DeleteRole(Role deletedRole);
}

using Contracts;
using Entities.Models;

namespace Repository;

public class RoleRepository : RepositoryBase<Role>, IRoleRepository
{
    public RoleRepository(RepositoryContext repositoryContext) : base(repositoryContext)
    {
    }

    public async Task CreateRole(Role newRole)
    {
        await Create(newRole);
    }

    public void DeleteRole(Role deletedRole)
    {
        DeleteEntity(deletedRole);
    }

    public IQueryable<Role> GetAllRoles(bool trackChanges = true, bool hasQueryFilter = true)
    {
        return FindAll(trackChanges, hasQueryFilter);
    }

    public IQueryable<Role> GetById(int Id, bool trackChanges = true, bool hasQueryFilter = true)
    {
        return FindByCondition(x => x.Id == Id, trackChanges, hasQueryFilter);
    }

    public void UpdateRole(Role updatedRole)
    {
        UpdateEntity(updatedRole);
    }
}

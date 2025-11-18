using Contracts;
using Entities.Models;

namespace Repository;

public sealed class UserRoleRepository : RepositoryBase<UserRole>, IUserRoleRepository
{
    public UserRoleRepository(RepositoryContext repositoryContext) : base(repositoryContext)
    {
    }

    public async Task CreateUserRole(UserRole newUserRole)
    {
        await Create(newUserRole);
    }

    public void DeleteUserRole(UserRole deletedUserRole)
    {
        DeleteEntity(deletedUserRole);
    }

    public IQueryable<UserRole> GetAll(bool trackChanges = true, bool hasQueryFilter = true)
    {
        return FindAll(trackChanges, hasQueryFilter);
    }

    public IQueryable<UserRole> GetById(int Id, bool trackChanges = true, bool hasQueryFilter = true)
    {
        return FindByCondition(x => x.Id == Id, trackChanges, hasQueryFilter);
    }

    public IQueryable<UserRole> GetByRoleId(int roleId, bool trackChanges = true, bool hasQueryFilter = true)
    {
        return FindByCondition(x => x.RoleId == roleId, trackChanges, hasQueryFilter);
    }

    public IQueryable<UserRole> GetByUserId(int userId, bool trackChanges = true, bool hasQueryFilter = true)
    {
        return FindByCondition(x => x.UserId == userId, trackChanges, hasQueryFilter); 
    }

    public void UpdateUserRole(UserRole updatedUserRole)
    {
        UpdateEntity(updatedUserRole);
    }
}

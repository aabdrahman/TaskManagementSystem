using Contracts;
using Entities.Models;

namespace Repository;

public sealed class UserRepository : RepositoryBase<User>, IUserRepository
{
    public UserRepository(RepositoryContext repositoryContext) : base(repositoryContext)
    {
    }

    public async Task CreateUser(User newUser)
    {
        await Create(newUser);
    }

    public void DeleteUser(User deletedUser)
    {
        DeleteEntity(deletedUser);
    }

    public IQueryable<User> GetAllUsers(bool trackChanges = true, bool hasQueryFilter = true)
    {
        return FindAll(trackChanges, hasQueryFilter);
    }

    public IQueryable<User> GetByEmail(string email, bool trackChanges = true, bool hasQueryFilter = true)
    {
        return FindByCondition(x => x.Email == email.ToUpper(), trackChanges, hasQueryFilter);
    }

    public IQueryable<User> GetById(int Id, bool trackChanges = true, bool hasQueryFilter = true)
    {
        return FindByCondition(x => x.Id == Id, trackChanges, hasQueryFilter);
    }

    public IQueryable<User> GetByUnitId(int unitId, bool trackChanges = true, bool hasQueryFilter = true)
    {
        return FindByCondition(x => x.UnitId == unitId, trackChanges, hasQueryFilter);
    }

    public IQueryable<User> GetByUserName(string userName, bool trackChanges = true, bool hasQueryFilter = true)
    {
        return FindByCondition(x => x.Username == userName.ToUpper(), trackChanges, hasQueryFilter);
    }

    public void UpdateUser(User updatedUser)
    {
        UpdateEntity(updatedUser);
    }
}

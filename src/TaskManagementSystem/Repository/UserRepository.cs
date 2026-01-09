using Contracts;
using Entities.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Repository.QueryExtensions;
using Shared.RequestParameters;

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

    public IQueryable<User> GetAllUsers(UsersRequestParameter usersRequestParameter, bool trackChanges = true, bool hasQueryFilter = true)
    {
        return FindAll(trackChanges, hasQueryFilter)
                        .SearchByEmail(usersRequestParameter)
                        .SearchByName(usersRequestParameter)
                        .SearchByUnitId(usersRequestParameter);
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

    public async Task<IQueryable<User>> GetUsersWithSameUnit(int userId)
    {
        var unitIdParameter = new SqlParameter("@_userId_0", userId);

        string query = @"SELECT *
                            FROM dbo.Users
                            WHERE [Id] != @_userId_0 AND [UnitId] = 
                            (
                            SELECT [UnitId]
                            FROM dbo.Users
                            WHERE [Id] = @_userId_0)";

        var result = await CustomeDatabaseQuery(query, unitIdParameter);

        return result;
    }

    public void UpdateUser(User updatedUser)
    {
        UpdateEntity(updatedUser);
    }
}

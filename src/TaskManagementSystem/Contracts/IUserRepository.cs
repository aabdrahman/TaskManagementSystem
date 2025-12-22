using Entities.Models;

namespace Contracts;

public interface IUserRepository
{
    Task CreateUser(User newUser);
    void UpdateUser(User updatedUser);
    void DeleteUser(User deletedUser);
    IQueryable<User> GetAllUsers(bool trackChanges = true, bool hasQueryFilter = true);
    IQueryable<User> GetByUserName(string userName, bool trackChanges = true, bool hasQueryFilter = true);
    IQueryable<User> GetByEmail(string email, bool trackChanges = true, bool hasQueryFilter = true);
    IQueryable<User> GetByUnitId(int unitId, bool trackChanges = true, bool hasQueryFilter = true);
    IQueryable<User> GetById(int Id, bool trackChanges = true, bool hasQueryFilter = true);
    Task<IQueryable<User>> GetUsersWithSameUnit(int userId);
}

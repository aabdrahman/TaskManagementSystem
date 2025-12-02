using Entities.Models;

namespace Contracts;

public interface IUserRoleRepository
{
    Task CreateUserRole(UserRole newUserRole);
    void UpdateUserRole(UserRole updatedUserRole);
    void DeleteUserRole(UserRole deletedUserRole);
    IQueryable<UserRole> GetAll(bool trackChanges = true, bool hasQueryFilter = true);
    IQueryable<UserRole> GetByRoleId(int roleId, bool trackChanges = true, bool hasQueryFilter = true);
    IQueryable<UserRole> GetByUserId(int userId, bool trackChanges = true, bool hasQueryFilter = true);
    IQueryable<UserRole> GetById(int Id, bool trackChanges = true, bool hasQueryFilter = true);
}

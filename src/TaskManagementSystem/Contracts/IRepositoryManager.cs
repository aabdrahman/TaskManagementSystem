using Contracts.Keyless_Entities_Repository;

namespace Contracts;

public interface IRepositoryManager
{
    IUnitRepository UnitRepository { get; }
    IRoleRepository RoleRepository { get; }
    IUserRepository UserRepository { get; }
    ICreatedTaskRepository CreatedTaskRepository { get; }
    IUserRoleRepository UserRoleRepository { get; }
    ITaskUserRepository TaskUserRepository { get; }
    IAttachmentRepository AttachmentRepository { get; }
    IUserUnitAssignedUserTaskAnalyticsRepository UserUnitAssignedUserTaskAnalyticsRepository { get; }
    Task SaveChangesAsync();
}

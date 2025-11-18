namespace Contracts;

public interface IRepositoryManager
{
    IUnitRepository UnitRepository { get; }
    IRoleRepository RoleRepository { get; }
    IUserRepository UserRepository { get; }
}

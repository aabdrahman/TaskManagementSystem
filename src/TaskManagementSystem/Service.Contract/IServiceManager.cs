namespace Service.Contract;

public interface IServiceManager
{
    IUnitService UnitService { get; }
    IRoleService RoleService { get; }
}

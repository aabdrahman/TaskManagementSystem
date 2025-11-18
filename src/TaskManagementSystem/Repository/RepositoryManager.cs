using Contracts;

namespace Repository;

public sealed class RepositoryManager : IRepositoryManager
{
    private readonly RepositoryContext _repositoryContext;
    private readonly Lazy<IUnitRepository> _unitRepository;
    private readonly Lazy<IRoleRepository> _roleRepository;
    private readonly Lazy<IUserRepository> _userRepository;
    private readonly Lazy<ICreatedTaskRepository> _createdTaskRepository;
    public RepositoryManager(RepositoryContext repositoryContext)
    {
        _repositoryContext = repositoryContext;
        _unitRepository = new Lazy<IUnitRepository>(() => new UnitRepository(_repositoryContext));
        _roleRepository = new Lazy<IRoleRepository>(() => new RoleRepository(_repositoryContext));
        _userRepository = new Lazy<IUserRepository>(() => new UserRepository(_repositoryContext));
        _createdTaskRepository = new Lazy<ICreatedTaskRepository>(() => new CreatedTaskRepository(_repositoryContext));
    }

    public IUnitRepository UnitRepository => _unitRepository.Value;

    public IRoleRepository RoleRepository => _roleRepository.Value;

    public IUserRepository UserRepository => _userRepository.Value;

    public ICreatedTaskRepository CreatedTaskRepository => _createdTaskRepository.Value;
}

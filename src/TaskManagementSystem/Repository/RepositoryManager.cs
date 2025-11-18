using Contracts;

namespace Repository;

public sealed class RepositoryManager : IRepositoryManager
{
    private readonly RepositoryContext _repositoryContext;
    private readonly Lazy<IUnitRepository> _unitRepository;
    private readonly Lazy<IRoleRepository> _roleRepository;

    public RepositoryManager(RepositoryContext repositoryContext)
    {
        _repositoryContext = repositoryContext;
        _unitRepository = new Lazy<IUnitRepository>(() => new UnitRepository(repositoryContext));
        _roleRepository = new Lazy<IRoleRepository>(() => new RoleRepository(repositoryContext));
    }

    public IUnitRepository UnitRepository => _unitRepository.Value;

    public IRoleRepository RoleRepository => _roleRepository.Value;
}

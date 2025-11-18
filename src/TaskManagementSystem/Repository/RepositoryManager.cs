using Contracts;

namespace Repository;

public sealed class RepositoryManager : IRepositoryManager
{
    private readonly RepositoryContext _repositoryContext;
    private readonly Lazy<IUnitRepository> _unitRepository;

    public RepositoryManager(RepositoryContext repositoryContext)
    {
        _repositoryContext = repositoryContext;
        _unitRepository = new Lazy<IUnitRepository>(() => new UnitRepository(repositoryContext));
    }

    public IUnitRepository UnitRepository => _unitRepository.Value;
}

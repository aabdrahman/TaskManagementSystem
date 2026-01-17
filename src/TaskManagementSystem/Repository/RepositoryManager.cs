using Contracts;
using Contracts.Keyless_Entities_Repository;
using Repository.Keyless_Entities;

namespace Repository;

public sealed class RepositoryManager : IRepositoryManager
{
    private readonly RepositoryContext _repositoryContext;
    private readonly Lazy<IUnitRepository> _unitRepository;
    private readonly Lazy<IRoleRepository> _roleRepository;
    private readonly Lazy<IUserRepository> _userRepository;
    private readonly Lazy<ICreatedTaskRepository> _createdTaskRepository;
    private readonly Lazy<IUserRoleRepository> _userRoleRepository;
    private readonly Lazy<TaskUserRepository> _taskUserRepository;
    private readonly Lazy<IAttachmentRepository> _attachmentRepository;
    private readonly Lazy<IUserUnitAssignedUserTaskAnalyticsRepository> _userUnitAssignedUserTaskAnalyticsRepository;

    public RepositoryManager()
    {
    }

    public RepositoryManager(RepositoryContext repositoryContext)
    {
        _repositoryContext = repositoryContext;
        _unitRepository = new Lazy<IUnitRepository>(() => new UnitRepository(_repositoryContext));
        _roleRepository = new Lazy<IRoleRepository>(() => new RoleRepository(_repositoryContext));
        _userRepository = new Lazy<IUserRepository>(() => new UserRepository(_repositoryContext));
        _createdTaskRepository = new Lazy<ICreatedTaskRepository>(() => new CreatedTaskRepository(_repositoryContext));
        _userRoleRepository = new Lazy<IUserRoleRepository>(() => new UserRoleRepository(_repositoryContext));
        _taskUserRepository = new Lazy<TaskUserRepository>(()  => new TaskUserRepository(_repositoryContext));
        _attachmentRepository = new Lazy<IAttachmentRepository>(() => new AttachmentRepository(_repositoryContext));
        _userUnitAssignedUserTaskAnalyticsRepository = new Lazy<IUserUnitAssignedUserTaskAnalyticsRepository>(() => new UserUnitAssignedUserTaskAnalyticsRepository(_repositoryContext));
    }

    public IUnitRepository UnitRepository => _unitRepository.Value;

    public IRoleRepository RoleRepository => _roleRepository.Value;

    public IUserRepository UserRepository => _userRepository.Value;

    public ICreatedTaskRepository CreatedTaskRepository => _createdTaskRepository.Value;

    public IUserRoleRepository UserRoleRepository => _userRoleRepository.Value;

    public ITaskUserRepository TaskUserRepository => _taskUserRepository.Value;

    public IAttachmentRepository AttachmentRepository => _attachmentRepository.Value;

    public IUserUnitAssignedUserTaskAnalyticsRepository UserUnitAssignedUserTaskAnalyticsRepository => _userUnitAssignedUserTaskAnalyticsRepository.Value;

    public async Task SaveChangesAsync()
    {
        await _repositoryContext.SaveChangesAsync();
    }
}

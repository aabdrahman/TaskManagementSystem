namespace Service.Contract;

public interface IServiceManager
{
    IUnitService UnitService { get; }
    IRoleService RoleService { get; }
    IUserService UserService { get; }
    ICreatedTaskService CreatedTaskService { get; }
    ITaskUserService TaskUserService { get; }
    IAttachmentService AttachmentService { get; }
    IAnalyticsReportingService AnalyticsReportingService { get; }
}

using Shared.ApiResponse;
using Shared.DataTransferObjects.AnalyticsReporting;
using Shared.DataTransferObjects.AnalyticsReporting.UserDashboard;
using Shared.RequestParameters.Analytics;

namespace Service.Contract;

public interface IAnalyticsReportingService
{
    Task<GenericResponse<IEnumerable<UserUnitAssignedUserTaskAnalyticsDto>>> GetUserOrUnitAnalytics(UserUnitTaskUserAnalyticsRequestParameter userUnitTaskUserAnalyticsRequestParameter);
    Task<GenericResponse<UserTaskDashboardDto>> GetUserDashboard(int UserId);
}

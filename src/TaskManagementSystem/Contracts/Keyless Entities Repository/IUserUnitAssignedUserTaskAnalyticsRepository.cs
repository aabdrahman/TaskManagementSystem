using Entities.Models.Keyless_Entities;
using Shared.RequestParameters.Analytics;

namespace Contracts.Keyless_Entities_Repository;

public interface IUserUnitAssignedUserTaskAnalyticsRepository
{
    Task<IQueryable<UserUnitAssignedUserTaskAnalytics>> GetUserUnitAnalytics(UserUnitTaskUserAnalyticsRequestParameter userUnitTaskUserAnalyticsRequestParameter);
}

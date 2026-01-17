using Contracts.Keyless_Entities_Repository;
using Entities.Models;
using Entities.Models.Keyless_Entities;
using Microsoft.Data.SqlClient;
using Shared.RequestParameters.Analytics;

namespace Repository.Keyless_Entities;

public sealed class UserUnitAssignedUserTaskAnalyticsRepository : RepositoryBase<UserUnitAssignedUserTaskAnalytics>, IUserUnitAssignedUserTaskAnalyticsRepository
{
    public UserUnitAssignedUserTaskAnalyticsRepository(RepositoryContext repositoryContext) : base(repositoryContext)
    {
    }

    public async Task<IQueryable<UserUnitAssignedUserTaskAnalytics>> GetUserUnitAnalytics(UserUnitTaskUserAnalyticsRequestParameter userUnitTaskUserAnalyticsRequestParameter)
    {
        SqlParameter startDateParameter = new SqlParameter("@StartDate", userUnitTaskUserAnalyticsRequestParameter.StartDate);
        SqlParameter endDateParameter = new SqlParameter("@EndDate", userUnitTaskUserAnalyticsRequestParameter.EndDate); 
        SqlParameter unitIdParameter = new SqlParameter("@UnitId", userUnitTaskUserAnalyticsRequestParameter.UnitId.HasValue ? userUnitTaskUserAnalyticsRequestParameter.UnitId : (object)DBNull.Value);
        SqlParameter userIdParameter = new SqlParameter("@UserId", userUnitTaskUserAnalyticsRequestParameter.UserId.HasValue ? userUnitTaskUserAnalyticsRequestParameter.UserId : (object)DBNull.Value);

        return await CustomDatabaseQueryWithListResult(@"EXEC [dbo].[Proc_GetUserOrUnitUserTaskAnalytics] 
                @StartDate, @EndDate, @UnitId, @UserId", 
            startDateParameter, endDateParameter, unitIdParameter, userIdParameter);
    }
}


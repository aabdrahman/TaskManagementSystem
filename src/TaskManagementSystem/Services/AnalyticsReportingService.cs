using Contracts;
using Contracts.Infrastructure;
using Entities.Models.Keyless_Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Service.Contract;
using Shared.ApiResponse;
using Shared.DataTransferObjects.AnalyticsReporting;
using Shared.DataTransferObjects.AnalyticsReporting.UserDashboard;
using Shared.RequestParameters.Analytics;
using System.Data.Common;
using System.Net;
using System.Text.Json;

namespace Services;

public sealed class AnalyticsReportingService : IAnalyticsReportingService
{
    private readonly ILoggerManager _loggerManager;
    private readonly IRepositoryManager _repositoryManager;
    private readonly IHttpContextAccessor _contextAccessor;

    public AnalyticsReportingService(ILoggerManager loggerManager, IRepositoryManager repositoryManager, IHttpContextAccessor contextAccessor)
    {
        _loggerManager = loggerManager;
        _repositoryManager = repositoryManager;
        _contextAccessor = contextAccessor;
    }

    public async Task<GenericResponse<UserTaskDashboardDto>> GetUserDashboard(int UserId)
    {
        try
        {
            await _loggerManager.LogInfo($"Fetching User Dashboard for: {UserId}");

            string loggedinUserId = _contextAccessor.HttpContext.User.FindFirst(x => x.Type.EndsWith("serialnumber"))?.Value ?? "0";

            if(!(loggedinUserId == UserId.ToString()))
            {
                await _loggerManager.LogInfo($"Logged in User Id: {loggedinUserId} is not the same as {UserId}");
                return GenericResponse<UserTaskDashboardDto>.Failure(null, HttpStatusCode.Conflict, $"User cannot spool dashboard.");
            }

            //GET COUNT OF USER TASKS
            int TotalUserTasks = await _repositoryManager.TaskUserRepository.GetByUserId(UserId).CountAsync(x => !x.CompletionDate.HasValue && x.CancelReason == null);

            if (TotalUserTasks == 0)
            {
                await _loggerManager.LogWarning($"No Record Found for - {UserId}");
                return GenericResponse<UserTaskDashboardDto>.Failure(new UserTaskDashboardDto(), HttpStatusCode.NotFound, "No Pending Record Found."); //This returns a default instance for frontend use
            }

            UserTaskDashboardDto output = await _repositoryManager.TaskUserRepository.GetByUserId(UserId)
                                                                        .Where(x => !x.CompletionDate.HasValue && x.CancelReason == null)
                                                                        .GroupBy(_ => 1)
                                                                        .Select(x => new UserTaskDashboardDto()
                                                                        {
                                                                            DueToday = x.Count(x => x.ProposedCompletionDate == DateTime.UtcNow.Date),
                                                                            OverDueTasks = x.Count(x => x.ProposedCompletionDate < DateTime.UtcNow.Date),
                                                                            PendingTasks = x.Count(x => x.ProposedCompletionDate >= DateTime.UtcNow.Date.AddDays(1)),

                                                                            UserTasks = x.OrderBy(x => x.ProposedCompletionDate).Select(x => new UserTaskSummaryDto() { Id = x.Id, Title = x.Title, DueDate = x.ProposedCompletionDate }).Skip(0).Take(5).ToList()
                                                                        })
                                                                        .FirstAsync() ?? new UserTaskDashboardDto();


            await _loggerManager.LogInfo($"User Dashboard fetched for user: {UserId} - {SerializeObjectToString(output)}");

            return GenericResponse<UserTaskDashboardDto>.Success(output, HttpStatusCode.OK, "Dashboard Fetched.");

        }
        catch (DbException ex)
        {
            await _loggerManager.LogError(ex, "Internal Server Error - Database");
            return GenericResponse<UserTaskDashboardDto>.Failure(null, HttpStatusCode.InternalServerError, $"Internal Server Error - Database", new { ex.Message, Description = ex?.InnerException?.Message });
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, "Internal Server Error");
            return GenericResponse<UserTaskDashboardDto>.Failure(null, HttpStatusCode.InternalServerError, $"Internal Server Error", new { ex.Message, Description = ex?.InnerException?.Message });
        }
    }

    public async Task<GenericResponse<IEnumerable<UserUnitAssignedUserTaskAnalyticsDto>>> GetUserOrUnitAnalytics(UserUnitTaskUserAnalyticsRequestParameter userUnitTaskUserAnalyticsRequestParameter)
    {
        try
        {
            await _loggerManager.LogInfo($"Spooling User Analytics for - {SerializeObjectToString(userUnitTaskUserAnalyticsRequestParameter)}");

            IQueryable<UserUnitAssignedUserTaskAnalytics> analyticsResultAsQueryable = await _repositoryManager.UserUnitAssignedUserTaskAnalyticsRepository.GetUserUnitAnalytics(userUnitTaskUserAnalyticsRequestParameter);

            IEnumerable<UserUnitAssignedUserTaskAnalytics> res = await analyticsResultAsQueryable.ToListAsync();

            IEnumerable<UserUnitAssignedUserTaskAnalyticsDto> result = res.Select(x => new UserUnitAssignedUserTaskAnalyticsDto()
            { 
                UserId = x.UserId,
                FirstName = x.FirstName,
                LastName = x.LastName,
                TotalAssignedTasks = x.TotalAssignedTasks,  
                TotalCancelledTasks = x.TotalCancelledTasks,
                TotalTimeTaken = x.TotalTimeTaken ?? 0,
                TotalCompletedTasks = x.TotalCompletedTasks,
                TotalDeletedTasks = x.TotalDeletedTasks,
                TotalOverdueTasks = x.TotalOverdueTasks,
                TotalPendingTasks = x.TotalPendingTasks,
                CompletedAfterDueDate = x.CompletedAfterDueDate,
                CompletedWithinDueDate = x.CompletedWithinDueDate,
                CompletionRate = x.CompletionRate
            }).ToList();

            await _loggerManager.LogInfo($"Report Analytics successfully spooled - {SerializeObjectToString(res)}");

            return GenericResponse<IEnumerable<UserUnitAssignedUserTaskAnalyticsDto>>.Success(result, HttpStatusCode.OK, $"Analytics Reporting Fetched Successfully.");
        }
        catch (DbException ex)
        {
            await _loggerManager.LogError(ex, $"Internal Database Server Error - {ex.Message}");
            return GenericResponse<IEnumerable<UserUnitAssignedUserTaskAnalyticsDto>>.Failure(null, System.Net.HttpStatusCode.InternalServerError, $"Internal Database Server Error.", new { errorMessage = ex.Message, description = ex?.InnerException?.Message });
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, $"Internal Server Error - {ex.Message}");
            return GenericResponse<IEnumerable<UserUnitAssignedUserTaskAnalyticsDto>>.Failure(null, System.Net.HttpStatusCode.InternalServerError, $"Internal Server Error.", new { errorMessage = ex.Message, description = ex?.InnerException?.Message });
        }
    }

    private string SerializeObjectToString(object obj)
        => JsonSerializer.Serialize(obj);
}

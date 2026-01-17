using Contracts.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Contract;
using Shared.RequestParameters.Analytics;
using System.Net;

namespace TaskManagementSystem.ApiPresentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AnalyticsReportingController : ControllerBase
{
    private readonly ILoggerManager _loggerManager;
    private readonly IServiceManager _serviceManager;

    public AnalyticsReportingController(ILoggerManager loggerManager, IServiceManager serviceManager)
    {
        _loggerManager = loggerManager;
        _serviceManager = serviceManager;
    }


    [HttpGet("getUserUnitAnalytics")]
    public async Task<IActionResult> GetUserUnitAnalytics([FromQuery] UserUnitTaskUserAnalyticsRequestParameter userUnitTaskUserAnalyticsRequestParameter)
    {
        try
        {
            var getReportResponse = await _serviceManager.AnalyticsReportingService.GetUserOrUnitAnalytics(userUnitTaskUserAnalyticsRequestParameter);

            return StatusCode((int)getReportResponse.StatusCode, getReportResponse);
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, ex.Message);
            return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    [HttpGet("userdashboard/userid/{userId:int}")]
    public async Task<IActionResult> GetUserDaashboard(int userId)
    {
        try
        {
            var getByUserDashboardResponse = await _serviceManager.AnalyticsReportingService.GetUserDashboard(userId);

            return StatusCode((int)getByUserDashboardResponse.StatusCode, getByUserDashboardResponse);
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, ex.Message);
            return StatusCode(500, ex.Message);
        }
    }
}

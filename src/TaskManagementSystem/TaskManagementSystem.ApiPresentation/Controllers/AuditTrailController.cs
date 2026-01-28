using Contracts.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Service.Contract;

namespace TaskManagementSystem.ApiPresentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuditTrailController : ControllerBase
{
    private readonly IServiceManager _serviceManager;
    private readonly ILoggerManager _loggerManager;

    public AuditTrailController(ILoggerManager loggerManager, IServiceManager serviceManager)
    {
        _loggerManager = loggerManager;
        _serviceManager = serviceManager;
    }

    [HttpGet("getparticipantaudit/{userId:int}")]
    public async Task<IActionResult> GetParticipantAudit(int userId)
    {
        try
        {
            var fetchParticipantResponse = await _serviceManager.AuditService.GetAuditTrailForUser(userId);

            return StatusCode((int)fetchParticipantResponse.StatusCode, fetchParticipantResponse);
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetRecordAudit(string entityName, int entityId)
    {
        try
        {
            var getRecordAuditResponse = await  _serviceManager.AuditService.GetAuditForRecord(entityId, entityName);

            return StatusCode((int)getRecordAuditResponse.StatusCode, getRecordAuditResponse);
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, ex.Message);
            return StatusCode(500, ex.Message);
        }
    }
}

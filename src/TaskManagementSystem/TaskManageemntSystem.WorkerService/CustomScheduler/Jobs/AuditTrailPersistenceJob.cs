using Contracts.Infrastructure;
using Infrastructure.Contracts;
using Quartz;
using Shared.DataTransferObjects.AuditTrail;
using System.Text.Json;

namespace TaskManageemntSystem.WorkerService.CustomScheduler.Jobs;

[DisallowConcurrentExecution]
public sealed class AuditTrailPersistenceJob : IJob
{
    private readonly ILoggerManager _loggerManager;
    private readonly IAuditPersistenceService _auditPersistenceService;

    public AuditTrailPersistenceJob(ILoggerManager loggerManager, IAuditPersistenceService auditPersistenceService)
    {
        _loggerManager = loggerManager;
        _auditPersistenceService = auditPersistenceService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            await _loggerManager.LogInfo($"Running Audit Trail Persistence Job at: {DateTime.Now}");

            ProcessQueuedAuditResponseDto result = await _auditPersistenceService.ProcessQueuedAudit();

            await _loggerManager.LogInfo($"Audit Persistence successfully run at: {DateTime.Now}. Result - {JsonSerializer.Serialize(result)}");
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, $"Audit Persistence ob failed to run at: {DateTime.Now}");
        }
    }
}

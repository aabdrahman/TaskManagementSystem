using Contracts;
using Contracts.Infrastructure;
using Entities.Models;
using Infrastructure.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Shared.DataTransferObjects.AuditTrail;
using System.Collections.Concurrent;

public sealed class AuditPersistenceService : IAuditPersistenceService
{
    private readonly ILoggerManager _loggerManager;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    private readonly ConcurrentQueue<AuditTrail> _auditQueue = new();
    private int _isProcessing = 0; // 0 = false, 1 = true

    public AuditPersistenceService(
        ILoggerManager loggerManager,
        IServiceScopeFactory serviceScopeFactory)
    {
        _loggerManager = loggerManager;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task<bool> QueueAuditRecord(AuditTrail auditTrail)
    {
        try
        {
            _auditQueue.Enqueue(auditTrail);

            await _loggerManager.LogInfo(
                $"Audit queued: {auditTrail.EntityName} ({auditTrail.EntityId})");

            return true;
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, "Failed to queue audit trail.");
            return false;
        }
    }

    public async Task<ProcessQueuedAuditResponseDto> ProcessQueuedAudit()
    {
        if (Interlocked.Exchange(ref _isProcessing, 1) == 1)
        {
            return new ProcessQueuedAuditResponseDto
            {
                TotalProcessed = 0,
                ProcessedAt = DateTime.UtcNow.ToLocalTime()
            };
        }

        int successCount = 0;
        int failureCount = 0;
        var processedAt = DateTime.UtcNow.ToLocalTime();
        var retryList = new List<AuditTrail>();

        try
        {
            await _loggerManager.LogInfo(
                $"Processing audit queue at {processedAt}");

            using var scope = _serviceScopeFactory.CreateAsyncScope();
            var repositoryManager = scope.ServiceProvider
                .GetRequiredService<IRepositoryManager>();

            while (_auditQueue.TryDequeue(out var audit))
            {
                try
                {
                    await repositoryManager.AuditTrailRepository
                        .CreateAuditTrailAsync(audit);

                    successCount++;
                }
                catch (Exception ex)
                {
                    failureCount++;
                    retryList.Add(audit);

                    await _loggerManager.LogError(
                        ex,
                        $"Failed to persist audit for {audit.EntityName} ({audit.EntityId})");
                }
            }

            await repositoryManager.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(
                ex,
                "Fatal error while processing audit queue.");
        }
        finally
        {
            foreach (var retry in retryList)
                _auditQueue.Enqueue(retry);

            Interlocked.Exchange(ref _isProcessing, 0);
        }

        return new ProcessQueuedAuditResponseDto
        {
            SuccessCount = successCount,
            FailureCount = failureCount,
            TotalProcessed = successCount + failureCount,
            ProcessedAt = processedAt
        };
    }
}

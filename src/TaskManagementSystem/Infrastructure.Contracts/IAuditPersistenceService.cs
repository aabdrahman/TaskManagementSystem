using Entities.Models;
using Shared.DataTransferObjects.AuditTrail;

namespace Infrastructure.Contracts;

public interface IAuditPersistenceService
{
    Task<bool> QueueAuditRecord(AuditTrail auditTrail);
    Task<ProcessQueuedAuditResponseDto> ProcessQueuedAudit();
}

using Entities.Models;

namespace Contracts;

public interface IAuditTrailRepository
{
    Task CreateAuditTrailAsync(AuditTrail auditTrail);
    Task CreateMultipleAuditTrailAsync(IEnumerable<AuditTrail> auditTrails);
    IQueryable<AuditTrail> GetAuditTrail(string entityId, string entityName);
    IQueryable<AuditTrail> GetParticipantAudit(string participantId);
}

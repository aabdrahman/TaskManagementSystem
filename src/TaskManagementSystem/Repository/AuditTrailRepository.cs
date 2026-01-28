using Contracts;
using Entities.Models;

namespace Repository;

public sealed class AuditTrailRepository : RepositoryBase<AuditTrail>, IAuditTrailRepository
{
    public AuditTrailRepository(RepositoryContext repositoryContext) : base(repositoryContext)
    {
    }

    public async Task CreateAuditTrailAsync(AuditTrail auditTrail)
    {
        await Create(auditTrail);
    }

    public async Task CreateMultipleAuditTrailAsync(IEnumerable<AuditTrail> auditTrails)
    {
        await CreateMultiple(auditTrails);
    }

    public IQueryable<AuditTrail> GetAuditTrail(string entityId, string entityName)
    {
        return FindByCondition(x => x.EntityId == entityId && x.EntityName == entityName, false, true);
    }

    public IQueryable<AuditTrail> GetParticipantAudit(string participantId)
    {
        return FindByCondition(x => x.ParticipandIdentification == participantId, false, true);
    }
}

using Entities.Models;
using Shared.DataTransferObjects.AuditTrail;
using System.Linq.Expressions;

namespace Shared.Mapper;

public static class AuditTrailMapper
{
    public static Expression<Func<AuditTrail, AuditTrailDto>> ToDtoExpression()
    {
        return auditTrail => new AuditTrailDto()
        {
            EntityId = auditTrail.EntityId,
            EntityName = auditTrail.EntityName,
            ParticipandIdentification = auditTrail.ParticipandIdentification,
            ParticipantName = auditTrail.ParticipantName,
            PerformedAction = auditTrail.PerformedAction.ToString(),
            PerformedAt = auditTrail.PerformedAt
        };
    }
}

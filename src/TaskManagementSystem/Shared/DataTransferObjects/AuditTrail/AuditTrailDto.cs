using Entities.StaticValues;

namespace Shared.DataTransferObjects.AuditTrail;

public record class AuditTrailDto
{
    public string EntityId { get; set; }
    public string EntityName { get; set; }
    public string ParticipantName { get; set; }
    public string ParticipandIdentification { get; set; }
    public DateTime PerformedAt { get; set; }
    public string PerformedAction { get; set; }
}

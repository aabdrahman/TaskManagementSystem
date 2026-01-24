using Entities.StaticValues;

namespace Entities.Models;

public class AuditTrail
{
    public Guid Id { get; set; }
    public string EntityId { get; set; }
    public string EntityName { get; set; }
    public string ParticipantName { get; set; }
    public string ParticipandIdentification { get; set; }
    public DateTime PerformedAt { get; set; }
    public AuditAction PerformedAction { get; set; }

    public string OldValue { get; set; } = "";
    public string NewValue { get; set; } = "";
}

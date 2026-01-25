namespace Shared.DataTransferObjects.AuditTrail;

public record class ProcessQueuedAuditResponseDto
{
    public int TotalProcessed { get; set; }
    public DateTime ProcessedAt { get; set; }
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
}

using Entities.StaticValues;

namespace Shared.DataTransferObjects.CreatedTask;

public record class CreatedTaskDto
{
    public int Id { get; set; }
    public string TaskId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime ProposedCompletionDate { get; set; }
    public string Priority { get; set; }
    public DateTime? CompletionDate { get; set; }
    public string Stage { get; set; }
}

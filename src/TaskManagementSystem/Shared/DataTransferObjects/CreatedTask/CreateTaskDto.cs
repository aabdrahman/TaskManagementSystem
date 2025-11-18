namespace Shared.DataTransferObjects.CreatedTask;

public record class CreateTaskDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime ProposedCompletionDate { get; set; }
    public string Priority { get; set; }
    public string Stage { get; set; }
}

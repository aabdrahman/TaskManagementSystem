namespace Shared.DataTransferObjects.TaskUser;

public record class TaskUserDto
{
    public int Id { get; set; } 
    public int PrimaryTaskId { get; set; }
    public string TaskId { get; set; }
    public DateTime? CompletionDate { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string AssignedUser { get; set; }
}

public record class CreateTaskUserDto
{
    public DateTime ProposedCompletionDate { get; set; }
    public string TaskId { get; set; }
    public int PrimaryTaskId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int AssignedUser { get; set; }

}

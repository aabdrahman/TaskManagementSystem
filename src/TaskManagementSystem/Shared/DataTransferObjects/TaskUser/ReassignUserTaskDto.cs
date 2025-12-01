namespace Shared.DataTransferObjects.TaskUser;

public record class ReassignUserTaskDto
{
    public int NewUserId { get; set; }
    public string Justification { get; set; }
}
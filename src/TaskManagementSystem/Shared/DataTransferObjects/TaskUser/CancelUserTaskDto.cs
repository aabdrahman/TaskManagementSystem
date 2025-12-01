namespace Shared.DataTransferObjects.TaskUser;

public record class CancelUserTaskDto
{
    public int UserTaskId { get; set; }
    public string CancelReason { get; set; }
}

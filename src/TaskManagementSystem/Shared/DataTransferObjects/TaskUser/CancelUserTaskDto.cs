using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects.TaskUser;

public record class CancelUserTaskDto
{
    [Required(ErrorMessage = "User Task Id is required.")]
    public int UserTaskId { get; set; }
    [Required(ErrorMessage = "Cancel Reason is required.")]
    public string CancelReason { get; set; }
}

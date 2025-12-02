using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects.CreatedTask;

public record class CancelCreatedTaskDto
{
    [Required(ErrorMessage = "Requester is required.")]
    public int ReqesterId { get; set; }
    [Required(ErrorMessage = "Justification is required.")]
    public string Justification { get; set; }
    [Required(ErrorMessage = "Task Id is required.")]
    public string TaskId { get; set; }
    [Required(ErrorMessage = "Primary Id is required.")]
    public int TaskPrimaryId { get; set; }
}

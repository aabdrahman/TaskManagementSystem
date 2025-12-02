using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects.TaskUser;

public record class ReassignTaskUserDto
{
    [Required(ErrorMessage = "User is required")]
    public int UserId { get; set; }
    [Required(ErrorMessage = "Justification is required.")]
    public string Justification { get; set; }
    [Required(ErrorMessage = "Task Id is required")]
    public string TaskId { get; set; }
    [Required(ErrorMessage = "User Task Id is required")]
    public int UserTaskId { get; set; }
}


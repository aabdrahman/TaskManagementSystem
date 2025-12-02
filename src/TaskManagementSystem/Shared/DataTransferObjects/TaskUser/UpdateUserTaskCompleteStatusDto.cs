using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects.TaskUser;

public record class UpdateUserTaskCompleteStatusDto
{
    [Required(ErrorMessage = "Task User Id is required.")]
    public int Id { get; set; }
    [Required(ErrorMessage = "Comment is required.")]
    public string Comment { get; set; }
}
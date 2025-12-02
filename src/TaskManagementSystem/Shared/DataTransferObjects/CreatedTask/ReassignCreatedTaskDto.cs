using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects.CreatedTask;

public record class ReassignCreatedTaskDto
{
    [Required(ErrorMessage = "Üser is required")]
    public int UserId { get; set; }
    [Required(ErrorMessage = "Justification is required.")]
    public string Justification { get; set; }
    [Required(ErrorMessage = "Task Id is required")]
    public string TaskId { get; set; }
}
using Shared.CustomValidator;
using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects.TaskUser;

public record class CreateTaskUserDto
{
    [DateTimeValidator]
    public DateTime ProposedCompletionDate { get; set; }
    [Required(ErrorMessage = "Task Id is a required field.")]
    public string TaskId { get; set; }
    [Required(ErrorMessage = "Primary Task Id is a required field.")]
    public int PrimaryTaskId { get; set; }
    [Required(ErrorMessage = "Title is a required field.")]
    [StringLength(250, ErrorMessage = "Title cannot exceed 250 characters.")]
    public string Title { get; set; }
    [Required(ErrorMessage = "Description is a required field.")]
    public string Description { get; set; }
    [Required(ErrorMessage = "Assigned User is a required field.")]
    [Range(0, maximum:double.MaxValue, ErrorMessage = "Invalid Selected Assigned User")]
    public int AssignedUser { get; set; }
    public int? ResponsibleUnit { get; set; }
}

using Shared.CustomValidator;
using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects.TaskUser;

public record class UpdateTaskUserDto
{
    [Required(ErrorMessage = "Task User Id is required.")]
    public int Id { get; set; }
    [DateTimeValidator]
    public DateTime ProposedCompletionDate { get; set; }
    [Required(ErrorMessage = "Title is a required field.")]
    [StringLength(250, ErrorMessage = "Title cannot exceed 250 characters.")]
    public string Title { get; set; }
    [Required(ErrorMessage = "Description is a required field.")]
    public string Description { get; set; }
}
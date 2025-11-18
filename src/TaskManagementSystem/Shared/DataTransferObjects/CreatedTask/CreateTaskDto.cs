using Shared.CustomValidator;
using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects.CreatedTask;

public record class CreateTaskDto
{
    [Required(ErrorMessage = "Title is a required field.")]
    public string Title { get; set; }
    [Required(ErrorMessage = "Description is a required field.")]
    public string Description { get; set; }
    [DateTimeValidator]
    [Required(ErrorMessage = "Proposed Completion Date is a required field.")]
    public DateTime ProposedCompletionDate { get; set; }
    [Required(ErrorMessage = "Priority is a required field.")]

    public string Priority { get; set; }
    [Required(ErrorMessage = "Stage is a required field.")]
    public string Stage { get; set; }
}

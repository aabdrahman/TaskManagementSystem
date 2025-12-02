using Entities.StaticValues;
using Shared.CustomValidator;
using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects.CreatedTask;

public record class UpdateCreatedTaskDto
{
    [Required(ErrorMessage = "Id is a required field.")]
    public int Id { get; set; }
    [Required(ErrorMessage = "Description is a required field.")]
    public string Description { get; set; }
    [Required(ErrorMessage = "Title is a required field.")]
    public string Title { get; set; }
    [DateTimeValidatorAttribute]
    [Required(ErrorMessage = "Proposed Completion Date is a required field.")]
    public DateTime ProposedCompletionDate { get; set; }
    [Required(ErrorMessage = "Priority is a required field.")]
    [EnumTypeValidator(typeof(PriorityLevel), message: "Invalid Priority provided.")]
    public string Priority { get; set; }
}

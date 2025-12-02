using Entities.StaticValues;
using Shared.CustomValidator;
using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects.CreatedTask;

public record class UpdateCreatedTaskStatusDto
{
    [Required(ErrorMessage = "Task User Id is required.")]
    public int Id { get; set; }
    [Required(ErrorMessage = "Comment is required.")]
    public string Comment { get; set; }
    [EnumTypeValidator(typeof(Stage), message: "Invalid Stage provided.")]
    public string Stage { get; set; }
}
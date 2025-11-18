using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects.Attachment;

public record class CreateAttachmentDto
{
    [Required(ErrorMessage = "Task Id is a required field.")]
    public string TaskId { get; set; }
    [Required(ErrorMessage = "Kindly upload file")]
    public string FileName { get; set; }
}

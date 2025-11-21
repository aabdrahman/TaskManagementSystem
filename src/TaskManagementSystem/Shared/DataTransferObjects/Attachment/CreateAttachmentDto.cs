using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects.Attachment;

public record class CreateAttachmentDto
{
    [Required(ErrorMessage = "Task Id is a required field.")]
    public string TaskId { get; set; }
    [Required(ErrorMessage = "Kindly upload file")]
    [StringLength(100, ErrorMessage = "File Name cannot exceed 100 characters.")]
    public string FileName { get; set; }
    public IFormFile AttachmentFile { get; set; }
}

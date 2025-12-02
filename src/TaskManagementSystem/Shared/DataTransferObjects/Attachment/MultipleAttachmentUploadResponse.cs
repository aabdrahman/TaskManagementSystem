using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects.Attachment;

public record class MultipleAttachmentUploadResponse
(
    int successCount,
    int failureCount,
    IEnumerable<string> successfulFiles
);

public class UploadMultipleAttachmentDto
{
    public string TaskId { get; set; }
    [MinLength(1, ErrorMessage = "One or more upload attachments required.")]
    public IEnumerable<IFormFile> Attachments { get; set; }
}

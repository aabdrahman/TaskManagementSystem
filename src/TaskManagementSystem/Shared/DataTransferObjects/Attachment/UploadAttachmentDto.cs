using Microsoft.AspNetCore.Http;

namespace Shared.DataTransferObjects.Attachment;

public record class UploadAttachmentDto
(
    IFormFile UploadAttachment,
    string TaskId
);

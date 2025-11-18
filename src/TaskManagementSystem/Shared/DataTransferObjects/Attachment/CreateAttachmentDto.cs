namespace Shared.DataTransferObjects.Attachment;

public record class CreateAttachmentDto
{
    public string TaskId { get; set; }
    public string FileName { get; set; }
}

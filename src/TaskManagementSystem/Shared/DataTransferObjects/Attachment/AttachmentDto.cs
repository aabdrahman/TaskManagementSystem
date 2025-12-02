namespace Shared.DataTransferObjects.Attachment;

public record class AttachmentDto
{
    public int Id { get; set; }
    public string FilePath { get; set; }
    public string FileName { get; set; }
    public string TaskId { get; set; }
}

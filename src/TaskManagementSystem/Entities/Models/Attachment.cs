namespace Entities.Models;

public class Attachment : BaseEntity
{
    public string FilePath { get; set; }
    public string FileName { get; set; }
    public string TaskId { get; set; }
    public bool IsDeleted { get; set; }
}
using Entities.StaticValues;

namespace Entities.Models;

public class CreatedTask : BaseEntity
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime ProjectedCompletionDate { get; set; }
    public DateTime CompletionDate { get; set; }
    public PrirityLevel Priority { get; set; }
    public Stage TaskStage { get; set; }
    public string TaskId { get; set; }
    public bool IsDeleted { get; set; } = false;
    public string? CancelReason { get; set; }

    //RELATIONSHIP
    /* User Task Realationship */
    public ICollection<TaskUser> UserLink { get; set; } = [];
}


public class Attachment : BaseEntity
{
    public string FilePath { get; set; }
    public string FileName { get; set; }
    public string TaskId { get; set; }
    public bool IsDeleted { get; set; } = false;
}
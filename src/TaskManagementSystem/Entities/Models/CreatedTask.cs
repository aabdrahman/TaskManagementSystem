using Entities.StaticValues;

namespace Entities.Models;

public class CreatedTask : BaseEntity
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime ProjectedCompletionDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public PriorityLevel Priority { get; set; }
    public Stage TaskStage { get; set; }
    public string TaskId { get; set; }
    public bool IsDeleted { get; set; } = false;
    public string? CancelReason { get; set; }

    //RELATIONSHIP
    /* User Task Realationship */
    public ICollection<TaskUser> UserLink { get; set; } = [];

    /* Created User Relationship */
    public int UserId { get; set; }    
    public User CreatedByUser { get; set; }
}

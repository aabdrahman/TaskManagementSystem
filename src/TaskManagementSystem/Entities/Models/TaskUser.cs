namespace Entities.Models;

public class TaskUser : BaseEntity //This is the linking table for the manay to many relationship between tasks and users.
{
    public int TaskId { get; set; }
    public int UserId { get; set; }
    public CreatedTask task { get; set; }
    public User user { get; set; }

    //PROPERTIES
    public bool IsDeleted { get; set; }
    public DateTime? CompletionDate { get; set; }
    public string Title {  get; set; }
    public string Description { get; set; }
    public DateTime ProposedCompletionDate { get; set; }
}

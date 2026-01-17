namespace Entities.Models.Keyless_Entities;

public class UserUnitAssignedUserTaskAnalytics
{
    public int UserId { get; set; }
    public string FirstName { get; set; } = string.Empty!;
    public string LastName { get; set; } = string.Empty!;

    // Task Counts
    public int TotalAssignedTasks { get; set; }
    public int TotalPendingTasks { get; set; }
    public int TotalCancelledTasks { get; set; }
    public int TotalDeletedTasks { get; set; }
    public int TotalOverdueTasks { get; set; }
    public int TotalCompletedTasks { get; set; }
    public int CompletedAfterDueDate { get; set; }
    public int CompletedWithinDueDate { get; set; }

    // Task Timing
    public decimal CompletionRate { get; set; } // percentage 0-100
    public int? TotalTimeTaken { get; set; } // in minutes
}

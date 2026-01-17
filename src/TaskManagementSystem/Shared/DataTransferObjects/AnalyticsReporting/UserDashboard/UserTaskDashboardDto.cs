namespace Shared.DataTransferObjects.AnalyticsReporting.UserDashboard;

public class UserTaskDashboardDto
{
    public int OverDueTasks { get; set; } = 0;
    public int DueToday { get; set; } = 0;
    public int PendingTasks { get; set; } = 0;

    public IEnumerable<UserTaskSummaryDto> UserTasks { get; set; } = [];
}

public class UserTaskSummaryDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public DateTime DueDate { get; set; }
}
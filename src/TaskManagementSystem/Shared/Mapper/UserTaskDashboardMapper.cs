using Shared.DataTransferObjects.TaskUser;
using Shared.DataTransferObjects.UserDashboard;

namespace Shared.Mapper;

public static class UserTaskDashboardMapper
{
	public static UserTaskDashboardDto ToUserDashboardDto(this IEnumerable<TaskUserDto> taskUsers)
	{
		return new UserTaskDashboardDto()
		{
			OverDueTasks = taskUsers.Count(x => x.ProposedCompletionDate <= DateTime.UtcNow),
			DueToday = taskUsers.Count(x => x.ProposedCompletionDate >= DateTime.UtcNow && x.ProposedCompletionDate < DateTime.UtcNow.AddDays(1)),
			PendingTasks = taskUsers.Count(x => x.ProposedCompletionDate >= DateTime.UtcNow.AddDays(1)),
			UserTasks = taskUsers.Select(x => new UserTaskSummaryDto() { Id = x.Id, DueDate = x.ProposedCompletionDate, Title = x.Title })
		};
	}
}

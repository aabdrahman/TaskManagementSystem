using Entities.Models;
using Shared.DataTransferObjects.TaskUser;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Shared.Mapper;

public static class TaskUserMapper
{
    public static TaskUser ToEntity(this CreateTaskUserDto createTaskUser)
    {
        return new TaskUser()
        {
            TaskId = createTaskUser.PrimaryTaskId,
            Title = createTaskUser.Title,
            Description = createTaskUser.Description,
            UserId = createTaskUser.AssignedUser,
            ProposedCompletionDate = createTaskUser.ProposedCompletionDate,
            CreatedBy = ""
        };
    }

    public static TaskUserDto ToDto(this TaskUser taskUser)
    {
        return new TaskUserDto()
        {
            Id = taskUser.Id,
            Title = taskUser.Title,
            Description = taskUser.Description,
            TaskId = taskUser?.task?.TaskId ?? "",
            CompletionDate = taskUser?.CompletionDate,
            AssignedUser = taskUser?.user?.FirstName + " " + taskUser?.user?.LastName ?? "",
            PrimaryTaskId = taskUser.TaskId,
            ProposedCompletionDate = taskUser.ProposedCompletionDate,
            CancelReason = taskUser?.CancelReason ?? null,
            UserId = taskUser.UserId
        };
    }

    public static Expression<Func<TaskUser, TaskUserDto>> ToDtoExpression()
    {
        return taskUser => new TaskUserDto
        {
            Id = taskUser.Id,
            Title = taskUser.Title,
            Description = taskUser.Description,

            TaskId = taskUser.task != null
                ? taskUser.task.TaskId
                : "",

            CompletionDate = taskUser.CompletionDate,

            AssignedUser = taskUser.user != null
                ? taskUser.user.FirstName + " " + taskUser.user.LastName
                : "",

            PrimaryTaskId = taskUser.TaskId,
            ProposedCompletionDate = taskUser.ProposedCompletionDate,
            CancelReason = taskUser.CancelReason,
            UserId = taskUser.UserId
        };
    }

}

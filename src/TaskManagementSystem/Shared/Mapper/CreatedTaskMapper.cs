using Entities.Models;
using Entities.StaticValues;
using Shared.DataTransferObjects.CreatedTask;
using System.Linq.Expressions;

namespace Shared.Mapper;

public static class CreatedTaskMapper
{
    public static CreatedTaskDto ToDto(this CreatedTask createdTask)
    {
        return new CreatedTaskDto()
        {
            Id = createdTask.Id,
            Title = createdTask.Title,
            Description = createdTask.Description,
            ProposedCompletionDate = createdTask.ProjectedCompletionDate,
            CompletionDate = createdTask?.CompletionDate,
            Priority = createdTask.Priority.ToString(),
            Stage = createdTask.TaskStage.ToString(),
            IsActive = !createdTask.IsDeleted,
            TaskId = createdTask.TaskId,
            CancelReason = createdTask?.CancelReason ?? null,
        };
    }

    public static CreatedTask ToEntity(this CreateTaskDto newCreateTask)
    {
        return new CreatedTask()
        {
            Title = newCreateTask.Title,
            Description = newCreateTask.Description,
            ProjectedCompletionDate = newCreateTask.ProposedCompletionDate,
            Priority = Enum.Parse<PriorityLevel>(newCreateTask?.Priority.ToString(), ignoreCase: true),
            CreatedBy = "",
            UserId = newCreateTask.UserId
        };
    }

    public static Expression<Func<CreatedTask, CreatedTaskDto>> ToDtoExpression()
    {
        return createdTask => new CreatedTaskDto()
        {
            Id = createdTask.Id,
            Title = createdTask.Title,
            Description = createdTask.Description,
            ProposedCompletionDate = createdTask.ProjectedCompletionDate,
            CompletionDate = createdTask.CompletionDate,
            Priority = createdTask.Priority.ToString(),
            Stage = createdTask.TaskStage.ToString(),
            IsActive = !createdTask.IsDeleted,
            TaskId = createdTask.TaskId,
            CancelReason = createdTask.CancelReason ?? null,
        };
    }
}

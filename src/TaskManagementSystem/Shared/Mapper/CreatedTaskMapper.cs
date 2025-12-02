using Entities.Models;
using Entities.StaticValues;
using Shared.DataTransferObjects.CreatedTask;

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
            TaskId = createdTask.TaskId
        };
    }

    public static CreatedTask ToEntity(this CreateTaskDto newCreateTask)
    {
        return new CreatedTask()
        {
            Title = newCreateTask.Title,
            Description = newCreateTask.Description,
            ProjectedCompletionDate = newCreateTask.ProposedCompletionDate,
            Priority = Enum.Parse<PriorityLevel>(newCreateTask?.Priority.ToString()),
            CreatedBy = "",
            UserId = newCreateTask.UserId
        };
    }
}

using Contracts;
using Entities.Models;

namespace Repository;

public class TaskUserRepository : RepositoryBase<TaskUser>, ITaskUserRepository
{
    public TaskUserRepository(RepositoryContext repositoryContext) : base(repositoryContext)
    {
    }

    public async Task CreateTaskUser(TaskUser newTaskUser)
    {
        await Create(newTaskUser);
    }

    public void DeleteTaskUser(TaskUser deletedTaskUser)
    {
        DeleteEntity(deletedTaskUser);
    }

    public IQueryable<TaskUser> GetAllTasks(bool trackChanges = true, bool hasQueryFilter = true)
    {
        return FindAll(trackChanges, hasQueryFilter);
    }

    public IQueryable<TaskUser> GetByTaskId(int taskId, bool trackChanges = true, bool hasQueryFilter = true)
    {
        return FindByCondition(x => x.TaskId == taskId, trackChanges, hasQueryFilter);
    }

    public IQueryable<TaskUser> GetByUserId(int userId, bool trackChanges = true, bool hasQueryFilter = true)
    {
        return FindByCondition(x => x.UserId == userId, trackChanges, hasQueryFilter);
    }

    public IQueryable<TaskUser> GetPendingTaskByUser(int userId, bool trackChanges = true, bool hasQueryFilter = true)
    {
        return FindByCondition(x => x.UserId == userId && !x.CompletionDate.HasValue, trackChanges, hasQueryFilter);
    }

    public IQueryable<TaskUser> GetTaskUserById(int Id, bool trackChanges = true, bool hasQueryFilter = true)
    {
        return FindByCondition(x => x.Id == Id, trackChanges, hasQueryFilter);
    }

    public void UpdateTaskUser(TaskUser updatedTaskUser)
    {
        UpdateEntity(updatedTaskUser);
    }
}

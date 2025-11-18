using Contracts;
using Entities.Models;
using Entities.StaticValues;

namespace Repository;

public sealed class CreatedTaskRepository : RepositoryBase<CreatedTask>, ICreatedTaskRepository
{
    public CreatedTaskRepository(RepositoryContext repositoryContext) : base(repositoryContext)
    {
    }

    public async Task CreateTask(CreatedTask newCreatedTask)
    {
        await Create(newCreatedTask);
    }

    public void DeleteTask(CreatedTask deletedCreatedTask)
    {
        DeleteEntity(deletedCreatedTask);
    }

    public IQueryable<CreatedTask> GetAllTasks(bool trackChanges = true, bool hasQueryFilter = true)
    {
        return FindAll(trackChanges, hasQueryFilter);
    }

    public IQueryable<CreatedTask> GetById(int id, bool trackChanges = true, bool hasQueryFilter = true)
    {
        return FindByCondition(x => x.Id ==  id, trackChanges, hasQueryFilter);
    }

    public IQueryable<CreatedTask> GetByPriority(PrirityLevel priorityLevel, bool trackChanges = true, bool hasQueryFilter = true)
    {
        return FindByCondition(x => x.Priority == priorityLevel, trackChanges, hasQueryFilter);
    }

    public IQueryable<CreatedTask> GetByStatus(Stage stage, bool trackChanges = true, bool hasQueryFilter = true)
    {
        return FindByCondition(x => x.TaskStage == stage, trackChanges, hasQueryFilter);
    }

    public IQueryable<CreatedTask> GetByTaskId(string taskId, bool trackChanges = true, bool hasQueryFilter = true)
    {
        return FindByCondition(x => x.TaskId == taskId, trackChanges, hasQueryFilter);
    }

    public void UpdateTask(CreatedTask updatedCreatedTask)
    {
        UpdateEntity(updatedCreatedTask);
    }
}

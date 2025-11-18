using Entities.Models;
using Entities.StaticValues;

namespace Contracts;

public interface ICreatedTaskRepository
{
    Task CreateTask(CreatedTask newCreatedTask);
    void UpdateTask(CreatedTask updatedCreatedTask);
    void DeleteTask(CreatedTask deletedCreatedTask);
    IQueryable<CreatedTask> GetAllTasks(bool trackChanges = true, bool hasQueryFilter = true);
    IQueryable<CreatedTask> GetByPriority(PrirityLevel priorityLevel, bool trackChanges = true, bool hasQueryFilter = true);
    IQueryable<CreatedTask> GetByStatus(Stage stage, bool trackChanges = true, bool hasQueryFilter = true);
    IQueryable<CreatedTask> GetById(int id, bool trackChanges = true, bool hasQueryFilter = true);
    IQueryable<CreatedTask> GetByTaskId(string taskId, bool trackChanges = true, bool hasQueryFilter = true);
}

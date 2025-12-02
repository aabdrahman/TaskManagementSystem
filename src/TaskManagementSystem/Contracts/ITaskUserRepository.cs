using Entities.Models;

namespace Contracts;

public interface ITaskUserRepository
{
    Task CreateTaskUser(TaskUser newTaskUser);
    void UpdateTaskUser(TaskUser updatedTaskUser);
    void DeleteTaskUser(TaskUser deletedTaskUser);
    IQueryable<TaskUser> GetAllTasks(bool trackChanges = true, bool hasQueryFilter = true);
    IQueryable<TaskUser> GetTaskUserById(int Id, bool trackChanges = true, bool hasQueryFilter = true);
    IQueryable<TaskUser> GetByUserId(int userId, bool trackChanges = true, bool hasQueryFilter = true);
    IQueryable<TaskUser> GetByTaskId(int taskId, bool trackChanges = true, bool hasQueryFilter = true);
    IQueryable<TaskUser> GetPendingTaskByUser(int userId, bool trackChanges = true, bool hasQueryFilter = true);
}

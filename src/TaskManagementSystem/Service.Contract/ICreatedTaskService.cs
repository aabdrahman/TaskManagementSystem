using Shared.ApiResponse;
using Shared.DataTransferObjects.CreatedTask;

namespace Service.Contract;

public interface ICreatedTaskService
{
    Task<GenericResponse<CreatedTaskDto>> CreateAsync(CreateTaskDto createTask);
    Task<GenericResponse<CreatedTaskDto>> GetByIdAsync(string taskId, bool trackChanges = false, bool hasQueryFilter = true);
    Task<GenericResponse<IEnumerable<CreatedTaskDto>>> GetByUserAsync(int UserId, bool trackChanges = false, bool hasQueryFilter = true);
    Task<GenericResponse<IEnumerable<CreatedTaskDto>>> GetByStatusAsync(string taskStatus, bool trackChanges = false, bool hasQueryFilter = true);
    Task<GenericResponse<IEnumerable<CreatedTaskDto>>> GetByPriorityAsync(string taskPriority, bool trackChanges = false, bool hasQueryFilter = true);
    Task<GenericResponse<string>> DeleteAsync(int taskId, bool isSoftDelete = true);
    Task<GenericResponse<string>> CancelTaskAsync(CancelCreatedTaskDto cancelCreatedTaskDto);
    Task<GenericResponse<string>> ReassignTaskAsync(ReassignCreatedTaskDto reassignCreatedTaskDto);
}

using Shared.ApiResponse;
using Shared.DataTransferObjects.TaskUser;

namespace Service.Contract;

public interface ITaskUserService
{
    Task<GenericResponse<TaskUserDto>> AssignUserToTask(CreateTaskUserDto taskUserDto);
    Task<GenericResponse<IEnumerable<TaskUserDto>>> GetAssignedTasksToUser(int UserId);
    Task<GenericResponse<IEnumerable<TaskUserDto>>> GetAssignedTasksByTaskId(int TaskId);
    Task<GenericResponse<string>> RemoveAssignedTask(int Id, bool isSoftDelete = true);
}

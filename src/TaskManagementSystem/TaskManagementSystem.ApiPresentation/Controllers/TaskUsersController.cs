using Contracts.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Service.Contract;
using Shared.DataTransferObjects.TaskUser;

namespace TaskManagementSystem.ApiPresentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TaskUsersController : ControllerBase
{
    private readonly IServiceManager _serviceManager;
    private readonly ILoggerManager _loggerManager;

    public TaskUsersController(IServiceManager serviceManager, ILoggerManager loggerManager)
    {
        _serviceManager = serviceManager;
        _loggerManager = loggerManager;
    }

    [HttpGet("userid/{userId:int}")]
    public async Task<IActionResult> GetByUser(int userId, bool hasQueryFilter = true)
    {
        try
        {
            var getByUserResponse = await _serviceManager.TaskUserService.GetAssignedTasksToUser(userId);

            return StatusCode((int)getByUserResponse.StatusCode, getByUserResponse);    
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("taskid/{taskId:int}")]
    public async Task<IActionResult> GetByTaskId(int taskId, bool hasQueryFilter = true)
    {
        try
        {
            var getByTaskIdResponse = await _serviceManager.TaskUserService.GetAssignedTasksByTaskId(taskId);

            return StatusCode((int)getByTaskIdResponse.StatusCode, getByTaskIdResponse);
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete("{Id:int}")]
    public async Task<IActionResult> RemoveTaskUser(int Id, bool isSoftDelete = true)
    {
        try
        {
            var removeTaskUserResonse = await _serviceManager.TaskUserService.RemoveAssignedTask(Id, isSoftDelete);

            return StatusCode((int)removeTaskUserResonse.StatusCode, removeTaskUserResonse);
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> AssignTaskToUser([FromBody] CreateTaskUserDto createTaskUser)
    {
        try
        {
            var assignTaskToUserResponse = await _serviceManager.TaskUserService.AssignUserToTask(createTaskUser);

            return StatusCode((int)assignTaskToUserResponse.StatusCode, assignTaskToUserResponse);
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("cancel-user-task")]
    public async Task<IActionResult> CancelUserTask([FromBody]CancelUserTaskDto cancelTask)
    {
        try
        {
            var cancelUserTaskResponse = await _serviceManager.TaskUserService.CancelTask(cancelTask);

            return StatusCode((int)cancelUserTaskResponse.StatusCode, cancelUserTaskResponse);
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, ex.Message);
            return StatusCode(500, ex.Message);
        }
    }
    [HttpPost("reassign-user-task")]
    public async Task<IActionResult> ReassignUserTask([FromBody] ReassignTaskUserDto reassignTaskUser)
    {
        try
        {
            var reassignUserTaskResponse = await _serviceManager.TaskUserService.ReassignTaskToUser(reassignTaskUser);

            return StatusCode((int)reassignUserTaskResponse.StatusCode, reassignUserTaskResponse);
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, ex.Message);
            return StatusCode(500, ex.Message);
        }
    }
}

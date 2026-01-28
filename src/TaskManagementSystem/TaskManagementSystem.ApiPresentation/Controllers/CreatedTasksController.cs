using Contracts.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Contract;
using Shared.DataTransferObjects.CreatedTask;

namespace TaskManagementSystem.ApiPresentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CreatedTasksController : ControllerBase
{
    private readonly IServiceManager _serviceManager;
    private readonly ILoggerManager _loggerManager;

    public CreatedTasksController(IServiceManager serviceManager, ILoggerManager loggerManager)
    {
        _serviceManager = serviceManager;
        _loggerManager = loggerManager;
    }

    [HttpGet("taskid/{taskId}")]
    [Authorize(Policy = "ProductOwnerOrAdminPolicy")]
    public async Task<IActionResult> GetByTaskId(string taskId, bool hasQueryFilter = true)
    {
        try
        {
            var getByTaskIdResponse = await _serviceManager.CreatedTaskService.GetByIdAsync(taskId, false, hasQueryFilter);

            return StatusCode((int)getByTaskIdResponse.StatusCode, getByTaskIdResponse);
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, "Internal Server Error");
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpGet("status/{status}")]
    //[Authorize(Policy = "ProductOwnerPolicy")]
    [Authorize(Policy = "AdminPolicy")]

    public async Task<IActionResult> GetByStatus(string status, bool hasQueryFilter = true)
    {
        try
        {
            var getByStatusResponse = await _serviceManager.CreatedTaskService.GetByStatusAsync(status, false, hasQueryFilter);

            return StatusCode((int)getByStatusResponse.StatusCode, getByStatusResponse);
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, "Internal Server Error");
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpGet("userid/{userid:int}")]
    [Authorize(Policy = "UnitHeadOrAdminOrProductOwnerPolicy")]
    public async Task<IActionResult> GetByUser(int userid, bool hasQueryFilter = true)
    {
        try
        {
            var getByStatusResponse = await _serviceManager.CreatedTaskService.GetByUserAsync(userid, false, hasQueryFilter);

            return StatusCode((int)getByStatusResponse.StatusCode, getByStatusResponse);
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, "Internal Server Error");
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpGet("priority/{taskPriority}")]
    [Authorize(Policy = "ProductOwnerOrAdminPolicy")]
    public async Task<IActionResult> GetByPriority(string taskPriority, bool hasQueryFilter = true)
    {
        try
        {
            var getByStatusResponse = await _serviceManager.CreatedTaskService.GetByPriorityAsync(taskPriority, false, hasQueryFilter);

            return StatusCode((int)getByStatusResponse.StatusCode, getByStatusResponse);
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, "Internal Server Error");
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpPost]
    [Authorize(Policy = "ProductOwnerOrAdminPolicy")]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto createTask)
    {
        try
        {
            var createTaskResponse = await _serviceManager.CreatedTaskService.CreateAsync(createTask);

            return StatusCode((int)createTaskResponse.StatusCode, createTaskResponse);
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, "Internal Server Error");
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpDelete("{Id}")]
    [Authorize(Policy = "ProductOwnerOrAdminPolicy")]
    public async Task<IActionResult> Delete(int Id, bool isSoftDelete = true)
    {
        try
        {
            var deleteResponse = await _serviceManager.CreatedTaskService.DeleteAsync(Id, isSoftDelete);

            return StatusCode((int)deleteResponse.StatusCode, deleteResponse);
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, "Internal Server Error");
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpPost("cancel-task")]
    [Authorize(Policy = "ProductOwnerOrAdminPolicy")]
    public async Task<IActionResult> CancelTask([FromBody] CancelCreatedTaskDto cancelCreatedTask)
    {
        try
        {
            var cancelTaskResponse = await _serviceManager.CreatedTaskService.CancelTaskAsync(cancelCreatedTask);

            return StatusCode((int)cancelTaskResponse.StatusCode, cancelTaskResponse);
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, "Internal Server Error");
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpPost("reassign-task")]
    [Authorize(Policy = "UnitHeadOrAdminOrProductOwnerPolicy")]
    public async Task<IActionResult> ReassignTask([FromBody]  ReassignCreatedTaskDto reassignCreatedTask)
    {
        try
        {
            var reassignTaskResponse = await _serviceManager.CreatedTaskService.ReassignTaskAsync(reassignCreatedTask);

            return StatusCode((int)reassignTaskResponse.StatusCode, reassignTaskResponse);
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, "Internal Server Error");
            return StatusCode(500, "Internal Server Error");
        }
    }
    
    [HttpPut("update-task")]
    [Authorize(Policy = "ProductOwnerOrAdminPolicy")]
    public async Task<IActionResult> UpdateTask([FromBody] UpdateCreatedTaskDto updateCreatedTask)
    {
        try
        {
            var updateCreatedTaskResponse = await _serviceManager.CreatedTaskService.UpdateCreatedTaskAsync(updateCreatedTask);

            return StatusCode((int)updateCreatedTaskResponse.StatusCode, updateCreatedTaskResponse);
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, "Internal Server Error");
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpPut("update-task-status")]
    [Authorize(Policy = "ProductOwnerOrAdminPolicy")]
    public async Task<IActionResult> UpdateTaskStatus([FromBody] UpdateCreatedTaskStatusDto updateCreatedTaskStatus)
    {
        try
        {
            var updateCreatedTaskStatusResponse = await _serviceManager.CreatedTaskService.UpdateCreatedTaskStatus(updateCreatedTaskStatus);

            return StatusCode((int)updateCreatedTaskStatusResponse.StatusCode, updateCreatedTaskStatusResponse);
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, "Internal Server Error");
            return StatusCode(500, "Internal Server Error");
        }
    }
}

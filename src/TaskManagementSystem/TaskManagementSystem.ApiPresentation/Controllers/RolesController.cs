using Contracts.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Service.Contract;
using Shared.DataTransferObjects.Role;

namespace TaskManagementSystem.ApiPresentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly IServiceManager _serviceManager;
    private readonly ILoggerManager _loggerManager;

    public RolesController(IServiceManager serviceManager, ILoggerManager loggerManager)
    {
        _serviceManager = serviceManager;
        _loggerManager = loggerManager;
    }
    [HttpGet]
    public async Task<IActionResult> GetAllRoles(bool hasQueyFilter = true)
    {
        try
        {
            var getAllRolesResponse = await _serviceManager.RoleService.GetAllAsync(false, hasQueyFilter);

            return StatusCode((int)getAllRolesResponse.StatusCode, getAllRolesResponse);
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, "Error Occurred");
            return StatusCode(500, $"{ex.Message}");
        }
    }

    [HttpGet("{Id:int}")]
    public async Task<IActionResult> GetById(int Id, bool hasQueryFilter = false)
    {
        try
        {
            var getRoleByIdResponse = await _serviceManager.RoleService.GetByIdAsync(Id, false, hasQueryFilter);

            return StatusCode((int)getRoleByIdResponse.StatusCode, getRoleByIdResponse);
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, "Error Occurred");
            return StatusCode(500, $"{ex.Message}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRoleDto createRole)
    {
        try
        {
            var createRoleResponse = await _serviceManager.RoleService.CreateAsync(createRole);

            return StatusCode((int)createRoleResponse.StatusCode, createRoleResponse);
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, "Error Occurred");
            return StatusCode(500, $"{ex.Message}");
        }
    }

    [HttpDelete("{Id:int}")]
    public async Task<IActionResult> Delete(int Id, bool isSoftDelete = false)
    {
        try
        {
            var deleteRoleResponse = await _serviceManager.RoleService.DeleteAsync(Id, isSoftDelete);

            return StatusCode((int)deleteRoleResponse.StatusCode, deleteRoleResponse);
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, "Error Occurred");
            return StatusCode(500, $"{ex.Message}");
        }
    }
}

using Contracts.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Contract;
using Shared.DataTransferObjects.Unit;

namespace TaskManagementSystem.ApiPresentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UnitsController : ControllerBase
{
    private readonly IServiceManager _serviceManager;
    private readonly ILoggerManager _loggerManager;

    public UnitsController(IServiceManager serviceManager, ILoggerManager loggerManager)
    {
        _serviceManager = serviceManager;
        _loggerManager = loggerManager;
    }


    [HttpGet]
    [Authorize(Policy = "UnitHeadOrAdminOrProductOwnerPolicy")]
    public async Task<IActionResult> GetAllUnits(bool hasQueryFilter = true)
    {
        try
        {
            var result = await _serviceManager.UnitService.GetAllUnitsAsync(false, hasQueryFilter);

            return StatusCode((int)result.StatusCode, result);
                
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, "An Error Occurred.");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{Id:int}")]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<IActionResult> GetUnitById(int Id, bool hasQueryFilter = true)
    {
        try
        {
            var result = await _serviceManager.UnitService.GetByIdAsuync(Id);

            return StatusCode((int)result.StatusCode, result);
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, "An Error Occurred.");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPut]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<IActionResult> UpdateUnit([FromBody] UpdateUnitDto updateUnit)
    {
        try
        {
            var updateUnitResponse = await _serviceManager.UnitService.UpdateUnitAsync(updateUnit);

            return StatusCode((int)updateUnitResponse.StatusCode, updateUnitResponse);
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, "An Error Occurred.");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<IActionResult> Create([FromBody] CreateUnitDto newUnitToCreate)
    {
        try
        {
            var result = await _serviceManager.UnitService.CreateAsync(newUnitToCreate);

            return StatusCode((int)result.StatusCode, result);
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, "An Error Occurred.");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete("{Id:int}")]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<IActionResult> Delete(int Id, bool isSoftDelete = false)
    {
        try
        {
            var result = await _serviceManager.UnitService.DeleteAsync(Id, isSoftDelete);

            return StatusCode((int)result.StatusCode, result);
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, "An Error Occurred.");
            return StatusCode(500, ex.Message);
        }
    }
}

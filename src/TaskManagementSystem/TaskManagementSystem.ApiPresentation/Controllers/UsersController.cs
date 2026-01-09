using Contracts.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Service.Contract;
using Shared.DataTransferObjects.User;
using Shared.RequestParameters;
using System.Text.Json;

namespace TaskManagementSystem.ApiPresentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IServiceManager _serviceManager;
    private readonly ILoggerManager _loggerManager;
    public UsersController(IServiceManager serviceManager, ILoggerManager loggerManager)
    {
        _serviceManager = serviceManager;
        _loggerManager = loggerManager;
    }

    [HttpGet("{username}")]
    public async Task<IActionResult> GetByUsername(string username, bool hasQueryFilter = true)
    {
        try
        {
            var getUserByUsernameResponse = await _serviceManager.UserService.GetByUsernameAsync(username, false, hasQueryFilter);

            return StatusCode((int)getUserByUsernameResponse.StatusCode, getUserByUsernameResponse);
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, "An Error Occurred.");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("getByUnitId/{UnitId:int}")]
    public async Task<IActionResult> GetUsersByUnitId(int UnitId, bool hasQueryFilter = true)
    {
        try
        {
            var getUsersByUnitIdResponse = await _serviceManager.UserService.GetByUnitAsync(UnitId, false, hasQueryFilter);

            return StatusCode((int)getUsersByUnitIdResponse.StatusCode, getUsersByUnitIdResponse);
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, "An Error Occurred.");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("getUserSummaryDetails")]
    public async Task<IActionResult> GetUserSummaryDetails([FromQuery] UsersRequestParameter usersRequestParameter, bool hasQueryFilter = true)
    {
        try
        {
            var getUserResponse = await _serviceManager.UserService.GetAllUsers(usersRequestParameter, hasQueryFilter);
            if(getUserResponse.IsSuccessful)
                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(getUserResponse?.Data?.metaData ?? null));

            return StatusCode((int)getUserResponse.StatusCode, getUserResponse);
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, "An Error Occurred.");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{Id:int}")]
    public async Task<IActionResult> GetById(int Id, bool hasQueryFilter = true)
    {
        try
        {
            var getByIdResponse = await _serviceManager.UserService.GetByIdAsync(Id, false, hasQueryFilter);

            return StatusCode((int)getByIdResponse.StatusCode, getByIdResponse);
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, "An Error Occurred.");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("getUnitMember/{UserId:int}")]
    public async Task<IActionResult> GetUnitMember(int UserId)
    {
        try
        {
            var getUnitMemberResponse = await _serviceManager.UserService.GetUsersWithSameUnit(UserId);

            return StatusCode((int)getUnitMemberResponse.StatusCode, getUnitMemberResponse);
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, "An Error Occurred.");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] CreateUserDto createUser)
    {
        try
        {
            var registerUserResponse = await _serviceManager.UserService.CreateAsync(createUser);

            return StatusCode((int)registerUserResponse.StatusCode, registerUserResponse);
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, "An Error Occurred.");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete("{Username}")]
    public async Task<IActionResult> DeleteUser(string Username, bool isSoftDelete = false)
    {
        try
        {
            var deleteUserResponse = await _serviceManager.UserService.DeleteAsync(Username, isSoftDelete);

            return StatusCode((int)deleteUserResponse.StatusCode, deleteUserResponse);
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, "An Error Occurred.");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangeUserPasswordDto changePassword)
    {
        try
        {
            var changePasswordResponse = await _serviceManager.UserService.ChangePasswordAsync(changePassword);

            return StatusCode((int)changePasswordResponse.StatusCode, changePasswordResponse);
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, "An Error Occurred.");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserToLoginDto userToLogin)
    {
        try
        {
            var loginResponse = await _serviceManager.UserService.ValidateUserAsync(userToLogin);

            return StatusCode((int)loginResponse.StatusCode, loginResponse);
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, "An Error Occurred.");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] TokenDto tokenDto)
    {
        try
        {
            var refreshResponse = await _serviceManager.UserService.RefreshTokenAsync(tokenDto);

            return StatusCode((int)refreshResponse.StatusCode, refreshResponse);
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, "An Error Occurred.");
            return StatusCode(500, ex.Message);
        }
    }
}

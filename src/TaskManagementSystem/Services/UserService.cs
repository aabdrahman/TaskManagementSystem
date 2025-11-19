using Contracts;
using Contracts.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Service.Contract;
using Shared.ApiResponse;
using Shared.DataTransferObjects.User;
using System.Data.Common;
using System.Text.Json;
using System.Net;
using Entities.Models;
using Shared.Mapper;
using BCrypt.Net;

namespace Services;

public sealed class UserService : IUserService
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly ILoggerManager _loggerManager;
    public UserService(IRepositoryManager repositoryManager, ILoggerManager loggerManager)
    {
        _repositoryManager = repositoryManager;
        _loggerManager = loggerManager;
    }
    public async Task<GenericResponse<UserDto>> CreateAsync(CreateUserDto createUserDto)
    {
        try
        {
            await _loggerManager.LogInfo($"Creating User - {SerializeObject(createUserDto)}");
            
            bool userEmailExists = await _repositoryManager.UserRepository.GetByEmail(createUserDto.Email, false, false)
                                            .AnyAsync();
            if (userEmailExists)
            {
                await _loggerManager.LogWarning($"Email already taken: {createUserDto.Email}");
                return GenericResponse<UserDto>.Failure(null, HttpStatusCode.Conflict, $"Email already exists.", null);
            }
            
            bool userNameExists = await _repositoryManager.UserRepository.GetByUserName(createUserDto.Username, false, false)
                                            .AnyAsync();

            if (userNameExists)
            {
                await _loggerManager.LogWarning($"Username already taken: {createUserDto.Username}");
                return GenericResponse<UserDto>.Failure(null, HttpStatusCode.Conflict, $"Username already taken.", null);
            }

            bool isExistRole = await _repositoryManager.RoleRepository.GetAllRoles(false, true)
                                            .AnyAsync(x => x.Id == createUserDto.AssignedRole);

            if (!isExistRole)
            {
                await _loggerManager.LogWarning($"Invalid Role provided for creating user: {createUserDto.AssignedRole}");
                return GenericResponse<UserDto>.Failure(null, HttpStatusCode.BadRequest, $"Role does not exist", null);
            }

            bool isExistUnit = await _repositoryManager.UnitRepository.GetAllUnits(false, true)
                                            .AnyAsync(x => x.Id == createUserDto.AssignedUnit);
            if (!isExistUnit)
            {
                await _loggerManager.LogWarning($"Invalid Unit provided for creating user: {createUserDto.AssignedUnit}");
                return GenericResponse<UserDto>.Failure(null, HttpStatusCode.Conflict, $"Unit does not exist.", null);
            }

            User userToInsert = createUserDto.ToEntity();
            userToInsert.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(createUserDto.Password.Trim());

            await _repositoryManager.UserRepository.CreateUser(userToInsert);

            await _repositoryManager.SaveChangesAsync();

            await _loggerManager.LogInfo($"User Creation Successful - {SerializeObject(userToInsert.ToDto())}");

            return GenericResponse<UserDto>.Success(userToInsert.ToDto(), HttpStatusCode.Created, $"User creation successful.");

        }
        catch(DbException ex)
        {
            await _loggerManager.LogError(ex, "Internal Server Error - Database");
            return GenericResponse<UserDto>.Failure(null, HttpStatusCode.InternalServerError, $"An Error Occurred - Database", new { ex.Message, Description = ex?.InnerException?.Message });
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, "Internal Server Error");
            return GenericResponse<UserDto>.Failure(null, HttpStatusCode.InternalServerError, $"An Error Occurred.", new { ex.Message, Description = ex?.InnerException?.Message });
        }
    }

    public async Task<GenericResponse<string>> DeleteAsync(string Username, bool isSoftDelete = true)
    {
        try
        {
            await _loggerManager.LogInfo($"Deleting User with username - {Username}");

            User? existingUser = await _repositoryManager.UserRepository.GetByUserName(Username, true, false)
                                                .SingleOrDefaultAsync();
            if (existingUser is not null)
            {
                await _loggerManager.LogWarning($"User with specified username does not exist - {Username}");
                return GenericResponse<string>.Failure("Operation Failed", HttpStatusCode.NotFound, $"User with provided username does not exist.", null);
            }

            if(isSoftDelete)
            {
                await _loggerManager.LogInfo($"Marking User with Username: {Username} as inactive");
                existingUser.IsActive = false;
                _repositoryManager.UserRepository.UpdateUser(existingUser);
            }
            else
            {
                _repositoryManager.UserRepository.DeleteUser(existingUser);
            }

            await _repositoryManager.SaveChangesAsync();
            await _loggerManager.LogInfo(isSoftDelete ? $"User with username: {Username} soft deletion successful" : $"User Removed Successfully.");

            return GenericResponse<string>.Success("Operation Successful", HttpStatusCode.OK, isSoftDelete ? "User marked as inactive" : "User deleted successfully.");

        }
        catch (DbException ex)
        {
            await _loggerManager.LogError(ex, $"Internal Server Error - Database");
            return GenericResponse<string>.Failure("Operation Failed", HttpStatusCode.InternalServerError, $"An Error Occurred - Database", new { ex.Message, Description = ex?.InnerException?.Message });
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, $"Internal Server Error");
            return GenericResponse<string>.Failure("Operation Failed", HttpStatusCode.InternalServerError, $"An Error Occurred - Database", new { ex.Message, Description = ex?.InnerException?.Message });
        }
    }

    public async Task<GenericResponse<UserDto>> GetByIdAsync(int Id, bool trackChanges = false, bool hasQueryFilter = true)
    {
        try
        {
            UserDto? existingUser = await _repositoryManager.UserRepository.GetById(Id, trackChanges, hasQueryFilter)
                                                    .Select(x => x.ToDto())
                                                    .SingleOrDefaultAsync();
            if (existingUser is null)
            {
                await _loggerManager.LogWarning($"User with specified Id does not exist - {Id}");
                return GenericResponse<UserDto>.Failure(null, HttpStatusCode.NotFound, $"User with provided Id does not exist.", null);
            }

            await _loggerManager.LogInfo($"User with username: {Id} fetched successfully - {SerializeObject(existingUser)}");

            return GenericResponse<UserDto>.Success(existingUser, HttpStatusCode.OK, "User Fetched Successfully.");
        }
        catch (DbException ex)
        {
            await _loggerManager.LogError(ex, $"Internal Server Error - Database");
            return GenericResponse<UserDto>.Failure(null, HttpStatusCode.InternalServerError, $"An Error Occurred - Database", new { ex.Message, Description = ex?.InnerException?.Message });
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, $"Internal Server Error");
            return GenericResponse<UserDto>.Failure(null, HttpStatusCode.InternalServerError, $"An Error Occurred - Database", new { ex.Message, Description = ex?.InnerException?.Message });
        }
    }

    public async Task<GenericResponse<IEnumerable<UserDto>>> GetByUnitAsync(int unitId, bool trackChanges = false, bool hasQueryFilter = true)
    {
        try
        {
            await _loggerManager.LogInfo($"Fetcing Users for unit - {unitId}");

            IEnumerable<UserDto> allUsers = await _repositoryManager.UserRepository.GetAllUsers(trackChanges, hasQueryFilter)
                                        .Where(x => x.UnitId == unitId)
                                        .Select(x => x.ToDto())
                                        .ToListAsync();

            await _loggerManager.LogInfo($"Users with Id: {unitId} fetched successfully - {SerializeObject(allUsers)}");

            return GenericResponse<IEnumerable<UserDto>>.Success(allUsers, HttpStatusCode.OK, "Users fetched successfully.");

        }
        catch (DbException ex)
        {
            await _loggerManager.LogError(ex, $"Internal Server Error - Database");
            return GenericResponse<IEnumerable<UserDto>>.Failure(null, HttpStatusCode.InternalServerError, $"An Error Occurred - Database", new { ex.Message, Description = ex?.InnerException?.Message });
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, $"Internal Server Error");
            return GenericResponse<IEnumerable<UserDto>>.Failure(null, HttpStatusCode.InternalServerError, $"An Error Occurred - Database", new { ex.Message, Description = ex?.InnerException?.Message });
        }
    }

    public async Task<GenericResponse<UserDto>> GetByUsernameAsync(string Username, bool trackChanges = false, bool hasQueryFilter = true)
    {
        try
        {
            await _loggerManager.LogInfo($"Getting User with Username - {Username}");

            UserDto? existingUser = await _repositoryManager.UserRepository.GetByUserName(Username.Trim(), trackChanges, hasQueryFilter)
                                                    .Select(x => x.ToDto())
                                                    .FirstOrDefaultAsync();
            if (existingUser is null)
            {
                await _loggerManager.LogWarning($"User with specified username does not exist - {Username}");
                return GenericResponse<UserDto>.Failure(null, HttpStatusCode.NotFound, $"User with provided username does not exist.", null);
            }

            await _loggerManager.LogInfo($"User with username: {Username} fetched successfully - {SerializeObject(existingUser)}");

            return GenericResponse<UserDto>.Success(existingUser, HttpStatusCode.OK, "User Fetched Successfully.");
        }
        catch(DbException ex)
        {
            await _loggerManager.LogError(ex, $"Internal Server Error - Database");
            return GenericResponse<UserDto>.Failure(null, HttpStatusCode.InternalServerError, $"An Error Occurred - Database", new { ex.Message, Description = ex?.InnerException?.Message });
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, $"Internal Server Error");
            return GenericResponse<UserDto>.Failure(null, HttpStatusCode.InternalServerError, $"An Error Occurred - Database", new { ex.Message, Description = ex?.InnerException?.Message });
        }
    }

    private string SerializeObject(object obj)
                        => JsonSerializer.Serialize(obj);
}

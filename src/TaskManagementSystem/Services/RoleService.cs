using Contracts;
using Contracts.Infrastructure;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Service.Contract;
using Shared.ApiResponse;
using Shared.DataTransferObjects.Role;
using Shared.Mapper;
using System.Data.Common;
using System.Net;
using System.Text.Json;

namespace Services;

public sealed class RoleService : IRoleService
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly ILoggerManager _loggerManager;
    public RoleService(IRepositoryManager repositoryManager, ILoggerManager loggerManager)
    {
        _repositoryManager = repositoryManager;
        _loggerManager = loggerManager;
    }

    public async Task<GenericResponse<RoleDto>> CreateAsync(CreateRoleDto createRole)
    {
        try
        {
            await _loggerManager.LogInfo($"Creating Role - {SerializeObjects(createRole)}");

            bool isNameExist = await _repositoryManager.RoleRepository.GetAllRoles(false, false)
                                                    .AnyAsync(x => x.NormalizedName == createRole.Name.ToUpper());

            if(isNameExist)
            {
                await _loggerManager.LogWarning($"");
                return GenericResponse<RoleDto>.Failure(null, HttpStatusCode.Conflict, $"Role with name already exists.", null);
            }

            Role roleToInsert = createRole.ToEntity();

            await _repositoryManager.RoleRepository.CreateRole(roleToInsert);

            await _repositoryManager.SaveChangesAsync();

            await _loggerManager.LogInfo($"Role Creation Successful. Role - {SerializeObjects(roleToInsert)}");

            return GenericResponse<RoleDto>.Success(roleToInsert.ToDto(), HttpStatusCode.Created, $"Role Creation Successful.");

        }
        catch(DbUpdateException ex)
        {
            await _loggerManager.LogError(ex, "An Error Occurred.");
            return GenericResponse<RoleDto>.Failure(null, HttpStatusCode.InternalServerError, "Internal Server Error", new { ex.Message, Description = ex?.InnerException?.Message });
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, "An Error Occurred.");
            return GenericResponse<RoleDto>.Failure(null, HttpStatusCode.InternalServerError, "Internal Server Error", new { ex.Message, Description = ex?.InnerException?.Message });
        }
    }

    public async Task<GenericResponse<string>> DeleteAsync(int Id, bool isSofDelete = true)
    {
        try
        {
            await _loggerManager.LogInfo($"Deleting Role with Id: {Id}");

            Role? roleToDelete = await _repositoryManager.RoleRepository.GetById(Id, true, true)
                                            .SingleOrDefaultAsync();

            if (roleToDelete == null)
            {
                await _loggerManager.LogWarning($"Role with provided Id: {Id} does not exist");
                return GenericResponse<string>.Failure(string.Empty, HttpStatusCode.NotFound, $"Role with Id: {Id} does not exist", null);
            }

            bool userRoleExists = await _repositoryManager.UserRoleRepository.GetByRoleId(Id, false).AnyAsync();

            if(userRoleExists)
            {
                await _loggerManager.LogWarning($"Role: {Id} has one or more users assigned to it.");
                return GenericResponse<string>.Failure(string.Empty, HttpStatusCode.NotFound, $"Role with Id: {Id} has one or more users assigned.", null);
            }

            if(isSofDelete)
            {
                await _loggerManager.LogInfo($"Marking Role as inactive. Id - {Id}");
                _repositoryManager.RoleRepository.UpdateRole(roleToDelete);
            }
            else
            {
                await _loggerManager.LogInfo($"Removing Role with Id: {Id} from database");
                _repositoryManager.RoleRepository.DeleteRole(roleToDelete);
            }

            await _repositoryManager.SaveChangesAsync();
            await _loggerManager.LogInfo(isSofDelete ? $"Role with Id: {Id} marked as inactive successfully." : $"Role with Id: {Id} deleted successfully.");

            return GenericResponse<string>.Success("Operation successful.", HttpStatusCode.OK, $"Role deleted successfully.");

        }
        catch(DbUpdateException ex)
        {
            await _loggerManager.LogError(ex, ex.Message);
            return GenericResponse<string>.Failure($"Operation Unsuccessful.", HttpStatusCode.InternalServerError, "Internal Server Error.", new { ex.Message, Description = ex?.InnerException?.Message });
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, ex.Message);
            return GenericResponse<string>.Failure($"Operation Unsuccessful.", HttpStatusCode.InternalServerError, "Internal Server Error.", new { ex.Message, Description = ex?.InnerException?.Message });
        }
    }

    public async Task<GenericResponse<IEnumerable<RoleDto>>> GetAllAsync(bool trackChnages, bool hasQueryFilter)
    {
        try
        {
            await _loggerManager.LogInfo($"Fetching All Existing Roles.....");

            IEnumerable<RoleDto> allExistingRoles = await _repositoryManager.RoleRepository.GetAllRoles(trackChnages, hasQueryFilter)
                                                //.Select(x => x.ToDto())
                                                .Select(RoleMapper.ToDtoExpression())
                                                .ToListAsync();

            await _loggerManager.LogInfo($"Roles Fetched Successfully - {SerializeObjects(allExistingRoles)}");

            return GenericResponse<IEnumerable<RoleDto>>.Success(allExistingRoles, HttpStatusCode.OK, "Roles Fetched Successfully");
        }
        catch(DbException ex)
        {
            await _loggerManager.LogError(ex, ex.Message);
            return GenericResponse<IEnumerable<RoleDto>>.Failure(null, HttpStatusCode.InternalServerError, $"An Error Occurred.", new { ex.Message, Description = ex?.InnerException?.Message });
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, ex.Message);
            return GenericResponse<IEnumerable<RoleDto>>.Failure(null, HttpStatusCode.InternalServerError, $"An Error Occurred.", new { ex.Message, Description = ex?.InnerException?.Message });
        }
    }

    public async Task<GenericResponse<RoleDto>> GetByIdAsync(int Id, bool trackChanges = false, bool hasQueryFilter = false)
    {
        try
        {
            await _loggerManager.LogInfo($"Fetching Role with Id: {Id}");

            RoleDto? existingRole = await _repositoryManager.RoleRepository.GetById(Id, trackChanges, hasQueryFilter)
                                            //.Select(x => x.ToDto())
                                            .Select(RoleMapper.ToDtoExpression())
                                            .SingleOrDefaultAsync();

            if(existingRole == null)
            {
                await _loggerManager.LogWarning($"No Role with specified Id: {Id}");
                return GenericResponse<RoleDto>.Failure(null, HttpStatusCode.NotFound, $"No Role with Provided Id", null);
            }

            await _loggerManager.LogInfo($"Role with id: {Id} fetched successfully. Role - {SerializeObjects(existingRole)}");

            return GenericResponse<RoleDto>.Success(existingRole, HttpStatusCode.OK, $"Role Fetched Successfully.");

        }
        catch(DbException ex)
        {
            await _loggerManager.LogError(ex, ex.Message);
            return GenericResponse<RoleDto>.Failure(null, HttpStatusCode.InternalServerError, $"Internal Server Error Occurred.", new { ex.Message, Description = ex?.InnerException?.Message });
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, ex.Message);
            return GenericResponse<RoleDto>.Failure(null, HttpStatusCode.InternalServerError, $"Internal Server Error Occurred.", new { ex.Message, Description = ex?.InnerException?.Message });
        }
    }

    private string SerializeObjects(object obj)
    {
        return JsonSerializer.Serialize(obj);
    }
}

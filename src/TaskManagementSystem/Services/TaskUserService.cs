using Contracts;
using Contracts.Infrastructure;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Service.Contract;
using Shared.ApiResponse;
using Shared.DataTransferObjects.TaskUser;
using System.Data.Common;
using System.Text.Json;
using System.Net;
using Shared.Mapper;

namespace Services;

public sealed class TaskUserService : ITaskUserService
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly ILoggerManager _loggerManager;

    public TaskUserService(IRepositoryManager repositoryManager, ILoggerManager loggerManager)
    {
        _repositoryManager = repositoryManager;
        _loggerManager = loggerManager;
    }
    public async Task<GenericResponse<TaskUserDto>> AssignUserToTask(CreateTaskUserDto taskUserDto)
    {
        try
        {
            await _loggerManager.LogInfo($"Creating Task User: {SerializeObject(taskUserDto)}");

            CreatedTask? existingTask = await _repositoryManager.CreatedTaskRepository.GetById(taskUserDto.PrimaryTaskId, false, true)
                                            .SingleOrDefaultAsync();
            if(existingTask is null)
            {
                await _loggerManager.LogWarning($"Invalid Task Id: {taskUserDto.PrimaryTaskId}");
                return GenericResponse<TaskUserDto>.Failure(null, HttpStatusCode.NotFound, $"Invalid Task Id.", null);
            }

            if(existingTask.TaskId != taskUserDto.TaskId)
            {
                await _loggerManager.LogWarning($"Task Id mismatch existing Task Id.");
                return GenericResponse<TaskUserDto>.Failure(null, HttpStatusCode.NotFound, $"Invalid Task Credentials.",null);
            }

            if(existingTask.ProjectedCompletionDate.Date < taskUserDto.ProposedCompletionDate.Date)
            {
                await _loggerManager.LogWarning($"Task Proposed Completion Date: {existingTask.ProjectedCompletionDate.Date} is earlier than Assigned Task Proposed Completion Date: {taskUserDto.ProposedCompletionDate.Date}");
                return GenericResponse<TaskUserDto>.Failure(null, HttpStatusCode.BadRequest, $"Proposed Completion Date cannot be greater than Task Proposed Completion Date.", null);
            }

            if(existingTask.TaskStage == Entities.StaticValues.Stage.Cancelled || existingTask.TaskStage == Entities.StaticValues.Stage.Completed)
            {
                return GenericResponse<TaskUserDto>.Failure(null, HttpStatusCode.NotFound, $"Task Not available to be assigned. Current Status: {existingTask.TaskStage.ToString()}.", null);
            }
            
            TaskUser taskUserToInsert = taskUserDto.ToEntity();

            await _repositoryManager.TaskUserRepository.CreateTaskUser(taskUserToInsert);

            await _repositoryManager.SaveChangesAsync();

            await _loggerManager.LogInfo($"Task User Assign Operation successful - {SerializeObject(taskUserToInsert.ToDto())}");

            return GenericResponse<TaskUserDto>.Success(taskUserToInsert.ToDto(), HttpStatusCode.Created, $"Task User Assign Operation Successful.");

        }
        catch(DbException ex)
        {
            await _loggerManager.LogError(ex, "Internal Server Error - Database");
            return GenericResponse<TaskUserDto>.Failure(null, HttpStatusCode.InternalServerError, $"Internal Server Error - Database", new { ex.Message, Description = ex?.InnerException?.Message });
        }
        catch (Exception ex)
        {
            
            await _loggerManager.LogError(ex, "Internal Server Error");
            return GenericResponse<TaskUserDto>.Failure(null, HttpStatusCode.InternalServerError, $"Internal Server Error", new { ex.Message, Description = ex?.InnerException?.Message });
        }
    }

    public async Task<GenericResponse<IEnumerable<TaskUserDto>>> GetAssignedTasksByTaskId(int TaskId)
    {
        try
        {
            await _loggerManager.LogInfo($"Assigned tasks by taskId: {TaskId}");

            IEnumerable<TaskUserDto> assignedTasksToUsers = await _repositoryManager.TaskUserRepository.GetByTaskId(TaskId, false, true)
                                                                            .Include(X => X.user).Include(x => x.task)
                                                                            .Select(x => x.ToDto())
                                                                            .ToListAsync();

            await _loggerManager.LogInfo($"Assigned User Tasks Fetched - {SerializeObject(assignedTasksToUsers)}");

            return GenericResponse<IEnumerable<TaskUserDto>>.Success(assignedTasksToUsers, HttpStatusCode.OK, "Task Users Fetched Successfully.");
        }
        catch (DbException ex)
        {
            await _loggerManager.LogError(ex, "Internal Server Error - Database");
            return GenericResponse<IEnumerable<TaskUserDto>>.Failure(null, HttpStatusCode.InternalServerError, $"Internal Server Error - Database", new { ex.Message, Description = ex?.InnerException?.Message });
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, "Internal Server Error");
            return GenericResponse<IEnumerable<TaskUserDto>>.Failure(null, HttpStatusCode.InternalServerError, $"Internal Server Error", new { ex.Message, Description = ex?.InnerException?.Message });
        }
    }

    public async Task<GenericResponse<IEnumerable<TaskUserDto>>> GetAssignedTasksToUser(int UserId)
    {
        try
        {
            await _loggerManager.LogInfo($"Get Assigned Tasks to user: {UserId}");

            IEnumerable<TaskUserDto> assignedTasksToUser = await _repositoryManager.TaskUserRepository.GetByUserId(UserId, false, true)
                                                                                    .Include(x => x.user).Include(x => x.task)
                                                                                    .Select(x => x.ToDto())
                                                                                    .ToListAsync();
            await _loggerManager.LogInfo($"User Tasks fetched successfully - {SerializeObject(assignedTasksToUser)}");

            return GenericResponse<IEnumerable<TaskUserDto>>.Success(assignedTasksToUser, HttpStatusCode.OK, "User Tasks Fetched Successfully.");
        }
        catch (DbException ex)
        {
            await _loggerManager.LogError(ex, "Internal Server Error - Database");
            return GenericResponse<IEnumerable<TaskUserDto>>.Failure(null, HttpStatusCode.InternalServerError, $"Internal Server Error - Database", new { ex.Message, Description = ex?.InnerException?.Message });
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, "Internal Server Error");
            return GenericResponse<IEnumerable<TaskUserDto>>.Failure(null, HttpStatusCode.InternalServerError, $"Internal Server Error", new { ex.Message, Description = ex?.InnerException?.Message });
        }
    }

    public async Task<GenericResponse<string>> RemoveAssignedTask(int Id, bool isSoftDelete = true)
    {
        try
        {
            await _loggerManager.LogInfo($"Remove assigned task {Id}");

            TaskUser? existingTaskUser = await _repositoryManager.TaskUserRepository.GetTaskUserById(Id, true, true).SingleOrDefaultAsync();

            if(existingTaskUser is null)
            {
                await _loggerManager.LogWarning($"No User Task with Id: {Id}");
                return GenericResponse<string>.Failure("Operation unsuccessful.", HttpStatusCode.NotFound, $"No Task User with Id: {Id}", null);
            }

            if(existingTaskUser.CompletionDate.HasValue)
            {
                await _loggerManager.LogWarning($"Cannot perform operation becasue its alrady completed.");
                return GenericResponse<string>.Failure("Operation unsuccessful.", HttpStatusCode.Conflict, $"Invalid Task Selected. Task already completed.", null);
            }

            if(isSoftDelete)
            {
                await _loggerManager.LogInfo($"Task User Deactivation Operation - {Id}");
                existingTaskUser.IsDeleted = true;
                _repositoryManager.TaskUserRepository.UpdateTaskUser(existingTaskUser);
            }
            else
            {
                await _loggerManager.LogInfo($"Task User Deletion Operation: {Id}");
                _repositoryManager.TaskUserRepository.DeleteTaskUser(existingTaskUser);
            }

            await _repositoryManager.SaveChangesAsync();

            await _loggerManager.LogInfo(isSoftDelete ? $"Task User Deactivation Successful - {Id}" : $"Task User Deletion Successful - {Id}");

            return GenericResponse<string>.Success("Operation successful", HttpStatusCode.OK, isSoftDelete ? $"Task User Deactivation Successful - {Id}" : $"Task User Deletion Successful - {Id}");

        }
        catch (DbException ex)
        {
            await _loggerManager.LogError(ex, "Internal Server Error - Database");
            return GenericResponse<string>.Failure(null, HttpStatusCode.InternalServerError, $"Internal Server Error - Database", new { ex.Message, Description = ex?.InnerException?.Message });
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, "Internal Server Error");
            return GenericResponse<string>.Failure(null, HttpStatusCode.InternalServerError, $"Internal Server Error", new { ex.Message, Description = ex?.InnerException?.Message });
        }
    }

    private string SerializeObject(object obj) => JsonSerializer.Serialize(obj);
}

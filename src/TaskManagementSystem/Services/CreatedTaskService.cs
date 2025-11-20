using Contracts;
using Contracts.Infrastructure;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Service.Contract;
using Shared.ApiResponse;
using Shared.DataTransferObjects.CreatedTask;
using Shared.Mapper;
using System.Data.Common;
using System.Text.Json;
using System.Net;
using Entities.StaticValues;

namespace Services;

public sealed class CreatedTaskService : ICreatedTaskService
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly ILoggerManager _loggerManager;

    public CreatedTaskService(IRepositoryManager repositoryManager, ILoggerManager loggerManager)
    {
        _repositoryManager = repositoryManager;
        _loggerManager = loggerManager;
    }
    public async Task<GenericResponse<CreatedTaskDto>> CreateAsync(CreateTaskDto createTask)
    {
        try
        {
            await _loggerManager.LogInfo($"Creating Task - {SerializeObjects(createTask)}");

            User? creatingUser = await _repositoryManager.UserRepository.GetById(createTask.UserId, false, true).SingleOrDefaultAsync();

            if( creatingUser == null)
            {
                await _loggerManager.LogWarning($"No User exists with specified Id: {createTask.UserId}");
                return GenericResponse<CreatedTaskDto>.Failure(null, HttpStatusCode.BadRequest, $"Invalid User Id", null);
            }

            CreatedTask taskToInsert = createTask.ToEntity();
            taskToInsert.TaskStage = Stage.Review;
            taskToInsert.CreatedBy = $"{creatingUser.FirstName} {creatingUser.LastName}";

            await _repositoryManager.CreatedTaskRepository.CreateTask(taskToInsert);

            await _repositoryManager.SaveChangesAsync();

            return GenericResponse<CreatedTaskDto>.Success(taskToInsert.ToDto(), HttpStatusCode.Created, "Task Creation Successful.");
        }
        catch(DbException ex)
        {
            await _loggerManager.LogError(ex, "Internal Server Error Occurred - Database");
            return GenericResponse<CreatedTaskDto>.Failure(null, HttpStatusCode.InternalServerError, $"Internal Database Server Error.", new { ex.Message, Description = ex?.InnerException?.Message });
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, "Internal Server Error Occurred");
            return GenericResponse<CreatedTaskDto>.Failure(null, HttpStatusCode.InternalServerError, $"Internal Server Error.", new { ex.Message, Description = ex?.InnerException?.Message });
        }
    }

    public async Task<GenericResponse<CreatedTaskDto>> GetByIdAsync(string taskId, bool trackChanges = false, bool hasQueryFilter = true)
    {
        try
        {
            await _loggerManager.LogInfo($"Get Tasks by User Id - {taskId}");

            CreatedTaskDto? task = await _repositoryManager.CreatedTaskRepository.GetByTaskId(taskId, trackChanges, hasQueryFilter)
                                                                    .Select(x => x.ToDto())
                                                                    .SingleOrDefaultAsync();

            await _loggerManager.LogInfo($"Taks fetched By status: {taskId} - {SerializeObjects(task)}");

            return GenericResponse<CreatedTaskDto>.Success(task, HttpStatusCode.OK, "Tasks fetched successfully");
        }
        catch (DbException ex)
        {
            await _loggerManager.LogError(ex, "Internal Server Error Occurred - Database");
            return GenericResponse<CreatedTaskDto>.Failure(null, HttpStatusCode.InternalServerError, $"Internal Database Server Error.", new { ex.Message, Description = ex?.InnerException?.Message });
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, "Internal Server Error Occurred");
            return GenericResponse<CreatedTaskDto>.Failure(null, HttpStatusCode.InternalServerError, $"Internal Server Error.", new { ex.Message, Description = ex?.InnerException?.Message });
        }
    }

    public async Task<GenericResponse<IEnumerable<CreatedTaskDto>>> GetByPriorityAsync(string taskPriority, bool trackChanges = false, bool hasQueryFilter = true)
    {
        try
        {
            await _loggerManager.LogInfo($"Get Tasks by  Priority - {taskPriority}");

            if (!Enum.TryParse<PriorityLevel>(taskPriority, ignoreCase: true, out var priority))
            {
                await _loggerManager.LogWarning($"Invalid Stage provided: {taskPriority}");
                return GenericResponse<IEnumerable<CreatedTaskDto>>.Failure(null, HttpStatusCode.BadRequest, $"Invalid Priority: {taskPriority}", null);
            }

            IEnumerable<CreatedTaskDto> userTasks = await _repositoryManager.CreatedTaskRepository.GetByPriority(priority, trackChanges, hasQueryFilter)
                                                                    .Select(x => x.ToDto())
                                                                    .ToListAsync();
            
            await _loggerManager.LogInfo($"Taks fetched By status: {taskPriority} - {SerializeObjects(userTasks)}");

            return GenericResponse<IEnumerable<CreatedTaskDto>>.Success(userTasks, HttpStatusCode.OK, "Tasks fetched successfully");
        }
        catch (DbException ex)
        {
            await _loggerManager.LogError(ex, "Internal Server Error Occurred - Database");
            return GenericResponse<IEnumerable<CreatedTaskDto>>.Failure(null, HttpStatusCode.InternalServerError, $"Internal Database Server Error.", new { ex.Message, Description = ex?.InnerException?.Message });
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, "Internal Server Error Occurred");
            return GenericResponse<IEnumerable<CreatedTaskDto>>.Failure(null, HttpStatusCode.InternalServerError, $"Internal Server Error.", new { ex.Message, Description = ex?.InnerException?.Message });
        }
    }

    public async Task<GenericResponse<IEnumerable<CreatedTaskDto>>> GetByStatusAsync(string taskStatus, bool trackChanges = false, bool hasQueryFilter = true)
    {
        try
        {
            await _loggerManager.LogInfo($"Get Tasks by User Id - {taskStatus}");

            if(!Enum.TryParse<Stage>(taskStatus, ignoreCase:true, out var stage))
            {
                await _loggerManager.LogWarning($"Invalid Stage provided: {taskStatus}");
                return GenericResponse<IEnumerable<CreatedTaskDto>>.Failure(null, HttpStatusCode.BadRequest, $"Invalid Status: {taskStatus}", null);
            }

            IEnumerable<CreatedTaskDto> userTasks = await _repositoryManager.CreatedTaskRepository.GetByStatus(stage, trackChanges, hasQueryFilter)
                                                                    .Select(x => x.ToDto())
                                                                    .ToListAsync();
            await _loggerManager.LogInfo($"Taks fetched By status: {taskStatus} - {SerializeObjects(userTasks)}");

            return GenericResponse<IEnumerable<CreatedTaskDto>>.Success(userTasks, HttpStatusCode.OK, "User tasks fetched successfully");
        }
        catch (DbException ex)
        {
            await _loggerManager.LogError(ex, "Internal Server Error Occurred - Database");
            return GenericResponse<IEnumerable<CreatedTaskDto>>.Failure(null, HttpStatusCode.InternalServerError, $"Internal Database Server Error.", new { ex.Message, Description = ex?.InnerException?.Message });
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, "Internal Server Error Occurred");
            return GenericResponse<IEnumerable<CreatedTaskDto>>.Failure(null, HttpStatusCode.InternalServerError, $"Internal Server Error.", new { ex.Message, Description = ex?.InnerException?.Message });
        }
    }

    public async Task<GenericResponse<IEnumerable<CreatedTaskDto>>> GetByUserAsync(int UserId, bool trackChanges = false, bool hasQueryFilter = true)
    {
        try
        {
            await _loggerManager.LogInfo($"Get Tasks by User Id - {UserId}");

            IEnumerable<CreatedTaskDto> userTasks = await _repositoryManager.CreatedTaskRepository.GetAllTasks(trackChanges, hasQueryFilter)
                                                                    .Where(x => x.UserId == UserId)
                                                                    .Select(x => x.ToDto())
                                                                    .ToListAsync();
            return GenericResponse<IEnumerable<CreatedTaskDto>>.Success(userTasks, HttpStatusCode.OK, "User tasks fetched successfully");
        }
        catch (DbException ex)
        {
            await _loggerManager.LogError(ex, "Internal Server Error Occurred - Database");
            return GenericResponse<IEnumerable<CreatedTaskDto>>.Failure(null, HttpStatusCode.InternalServerError, $"Internal Database Server Error.", new { ex.Message, Description = ex?.InnerException?.Message });
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, "Internal Server Error Occurred");
            return GenericResponse<IEnumerable<CreatedTaskDto>>.Failure(null, HttpStatusCode.InternalServerError, $"Internal Server Error.", new { ex.Message, Description = ex?.InnerException?.Message });
        }
    }

    private string SerializeObjects(object obj)
        => JsonSerializer.Serialize(obj);
}

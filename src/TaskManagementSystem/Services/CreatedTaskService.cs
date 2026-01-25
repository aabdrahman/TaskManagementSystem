using Contracts;
using Contracts.Infrastructure;
using Entities.Models;
using Entities.StaticValues;
using Infrastructure.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Service.Contract;
using Shared.ApiResponse;
using Shared.DataTransferObjects.CreatedTask;
using Shared.Mapper;
using System.Data.Common;
using System.Net;
using System.Text.Json;

namespace Services;

public sealed class CreatedTaskService : ICreatedTaskService
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly ILoggerManager _loggerManager;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IAuditPersistenceService _auditPersistenceService;

    public CreatedTaskService(IRepositoryManager repositoryManager, ILoggerManager loggerManager, IHttpContextAccessor contextAccessor, IAuditPersistenceService auditPersistenceService)
    {
        _repositoryManager = repositoryManager;
        _loggerManager = loggerManager;
        _contextAccessor = contextAccessor;
        _auditPersistenceService = auditPersistenceService;
    }

    public async Task<GenericResponse<string>> CancelTaskAsync(CancelCreatedTaskDto cancelCreatedTaskDto)
    {
        try
        {
            await _loggerManager.LogInfo($"Cancel Task Request - {SerializeObjects(cancelCreatedTaskDto)}");

            CreatedTask? taskToCancel = await _repositoryManager.CreatedTaskRepository.GetById(cancelCreatedTaskDto.TaskPrimaryId, true).Include(x => x.UserLink).SingleOrDefaultAsync();

            if(taskToCancel is null)
            {
                await _loggerManager.LogWarning($"No task with Id - {cancelCreatedTaskDto.TaskPrimaryId}");
                return GenericResponse<string>.Failure("Operation Failed", HttpStatusCode.NotFound, "Invalid Task Id provided.", null);
            }

            if(taskToCancel.CompletionDate.HasValue || taskToCancel.TaskStage == Stage.Cancelled)
            {
                await _loggerManager.LogInfo($"Invalid Task Selected To Cancel - Status: {taskToCancel.TaskStage.ToString()}");
                return GenericResponse<string>.Failure("Operation Failed.", HttpStatusCode.Conflict, $"Task Status: {taskToCancel.TaskStage.ToString()}", null);
            }

            bool anyPendingRequest = taskToCancel.UserLink.Any(x => !x.CompletionDate.HasValue && string.IsNullOrEmpty(x.CancelReason));

            if (anyPendingRequest)
            {
                await _loggerManager.LogWarning($"One or more user task are still pending completion or cancellation.");
                return GenericResponse<string>.Failure("Operation Failed.", HttpStatusCode.Conflict, $"One or more user tasks are still pending.", null);
            }

            if (taskToCancel.UserId != cancelCreatedTaskDto.ReqesterId || taskToCancel.TaskId != cancelCreatedTaskDto.TaskId)
            {
                await _loggerManager.LogWarning($"Request Mismatch with the existing id: {SerializeObjects(taskToCancel)}");
                return GenericResponse<string>.Failure("Operation Failed.", HttpStatusCode.BadRequest, "Request Mismatch.", null);
            }

            string? oldValue = taskToCancel.CancelReason;

            taskToCancel.TaskStage = Stage.Cancelled;
            taskToCancel.CancelReason = cancelCreatedTaskDto.Justification;

            _repositoryManager.CreatedTaskRepository.UpdateTask(taskToCancel);

            await _repositoryManager.SaveChangesAsync();

            AuditTrail auditTrail = new AuditTrail()
            {
                EntityId = taskToCancel.Id.ToString(),
                EntityName = typeof(CreatedTask).Name,
                PerformedAction = AuditAction.Cancelled,
                PerformedAt = DateTime.UtcNow.ToLocalTime(),
                ParticipantName = _contextAccessor.HttpContext.User.FindFirst(x => x.Type.EndsWith("claims/name"))?.Value ?? "",
                ParticipandIdentification = _contextAccessor.HttpContext.User.FindFirst(x => x.Type.EndsWith("claims/serialnumber"))?.Value ?? "0",
                OldValue = string.IsNullOrEmpty(oldValue) ? "" : oldValue,
                NewValue = taskToCancel.CancelReason
            };

            bool queueAuditResponse = await _auditPersistenceService.QueueAuditRecord(auditTrail);

            await _loggerManager.LogInfo($"Cancel Created Task Success - {SerializeObjects(cancelCreatedTaskDto)} by {_contextAccessor.HttpContext.User.FindFirst(x => x.Type.EndsWith("claims/name"))?.Value}. Queue Audit Response: {queueAuditResponse}");

            return GenericResponse<string>.Success("Operation Successul", HttpStatusCode.OK, "Task Cancelled Successfuly.");
        }
        catch (DbException ex)
        {
            await _loggerManager.LogError(ex, "Internal Server Error Occurred - Database");
            return GenericResponse<string>.Failure(null, HttpStatusCode.InternalServerError, $"Internal Database Server Error.", new { ex.Message, Description = ex?.InnerException?.Message });
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, "Internal Server Error Occurred");
            return GenericResponse<string>.Failure(null, HttpStatusCode.InternalServerError, $"Internal Server Error.", new { ex.Message, Description = ex?.InnerException?.Message });
        }
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

            AuditTrail auditTrail = new AuditTrail()
            {
                EntityId = taskToInsert.Id.ToString(),
                EntityName = typeof(CreatedTask).Name,
                PerformedAction = AuditAction.Created,
                PerformedAt = DateTime.UtcNow.ToLocalTime(),
                ParticipantName = _contextAccessor.HttpContext.User.FindFirst(x => x.Type.EndsWith("claims/name"))?.Value ?? "",
                ParticipandIdentification = _contextAccessor.HttpContext.User.FindFirst(x => x.Type.EndsWith("claims/serialnumber"))?.Value ?? "0",
                NewValue = SerializeObjects(taskToInsert.ToDto())
            };

            bool queueAuditResponse = await _auditPersistenceService.QueueAuditRecord(auditTrail);

            await _loggerManager.LogInfo($"Task Created Successfully - {SerializeObjects(taskToInsert.ToDto())}. Queue Audit Response: {queueAuditResponse}");

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

    public async Task<GenericResponse<string>> DeleteAsync(int taskId, bool isSoftDelete = true)
    {
        try
        {
            await _loggerManager.LogInfo($"Deleting User with Id: {taskId}");

            CreatedTask? taskToDelete = await _repositoryManager.CreatedTaskRepository.GetById(taskId, true, true).SingleOrDefaultAsync();

            if(taskToDelete is null)
            {
                await _loggerManager.LogWarning($"No task exists with specified Id: {taskId}");
                return GenericResponse<string>.Failure("Operation Failed", HttpStatusCode.NotFound, $"No Task with Id: {taskId}", null);
            }

            if(isSoftDelete)
            {
                await _loggerManager.LogInfo($"Deactivation operation of Task: {taskId}");
                taskToDelete.IsDeleted = true;

                _repositoryManager.CreatedTaskRepository.UpdateTask(taskToDelete);
            }
            else
            {
                await _loggerManager.LogInfo($"Deletion operation of task: {taskId}");
                _repositoryManager.CreatedTaskRepository.DeleteTask(taskToDelete);
            }

            await _repositoryManager.SaveChangesAsync();

            AuditTrail auditTrail = new AuditTrail()
            {
                EntityId = taskToDelete.Id.ToString(),
                EntityName = typeof(CreatedTask).Name,
                PerformedAction = isSoftDelete ? AuditAction.SoftDeleted : AuditAction.HardDeleted,
                PerformedAt = DateTime.UtcNow.ToLocalTime(),
                ParticipantName = _contextAccessor.HttpContext.User.FindFirst(x => x.Type.EndsWith("claims/name"))?.Value ?? "",
                ParticipandIdentification = _contextAccessor.HttpContext.User.FindFirst(x => x.Type.EndsWith("claims/serialnumber"))?.Value ?? "0",
                OldValue = SerializeObjects(taskToDelete.ToDto())
            };

            bool queueAuditResponse = await _auditPersistenceService.QueueAuditRecord(auditTrail);

            await _loggerManager.LogInfo(isSoftDelete ? $"Task with Id: {taskId} deactivation successful." : $"Task with Id: {taskId} deleted successfully." + $"Queue Audit Record: {queueAuditResponse}");

            return GenericResponse<string>.Success("Operation successful.", HttpStatusCode.OK, isSoftDelete ? "Task deactivation successful" : "Task Deletion Successful");
        }
        catch (DbException ex)
        {
            await _loggerManager.LogError(ex, "Internal Server Error Occurred - Database");
            return GenericResponse<string>.Failure(null, HttpStatusCode.InternalServerError, $"Internal Database Server Error.", new { ex.Message, Description = ex?.InnerException?.Message });
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, "Internal Server Error Occurred");
            return GenericResponse<string>.Failure(null, HttpStatusCode.InternalServerError, $"Internal Server Error.", new { ex.Message, Description = ex?.InnerException?.Message });
        }
    }

    public async Task<GenericResponse<CreatedTaskDto>> GetByIdAsync(string taskId, bool trackChanges = false, bool hasQueryFilter = true)
    {
        try
        {
            await _loggerManager.LogInfo($"Get Tasks by User Id - {taskId}");

            CreatedTaskDto? task = await _repositoryManager.CreatedTaskRepository.GetByTaskId(taskId, trackChanges, hasQueryFilter)
                                                                    //.Select(x => x.ToDto())
                                                                    .Select(CreatedTaskMapper.ToDtoExpression())
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
                                                                    //.Select(x => x.ToDto())
                                                                    .Select(CreatedTaskMapper.ToDtoExpression())
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
                                                                    //.Select(x => x.ToDto())
                                                                    .Select(CreatedTaskMapper.ToDtoExpression())
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
                                                                    //.Select(x => x.ToDto())
                                                                    .Select(CreatedTaskMapper.ToDtoExpression())
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

    public async Task<GenericResponse<string>> ReassignTaskAsync(ReassignCreatedTaskDto reassignCreatedTaskDto)
    {
        try
        {
            string userIdFromToken = _contextAccessor.HttpContext.User.FindFirst(x => x.Type.EndsWith("serialnumber"))?.Value ?? "0";

            await _loggerManager.LogInfo($"Reassign Task - {SerializeObjects(reassignCreatedTaskDto)}. User: {_contextAccessor.HttpContext.User.FindFirst(x => x.Type.EndsWith("emailaddress"))?.Value ?? "anonymous"}");

            bool isValidUser = _contextAccessor.HttpContext.User.IsInRole("Analyst") || _contextAccessor.HttpContext.User.IsInRole("Product Manager");

            //if (!isValidUser)
            //{
            //    await _loggerManager.LogWarning($"User Role is not valid enough.");
            //    return GenericResponse<string>.Failure("Operation Failed", HttpStatusCode.Forbidden, "Unauthorized Access.", null);
            //}

            CreatedTask? taskToReassign = await _repositoryManager.CreatedTaskRepository.GetByTaskId(reassignCreatedTaskDto.TaskId, true).SingleOrDefaultAsync();

            if(taskToReassign is null)
            {
                await _loggerManager.LogWarning($"Invalid Task Id provided - {reassignCreatedTaskDto.TaskId}");
                return GenericResponse<string>.Failure("Operation Failed.", HttpStatusCode.NotFound, "Invalid Task Id.", null);
            }

            if(taskToReassign.CompletionDate.HasValue)
            {
                await _loggerManager.LogWarning($"Invalid Task Selected. Task already completed.");
                return GenericResponse<string>.Failure("Operation Failed.", HttpStatusCode.NotFound, $"Task already completed. Stage: {taskToReassign.TaskStage.ToString()}", null);
            }

            if(taskToReassign.UserId.ToString() != userIdFromToken)
            {
                if(!isValidUser)
                {
                    await _loggerManager.LogWarning($"Invalid User Conflict. Initiating User: {taskToReassign.UserId.ToString()}. Logged In User {userIdFromToken}");
                    return GenericResponse<string>.Failure("Operation Failed.", HttpStatusCode.BadRequest, "Failed Operation", null);
                }
            }

            //User? userToAssign = await _repositoryManager.UserRepository.GetById(reassignCreatedTaskDto.UserId, false).SingleOrDefaultAsync();

            //bool isUserValid = await _repositoryManager.UserRepository.GetAllUsers().AnyAsync(x => x.AssignedUnit.NormalizedName.Contains(StaticRoles.TaskCreateUnit1.ToUpper()) || x.AssignedUnit.NormalizedName.Contains(StaticRoles.TaskCreateUnit2.ToUpper()));

            //if(!isUserValid)
            //{
            //    await _loggerManager.LogWarning($"Invalid User provided. User is not in the desired role to assign task.");
            //    return GenericResponse<string>.Failure("Operation Failed", HttpStatusCode.BadRequest, "User Role is not valid.", null);
            //}

            var oldValue = taskToReassign.UserId.ToString();

            taskToReassign.UserId = reassignCreatedTaskDto.UserId;

            _repositoryManager.CreatedTaskRepository.UpdateTask(taskToReassign);

            await _repositoryManager.SaveChangesAsync();

            AuditTrail auditTrail = new AuditTrail()
            {
                EntityId = taskToReassign.Id.ToString(),
                EntityName = typeof(CreatedTask).Name,
                PerformedAction = AuditAction.Reassigned,
                PerformedAt = DateTime.UtcNow.ToLocalTime(),
                ParticipantName = _contextAccessor.HttpContext.User.FindFirst(x => x.Type.EndsWith("claims/name"))?.Value ?? "",
                ParticipandIdentification = _contextAccessor.HttpContext.User.FindFirst(x => x.Type.EndsWith("claims/serialnumber"))?.Value ?? "0",
                NewValue = taskToReassign.UserId.ToString(),
                OldValue = oldValue
            };

            bool queueAuditResponse = await _auditPersistenceService.QueueAuditRecord(auditTrail);

            await _loggerManager.LogInfo($"Task Reassign Successful. {SerializeObjects(reassignCreatedTaskDto)}. Queue Audit Record: {queueAuditResponse}");

            return GenericResponse<string>.Success("Operation Successful.", HttpStatusCode.OK, "Task Successfully Reassigned.");

        }
        catch (DbException ex)
        {
            await _loggerManager.LogError(ex, "Internal Server Error Occurred - Database");
            return GenericResponse<string>.Failure(null, HttpStatusCode.InternalServerError, $"Internal Database Server Error.", new { ex.Message, Description = ex?.InnerException?.Message });
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, "Internal Server Error Occurred");
            return GenericResponse<string>.Failure(null, HttpStatusCode.InternalServerError, $"Internal Server Error.", new { ex.Message, Description = ex?.InnerException?.Message });
        }
    }

    public async Task<GenericResponse<CreatedTaskDto>> UpdateCreatedTaskAsync(UpdateCreatedTaskDto updateCreatedTaskDto)
    {
        try
        {
            await _loggerManager.LogInfo($"Update Created Task - {SerializeObjects(updateCreatedTaskDto)}");

            CreatedTask? createdTaskToUpdate = await _repositoryManager.CreatedTaskRepository.GetById(updateCreatedTaskDto.Id, true).SingleOrDefaultAsync();

            if(createdTaskToUpdate is null)
            {
                await _loggerManager.LogInfo($"Created Task with Id: {updateCreatedTaskDto.Id} cannot be found");
                return GenericResponse<CreatedTaskDto>.Failure(null, HttpStatusCode.NotFound, "", null);
            }

            if(!createdTaskToUpdate.UserId.ToString().Equals(_contextAccessor.HttpContext.User.FindFirst(x => x.Type.EndsWith("claims/serialnumber"))?.Value ?? "0"))
            {
                await _loggerManager.LogWarning($"Logged in user id is not same as created task user id");
                return GenericResponse<CreatedTaskDto>.Failure(null, HttpStatusCode.Conflict, "You are not the creator of task", null);
            }

            if(createdTaskToUpdate.CompletionDate.HasValue)
            {
                await _loggerManager.LogWarning($"Task with Id: {updateCreatedTaskDto.Id} is already completed at: {createdTaskToUpdate.CompletionDate}");
                return GenericResponse<CreatedTaskDto>.Failure(null, HttpStatusCode.BadRequest, "Task already completed.", null);
            }

            if (createdTaskToUpdate.TaskStage == Stage.Cancelled)
            {
                await _loggerManager.LogWarning($"Task with Id: {updateCreatedTaskDto.Id} has an invalid stage: {createdTaskToUpdate.TaskStage.ToString()}");
                return GenericResponse<CreatedTaskDto>.Failure(null, HttpStatusCode.BadRequest, "Task is already cancelled.", null);
            }

            var oldValue = createdTaskToUpdate.ToDto();

            createdTaskToUpdate.Title = updateCreatedTaskDto.Title;
            createdTaskToUpdate.Description = updateCreatedTaskDto.Description;
            createdTaskToUpdate.ProjectedCompletionDate = updateCreatedTaskDto.ProposedCompletionDate;
            createdTaskToUpdate.Priority = Enum.Parse<PriorityLevel>(updateCreatedTaskDto.Priority, ignoreCase: true);

            _repositoryManager.CreatedTaskRepository.UpdateTask(createdTaskToUpdate);

            await _repositoryManager.SaveChangesAsync();

                AuditTrail auditTrail = new AuditTrail()
                {
                    EntityId = createdTaskToUpdate.Id.ToString(),
                    EntityName = typeof(CreatedTask).Name,
                    PerformedAction = AuditAction.Updated,
                    PerformedAt = DateTime.UtcNow.ToLocalTime(),
                    ParticipantName = _contextAccessor.HttpContext.User.FindFirst(x => x.Type.EndsWith("claims/name"))?.Value ?? "",
                    ParticipandIdentification = _contextAccessor.HttpContext.User.FindFirst(x => x.Type.EndsWith("claims/serialnumber"))?.Value ?? "0",
                    NewValue = SerializeObjects(createdTaskToUpdate.ToDto()),
                    OldValue = SerializeObjects(oldValue)
                };

                bool queueAuditResponse = await _auditPersistenceService.QueueAuditRecord(auditTrail);

            await _loggerManager.LogInfo($"Created Task Updated Successful - {SerializeObjects(updateCreatedTaskDto)}. Queue Audit Record: {queueAuditResponse}");

            return GenericResponse<CreatedTaskDto>.Success(createdTaskToUpdate.ToDto(), HttpStatusCode.OK, "Created Task Updated Successfully.");

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

    public async Task<GenericResponse<string>> UpdateCreatedTaskStatus(UpdateCreatedTaskStatusDto updateCreatedTaskCompleteStatusDto)
    {
        try
        {
            await _loggerManager.LogInfo($"Updating Created Task Status - {SerializeObjects(updateCreatedTaskCompleteStatusDto)}");

            CreatedTask? taskToUpdate = await _repositoryManager.CreatedTaskRepository.GetById(updateCreatedTaskCompleteStatusDto.Id).Include(x => x.UserLink).SingleOrDefaultAsync();

            if(taskToUpdate is null)
            {
                await _loggerManager.LogWarning($"Created Task with Id: {updateCreatedTaskCompleteStatusDto.Id} does not exist.");
                return GenericResponse<string>.Failure("Operation Failed.", HttpStatusCode.NotFound, "Created Task not found", null);
            }

            bool checkInValidStatus = Enum.Parse<Stage>(updateCreatedTaskCompleteStatusDto.Stage, ignoreCase: true) == Stage.Cancelled;

            if(checkInValidStatus)
            {
                await _loggerManager.LogWarning($"Created Task with Id: {updateCreatedTaskCompleteStatusDto.Id} cannot be updated.");
                return GenericResponse<string>.Failure("Operation Failed.", HttpStatusCode.NotFound, "Failed.", null);
            }

            bool anyPendingRequest = taskToUpdate.UserLink.Any(x => !x.CompletionDate.HasValue && string.IsNullOrEmpty(x.CancelReason));

            if(anyPendingRequest)
            {
                await _loggerManager.LogWarning($"One or more user task are still pending completion.");
                return GenericResponse<string>.Failure("Operation Failed.", HttpStatusCode.Conflict, $"One or more user tasks are yet to be completed.", null);
            }

            string loggedInUser = _contextAccessor.HttpContext.User.FindFirst(x => x.Type.EndsWith("claims/serialnumber"))?.Value ?? "0";

            if(!string.Equals(loggedInUser, taskToUpdate.UserId.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                await _loggerManager.LogWarning($"Logged in user: {loggedInUser} is not same as created task id: {taskToUpdate.UserId}");
                return GenericResponse<string>.Failure("Operation Failed.", HttpStatusCode.BadRequest, "Created Task not found", null);
            }

            var oldValue = taskToUpdate.TaskStage.ToString();

            taskToUpdate.TaskStage = Enum.Parse<Stage>(updateCreatedTaskCompleteStatusDto.Stage, ignoreCase: true);

            _repositoryManager.CreatedTaskRepository.UpdateTask(taskToUpdate);

            await _repositoryManager.SaveChangesAsync();

            AuditTrail auditTrail = new AuditTrail()
            {
                EntityId = taskToUpdate.Id.ToString(),
                EntityName = typeof(CreatedTask).Name,
                PerformedAction = AuditAction.Updated,
                PerformedAt = DateTime.UtcNow.ToLocalTime(),
                ParticipantName = _contextAccessor.HttpContext.User.FindFirst(x => x.Type.EndsWith("claims/name"))?.Value ?? "",
                ParticipandIdentification = _contextAccessor.HttpContext.User.FindFirst(x => x.Type.EndsWith("claims/serialnumber"))?.Value ?? "0",
                NewValue = taskToUpdate.TaskStage.ToString(),
                OldValue = oldValue
            };

            bool queueAuditResponse = await _auditPersistenceService.QueueAuditRecord(auditTrail);

            await _loggerManager.LogInfo($"Task Sttaus Update Successful - {SerializeObjects(updateCreatedTaskCompleteStatusDto)}. Queue Audit Record: {queueAuditResponse}");

            return GenericResponse<string>.Success("Operation Successful", HttpStatusCode.OK, $"Task updated Successfully. Current Status: {taskToUpdate.TaskStage.ToString()}");
        }
        catch (DbException ex)
        {
            await _loggerManager.LogError(ex, "Internal Server Error Occurred - Database");
            return GenericResponse<string>.Failure(null, HttpStatusCode.InternalServerError, $"Internal Database Server Error.", new { ex.Message, Description = ex?.InnerException?.Message });
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, "Internal Server Error Occurred");
            return GenericResponse<string>.Failure(null, HttpStatusCode.InternalServerError, $"Internal Server Error.", new { ex.Message, Description = ex?.InnerException?.Message });
        }
    }

    private string SerializeObjects(object obj)
        => JsonSerializer.Serialize(obj);
}

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
using Microsoft.AspNetCore.Http;
using Shared.DataTransferObjects.UserDashboard;

namespace Services;

public sealed class TaskUserService : ITaskUserService
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly ILoggerManager _loggerManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TaskUserService(IRepositoryManager repositoryManager, ILoggerManager loggerManager, IHttpContextAccessor httpContextAccessor)
    {
        _repositoryManager = repositoryManager;
        _loggerManager = loggerManager;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<GenericResponse<TaskUserDto>> AssignUserToTask(CreateTaskUserDto taskUserDto)
    {
        try
        {
            await _loggerManager.LogInfo($"Creating Task User: {SerializeObject(taskUserDto)}");

            string loggedInUserId = _httpContextAccessor.HttpContext.User.FindFirst(x => x.Type.EndsWith("serialnumber"))?.Value ?? "0";

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

            if(taskUserDto.AssignedUser.ToString().Equals(existingTask.UserId))
            {
                await _loggerManager.LogWarning($"Task created by: {existingTask.UserId} cannot be assigned a user task.");
                return GenericResponse<TaskUserDto>.Failure(null, HttpStatusCode.Conflict, $"User Task cannot be assigned to selected user.", null);
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

            var createdByEmail = _httpContextAccessor.HttpContext.User.FindFirst(x => x.Type.EndsWith("emailaddress"))?.Value ?? "anonymous";

            taskUserToInsert.CreatedBy = $"{createdByEmail}-{loggedInUserId}";

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

    public async Task<GenericResponse<string>> CancelTask(CancelUserTaskDto cancelUserTaskDto)
    {
        try
        {
            await _loggerManager.LogInfo($"Cancel User Task - {SerializeObject(cancelUserTaskDto)}");

            string loggedInUserId = _httpContextAccessor.HttpContext.User.FindFirst(x => x.Type.EndsWith("serialnumber"))?.Value ?? "0";

            TaskUser? taskUserToCancel = await _repositoryManager.TaskUserRepository.GetTaskUserById(cancelUserTaskDto.UserTaskId, true)
                                                                                .Include(x => x.task)
                                                                                .SingleOrDefaultAsync();
            if(taskUserToCancel is null)
            {
                await _loggerManager.LogInfo($"Task with User Id does not exist -  {cancelUserTaskDto.UserTaskId}");
                return GenericResponse<string>.Failure("Operation Failed.", HttpStatusCode.NotFound, $"No Task User with Id: {cancelUserTaskDto.UserTaskId}", null);
            }

            if(loggedInUserId != taskUserToCancel.task.UserId.ToString())
            {
                await _loggerManager.LogInfo($"Invalid Logged In User.");
                return GenericResponse<string>.Failure("Operation Failed", HttpStatusCode.BadRequest, "Unauthorized.", null);
            }

            if(taskUserToCancel.CompletionDate.HasValue)
            {
                await _loggerManager.LogWarning($"Invalid Task User Selected. Task already completed.");
                return GenericResponse<string>.Failure("Operation Failed", HttpStatusCode.Conflict, "Invalid Taskk User. Already Completed.", null);
            }

            taskUserToCancel.CancelReason = cancelUserTaskDto.CancelReason;

            _repositoryManager.TaskUserRepository.UpdateTaskUser(taskUserToCancel);

            await _repositoryManager.SaveChangesAsync();

            await _loggerManager.LogInfo($"User Task: {cancelUserTaskDto.UserTaskId} cancelled by {_httpContextAccessor.HttpContext.User.FindFirst(x => x.Type.EndsWith("claims/name"))?.Value} due to {cancelUserTaskDto.CancelReason}");

            return GenericResponse<string>.Success($"Operation Success.", HttpStatusCode.OK, "User Task Cancelled Successsfully.");
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

    public async Task<GenericResponse<IEnumerable<TaskUserDto>>> GetAssignedTasksByTaskId(int TaskId)
    {
        try
        {
            await _loggerManager.LogInfo($"Assigned tasks by taskId: {TaskId}");

            IEnumerable<TaskUserDto> assignedTasksToUsers = await _repositoryManager.TaskUserRepository.GetByTaskId(TaskId, false, true)
                                                                            .Include(X => X.user).Include(x => x.task)
                                                                            //.Select(x => x.ToDto())
                                                                            .Select(TaskUserMapper.ToDtoExpression())
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
                                                                                    //.Select(x => x.ToDto())
                                                                                    .Select(TaskUserMapper.ToDtoExpression())
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

	public async Task<GenericResponse<UserTaskDashboardDto>> GetUserDashboard(int UserId)
	{
        try
        {
            await _loggerManager.LogInfo($"Fetching User Dashboard for: {UserId}");

            //GET COUNT OF USER TASKS
            int TotalUserTasks = await _repositoryManager.TaskUserRepository.GetByUserId(UserId).CountAsync(x => !x.CompletionDate.HasValue && x.CancelReason == null);

            if(TotalUserTasks == 0)
            {
                await _loggerManager.LogWarning($"No Record Found for - {UserId}");
                return GenericResponse<UserTaskDashboardDto>.Failure(new UserTaskDashboardDto(), HttpStatusCode.NotFound, "No Pending Record Found."); //This returns a default instance for frontend use
            }

			UserTaskDashboardDto output = await _repositoryManager.TaskUserRepository.GetByUserId(UserId)
                                                                        .Where(x => !x.CompletionDate.HasValue && x.CancelReason == null)
                                                                        .GroupBy(_ => 1)
                                                                        .Select(x => new UserTaskDashboardDto()
                                                                        {
                                                                            DueToday = x.Count(x => x.ProposedCompletionDate == DateTime.UtcNow.Date),
                                                                            OverDueTasks = x.Count(x => x.ProposedCompletionDate < DateTime.UtcNow.Date),
                                                                            PendingTasks = x.Count(x => x.ProposedCompletionDate >= DateTime.UtcNow.Date.AddDays(1)),

                                                                            UserTasks = x.OrderBy(x => x.ProposedCompletionDate).Select(x => new UserTaskSummaryDto() { Id = x.Id, Title = x.Title, DueDate = x.ProposedCompletionDate }).Skip(0).Take(5).ToList()
                                                                        })
                                                                        .FirstAsync() ?? new UserTaskDashboardDto();

            //IEnumerable<TaskUserDto> output = await _repositoryManager.TaskUserRepository.GetByUserId(UserId)
            //                                            .Where(x => !x.CompletionDate.HasValue || string.IsNullOrEmpty(x.CancelReason))
            //                                            .Select(x => x.ToDto())
            //                                            .ToListAsync();

            await _loggerManager.LogInfo($"User Dashboard fetched for user: {UserId} - {SerializeObject(output)}");

            return GenericResponse<UserTaskDashboardDto>.Success(output, HttpStatusCode.OK, "Dashboard Fetched.");

        }
		catch (DbException ex)
		{
			await _loggerManager.LogError(ex, "Internal Server Error - Database");
			return GenericResponse<UserTaskDashboardDto>.Failure(null, HttpStatusCode.InternalServerError, $"Internal Server Error - Database", new { ex.Message, Description = ex?.InnerException?.Message });
		}
		catch (Exception ex)
		{
			await _loggerManager.LogError(ex, "Internal Server Error");
			return GenericResponse<UserTaskDashboardDto>.Failure(null, HttpStatusCode.InternalServerError, $"Internal Server Error", new { ex.Message, Description = ex?.InnerException?.Message });
		}
	}

	public async Task<GenericResponse<string>> MarkAsCompleteAsync(UpdateUserTaskCompleteStatusDto updateUserTaskCompleteStatus)
    {
        try
        {
            await _loggerManager.LogInfo($"Mark User Task as completed - {SerializeObject(updateUserTaskCompleteStatus)}");

            TaskUser? taskUserToUpdate = await _repositoryManager.TaskUserRepository.GetTaskUserById(updateUserTaskCompleteStatus.Id).Include(x => x.task).SingleOrDefaultAsync();

            if(taskUserToUpdate is null)
            {
                await _loggerManager.LogWarning($"User Task does not exist - {updateUserTaskCompleteStatus.Id}");
                return GenericResponse<string>.Failure(null, HttpStatusCode.NotFound, $"User Task does not exist.");
            }

            if(taskUserToUpdate.CompletionDate.HasValue)
            {
                await _loggerManager.LogWarning($"User Task already completed at {taskUserToUpdate.CompletionDate}");
                return GenericResponse<string>.Failure("Operation Failed", HttpStatusCode.Conflict, $"User Task already completed at: {taskUserToUpdate.CompletionDate.Value.Date}", null);
            }

            if(!string.IsNullOrEmpty(taskUserToUpdate.CancelReason))
            {
                await _loggerManager.LogWarning($"Task is already cancelled with reason: {taskUserToUpdate.CancelReason}");
                return GenericResponse<string>.Failure("Operation Failed", HttpStatusCode.Conflict, $"User Task already cancelled.", null);
            }

            string loggedInUserId = _httpContextAccessor.HttpContext.User.FindFirst(x => x.Type.EndsWith("claims/serialnumber"))?.Value ?? "0";

            if(!string.Equals(loggedInUserId, taskUserToUpdate.task.UserId.ToString(), StringComparison.OrdinalIgnoreCase) && !string.Equals(loggedInUserId, taskUserToUpdate.UserId.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                await _loggerManager.LogWarning($"Logged in user: {loggedInUserId} cannot perform operation.Can either be: {taskUserToUpdate.task.UserId} or {taskUserToUpdate.UserId}");
                return GenericResponse<string>.Failure("Operation Failed.", HttpStatusCode.BadRequest, "Cannot perform operation.", null);
            }

            taskUserToUpdate.CompletionDate = DateTime.UtcNow;

            _repositoryManager.TaskUserRepository.UpdateTaskUser(taskUserToUpdate);

            await _repositoryManager.SaveChangesAsync();

            await _loggerManager.LogInfo($"Mark User Task Completion Successful - {SerializeObject(updateUserTaskCompleteStatus)}");

            return GenericResponse<string>.Success("Operation Success", HttpStatusCode.OK, $"User Task marked as completed.");

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

    public async Task<GenericResponse<string>> ReassignTaskToUser(ReassignTaskUserDto reassignTaskUserDto)
    {
        try
        {
            await _loggerManager.LogInfo($"Reassign User Task - {SerializeObject(reassignTaskUserDto)}");

            string loggedInUserId = _httpContextAccessor.HttpContext.User.FindFirst(x => x.Type.EndsWith("serialnumber"))?.Value ?? "0";

            TaskUser? taskUserToReassign = await _repositoryManager.TaskUserRepository.GetTaskUserById(reassignTaskUserDto.UserTaskId, true)
                                                                        .Include(x => x.task)
                                                                        .Include(x => x.user)
                                                                        .SingleOrDefaultAsync();
            if(taskUserToReassign is null)
            {
                await _loggerManager.LogWarning($"User Task with specified Id does not exist: {reassignTaskUserDto.UserTaskId}");
                return GenericResponse<string>.Failure("Operation Failed.", HttpStatusCode.NotFound, "Invalid User Task Id", null);
            }

            bool isCreator = loggedInUserId == taskUserToReassign.task.UserId.ToString();

            bool isPrivilegedUser =
                            _httpContextAccessor.HttpContext.User.IsInRole("ADMIN") ||
                            _httpContextAccessor.HttpContext.User.HasClaim("isUnitHead", "true");

            if (!isCreator && !isPrivilegedUser)
            {
                await _loggerManager.LogWarning($"Logged in user and created task user mismatch. Logged in user: {loggedInUserId}. Created Task User Id: {taskUserToReassign.task.UserId.ToString()}");
                return GenericResponse<string>.Failure("Operation Failed", HttpStatusCode.Conflict, "Invalid Credentials.", null);
            }

            if(reassignTaskUserDto.UserId.ToString().Equals(taskUserToReassign.task.UserId.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                await _loggerManager.LogWarning($"Parent Task Createdd By: {taskUserToReassign.task.UserId} is the same as the newly assigned user: {reassignTaskUserDto.UserId}");
                return GenericResponse<string>.Failure("Operation Failed", HttpStatusCode.Conflict, "Invalid Credentials.Created By User cannot be a recipient.", null);
            }

            User? userToAssignTask = await _repositoryManager.UserRepository.GetById(reassignTaskUserDto.UserId, false)
                                                                .Include(x => x.AssignedUnit)
                                                                .SingleOrDefaultAsync();

            if(userToAssignTask is null)
            {
                await _loggerManager.LogWarning($"User with Id: {reassignTaskUserDto.UserId} does not exist.");
                return GenericResponse<string>.Failure("Operation Failed", HttpStatusCode.NotFound, "Invalid User Id.", null);
            }

            //User? currentPendingUser = await _repositoryManager.UserRepository.GetById(taskUserToReassign.UserId, false, false)
            //                                                        .Include(x => x.AssignedUnit)
            //                                                        .SingleOrDefaultAsync();

            if(userToAssignTask.UnitId != taskUserToReassign.user.UnitId)
            {
                await _loggerManager.LogInfo($"Current User and new user to reassign does not belong to same unit. Current User: {taskUserToReassign.user.UnitId} - Reassign User Unit: {userToAssignTask.UnitId}");
                return GenericResponse<string>.Failure($"Operation Failed.", HttpStatusCode.BadRequest, "User to assign task is not in the expected unit.", null);
            }

            taskUserToReassign.UserId = reassignTaskUserDto.UserId;

            _repositoryManager.TaskUserRepository.UpdateTaskUser(taskUserToReassign);

            await _repositoryManager.SaveChangesAsync();

            await _loggerManager.LogInfo($"Reassign User Task: {reassignTaskUserDto.TaskId} Operation successful by: {_httpContextAccessor.HttpContext.User.FindFirst(x => x.Type.EndsWith("claims/name"))?.Value}. Justification: {reassignTaskUserDto.Justification}");

            return GenericResponse<string>.Success("Operation Successful.", HttpStatusCode.OK, "User Task Reassigned Successfully.");
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

    public async Task<GenericResponse<TaskUserDto>> UpdateTaskUserAsync(UpdateTaskUserDto taskUserDto)
    {
        try
        {
            await _loggerManager.LogInfo($"Update Task User - {SerializeObject(taskUserDto)}");

            string loggedInUserId = _httpContextAccessor.HttpContext.User.FindFirst(x => x.Type.EndsWith("claims/serialnumber"))?.Value ?? "0";

            TaskUser? taskUserToUpdate = await _repositoryManager.TaskUserRepository.GetTaskUserById(taskUserDto.Id, true).Include(x => x.task).SingleOrDefaultAsync();

            if(taskUserToUpdate is null)
            {
                await _loggerManager.LogWarning($"User taks with Id: {taskUserDto.Id} does not exist.");
                return GenericResponse<TaskUserDto>.Failure(null, HttpStatusCode.NotFound, "Task User with Id does not exist", null);
            }

            if(!taskUserToUpdate.task.UserId.ToString().Equals(loggedInUserId, StringComparison.OrdinalIgnoreCase))
            {
                await _loggerManager.LogWarning($"Task Creation User: {taskUserToUpdate.task.UserId} is different from logged in user: {loggedInUserId}");
                return GenericResponse<TaskUserDto>.Failure(null, HttpStatusCode.BadRequest, "Invalid Credentials", null);
            }

            if(taskUserToUpdate.CompletionDate.HasValue)
            {
                await _loggerManager.LogWarning($"Task User with Id: {taskUserDto.Id} is already completed at: {taskUserToUpdate.CompletionDate}");
                return GenericResponse<TaskUserDto>.Failure(null, HttpStatusCode.Conflict, "User Task already completed.", null);
            }

            if (!string.IsNullOrEmpty(taskUserToUpdate.CancelReason))
            {
                await _loggerManager.LogWarning($"Task User with Id: {taskUserDto.Id} is already cancelled {taskUserToUpdate.CancelReason}");
                return GenericResponse<TaskUserDto>.Failure(null, HttpStatusCode.Conflict, "User Task already cancelled.", null);
            }

            taskUserToUpdate.Title = taskUserDto.Title;
            taskUserToUpdate.Description = taskUserDto.Description;
            taskUserToUpdate.ProposedCompletionDate = taskUserDto.ProposedCompletionDate.Date;

            _repositoryManager.TaskUserRepository.UpdateTaskUser(taskUserToUpdate);

            await _repositoryManager.SaveChangesAsync();

            await _loggerManager.LogInfo($"User Task successfully updated - {SerializeObject(taskUserDto)}");

            return GenericResponse<TaskUserDto>.Success(taskUserToUpdate.ToDto(), HttpStatusCode.OK, "User Task Update Successfully.");
        }
        catch (DbException ex)
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

    private string SerializeObject(object obj) => JsonSerializer.Serialize(obj);
}

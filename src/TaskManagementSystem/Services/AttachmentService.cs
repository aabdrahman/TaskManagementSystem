using Contracts;
using Contracts.Infrastructure;
using Entities.ConfigurationModels;
using Entities.Models;
using Infrastructure.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Service.Contract;
using Shared.ApiResponse;
using Shared.DataTransferObjects.Attachment;
using Shared.Mapper;
using System.Data.Common;
using System.Net;
using System.Text.Json;

namespace Services;

public sealed class AttachmentService : IAttachmentService
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly ILoggerManager _loggerManager;
    private readonly IInfrastructureManager _infrastructureManager;
    private UploadConfig _uploadFileConfig;
    public AttachmentService(IRepositoryManager repositoryManager, ILoggerManager loggerManager, IInfrastructureManager infrastructureManager, IOptionsMonitor<UploadConfig> uploadConfigOptionsMonitor)
    {
        _repositoryManager = repositoryManager;
        _loggerManager = loggerManager;
        _infrastructureManager = infrastructureManager;
        _uploadFileConfig = uploadConfigOptionsMonitor.CurrentValue as UploadConfig;
    }
    public Task<GenericResponse<IEnumerable<AttachmentDto>>> GetTaskAttachments(string taskId, bool hasQueryFilter)
    {
        throw new NotImplementedException();
    }

    public async Task<GenericResponse<string>> RemoveAttachment(int Id, bool isSoftDelete = true)
    {
        try
        {
            await _loggerManager.LogInfo($"Remove Attachment with Id: {Id}");

            Attachment? exisitngAttachmentRecord = await _repositoryManager.AttachmentRepository.GetByAttachmentId(Id, true, isSoftDelete ? true : false).SingleOrDefaultAsync();

            if(exisitngAttachmentRecord is null )
            {
                return GenericResponse<string>.Failure("Operation Failed", HttpStatusCode.NotFound, $"No Record with specified Id: {Id}", null);
            }

            if(isSoftDelete)
            {
                await _loggerManager.LogInfo($"Soft Delete Operation for Attachment. Task Id: {exisitngAttachmentRecord.TaskId} - {exisitngAttachmentRecord.FileName}");
                exisitngAttachmentRecord.IsDeleted = true;
                _repositoryManager.AttachmentRepository.UpdateAttachment(exisitngAttachmentRecord);
            }
            else
            {
                await _loggerManager.LogInfo($"Delete Operation for Attachment. Task Id: {exisitngAttachmentRecord.TaskId} - {exisitngAttachmentRecord.FileName}");
                string fileDeleteResponse = await _infrastructureManager.FileUtilityService.RemoveFileAsync(exisitngAttachmentRecord.TaskId, exisitngAttachmentRecord.FileName);
                await _loggerManager.LogInfo($"Delete Operation for Attachment. Task Id: {exisitngAttachmentRecord.TaskId} - {exisitngAttachmentRecord.FileName}. Response - {fileDeleteResponse}");

                if(string.Equals("success", fileDeleteResponse, StringComparison.OrdinalIgnoreCase) || string.Equals("FileNotFound", fileDeleteResponse, StringComparison.OrdinalIgnoreCase))
                {
                    _repositoryManager.AttachmentRepository.DeleteAttachment(exisitngAttachmentRecord);
                }
                else
                {
                    return GenericResponse<string>.Failure(fileDeleteResponse, HttpStatusCode.OK, "File Deletion Unsuccessful", null);
                }

            }

            await _repositoryManager.SaveChangesAsync();

            return GenericResponse<string>.Success("Operation Successful", HttpStatusCode.OK, "File Record Deletion successful.");

        }
        catch (DbException ex)
        {
            await _loggerManager.LogError(ex, "Internal Server Error Occurred - Database");
            return GenericResponse<string>.Failure("Error", HttpStatusCode.InternalServerError, "", new { ex.Message, description = ex?.InnerException?.Message });
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, "Internal Server Error Occurred");
            return GenericResponse<string>.Failure("Error", HttpStatusCode.InternalServerError, "", new { ex.Message, description = ex?.InnerException?.Message });
        }
    }

    public async Task<GenericResponse<string>> UploadAttachmentAsync(CreateAttachmentDto createAttachment)
    {
        try
        {
            await _loggerManager.LogInfo($"Uploading File for task Id: {createAttachment.TaskId} - {createAttachment.AttachmentFile.FileName}");

            if(!_uploadFileConfig.AllowedExtensions.Contains(Path.GetExtension(createAttachment.AttachmentFile.FileName)))
            {
                return GenericResponse<string>.Failure("Operation Failed.", HttpStatusCode.BadRequest, "Invalid File extension", null);
            }

            CreatedTask? existingTask = await _repositoryManager.CreatedTaskRepository.GetByTaskId(createAttachment.TaskId,  false, true).SingleOrDefaultAsync();

            if(existingTask is null)
            {
                await _loggerManager.LogWarning($"Task with specified Id does not exist: {createAttachment.TaskId}");
                return GenericResponse<string>.Failure("", HttpStatusCode.NotFound, "", null);
            }

            if(existingTask.TaskStage == Entities.StaticValues.Stage.Completed || existingTask.TaskStage == Entities.StaticValues.Stage.Cancelled)
            {
                await _loggerManager.LogWarning($"Specified Task is already completed. Sttaus - {existingTask.TaskStage.ToString()}");
                return GenericResponse<string>.Failure("Operaion Failed", HttpStatusCode.NotFound, $"Task Sttaus: {existingTask.TaskStage.ToString()}", null);
            }

            string uploadFileResponse = await _infrastructureManager.FileUtilityService.UploadFileAsync(createAttachment.TaskId, createAttachment.AttachmentFile);

            if(string.Equals("failed", uploadFileResponse, StringComparison.OrdinalIgnoreCase))
            {
                await _loggerManager.LogWarning($"File Upload Failed.");
                return GenericResponse<string>.Failure("Operaion Failed", HttpStatusCode.NotFound, $"File Upload Failled.", null);
            }

            Attachment? fileExistsForTask = await _repositoryManager.AttachmentRepository.GetAllAttachmentsByTaskId(createAttachment.TaskId, false, false).Where(x => x.FileName.Contains(createAttachment.AttachmentFile.FileName)).SingleOrDefaultAsync();

            if(fileExistsForTask is null)
            {
                Attachment attachmentToInsert = createAttachment.ToEntity();
                attachmentToInsert.IsDeleted = false;
                attachmentToInsert.CreatedBy = existingTask.CreatedBy;
                attachmentToInsert.FilePath = uploadFileResponse.Split("-", StringSplitOptions.TrimEntries)[1];
                attachmentToInsert.FileName = createAttachment.AttachmentFile.FileName;

                await _repositoryManager.AttachmentRepository.CreateAttachment(attachmentToInsert);
            }
            else
            {
                await _loggerManager.LogInfo($"File With name already exists for task - {JsonSerializer.Serialize(fileExistsForTask)}");

                fileExistsForTask.IsDeleted = false;
                _repositoryManager.AttachmentRepository.UpdateAttachment(fileExistsForTask);
            }

            await _repositoryManager.SaveChangesAsync();

            return GenericResponse<string>.Success("Operation Successful", HttpStatusCode.Created, "File Upload Successful");
        }
        catch(DbException ex)
        {
            await _loggerManager.LogError(ex, "Internal Server Error Occurred - Database");
            return GenericResponse<string>.Failure("Error", HttpStatusCode.InternalServerError, "", new { ex.Message, description = ex?.InnerException?.Message });
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, "Internal Server Error Occurred");
            return GenericResponse<string>.Failure("Error", HttpStatusCode.InternalServerError, "", new { ex.Message, description = ex?.InnerException?.Message });
        }
    }
}

using Contracts;
using Contracts.Infrastructure;
using Service.Contract;
using Shared.ApiResponse;
using Shared.DataTransferObjects.Attachment;

namespace Services;

public sealed class AttachmentService : IAttachmentService
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly ILoggerManager _loggerManager;
    public AttachmentService(IRepositoryManager repositoryManager, ILoggerManager loggerManager)
    {
        _repositoryManager = repositoryManager;
        _loggerManager = loggerManager;
    }
    public Task<GenericResponse<IEnumerable<AttachmentDto>>> GetTaskAttachments(string taskId, bool hasQueryFilter)
    {
        throw new NotImplementedException();
    }

    public Task<GenericResponse<string>> RemoveAttachment(int Id, bool isSoftDelete = true)
    {
        throw new NotImplementedException();
    }

    public Task<GenericResponse<string>> UploadAttachmentAsync(CreateAttachmentDto createAttachment)
    {
        throw new NotImplementedException();
    }
}

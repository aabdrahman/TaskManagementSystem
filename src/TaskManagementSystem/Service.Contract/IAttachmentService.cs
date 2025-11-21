using Shared.ApiResponse;
using Shared.DataTransferObjects.Attachment;

namespace Service.Contract;

public interface IAttachmentService
{
    Task<GenericResponse<string>> UploadAttachmentAsync(CreateAttachmentDto createAttachment);
    Task<GenericResponse<IEnumerable<AttachmentDto>>> GetTaskAttachments(string taskId, bool hasQueryFilter);
    Task<GenericResponse<string>> RemoveAttachment(int Id, bool isSoftDelete = true);
}

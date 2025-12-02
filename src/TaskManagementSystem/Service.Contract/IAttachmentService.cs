using Shared.ApiResponse;
using Shared.DataTransferObjects.Attachment;
using System.Threading.Tasks;

namespace Service.Contract;

public interface IAttachmentService
{
    Task<GenericResponse<string>> UploadAttachmentAsync(CreateAttachmentDto createAttachment);
    Task<GenericResponse<IEnumerable<AttachmentDto>>> GetTaskAttachments(string taskId, bool hasQueryFilter);
    Task<GenericResponse<string>> RemoveAttachment(int Id, bool isSoftDelete = true);
    Task<GenericResponse<string>> UploadMultipleAttachments(IEnumerable<CreateAttachmentDto> attachments);
    Task<GenericResponse<string>> UploadMultipleAttachments2(UploadMultipleAttachmentDto attachments);
}

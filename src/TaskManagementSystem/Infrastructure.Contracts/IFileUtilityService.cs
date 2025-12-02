using Microsoft.AspNetCore.Http;
using Shared.DataTransferObjects.Attachment;

namespace Infrastructure.Contracts;

public interface IFileUtilityService
{
    Task<string> UploadFileAsync(string taskId, IFormFile uploadedFile);
    Task<string> RemoveFileAsync(string taskId, string fileName);
    Task<MultipleAttachmentUploadResponse> UploadMultipleFilesAsync(IEnumerable<UploadAttachmentDto> uploadAttachments);
    Task<IEnumerable<Stream>> FetchTaskAttachments(string taskId);
}

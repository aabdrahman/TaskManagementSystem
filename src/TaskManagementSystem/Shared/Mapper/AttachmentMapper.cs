using Entities.Models;
using Shared.DataTransferObjects.Attachment;

namespace Shared.Mapper;

public static class AttachmentMapper
{
    public static Attachment ToEntity(this CreateAttachmentDto createAttachmentDto)
    {
        return new Attachment()
        {
            CreatedBy = "",
            TaskId = createAttachmentDto.TaskId,
            FileName = createAttachmentDto.AttachmentFile.FileName
        };
    }

    public static AttachmentDto TpDto(this Attachment attachment)
    {
        return new AttachmentDto()
        {
            Id = attachment.Id,
            TaskId=attachment.TaskId,
            FileName=attachment.FileName,
            FilePath = attachment.FilePath
        };
    }

    public static UploadAttachmentDto ToUploadAttachmentDto(this CreateAttachmentDto createAttachment)
    {
        return new UploadAttachmentDto(UploadAttachment: createAttachment.AttachmentFile, TaskId: createAttachment.TaskId);
    }

    public static IEnumerable<UploadAttachmentDto> ToUploadDto(this UploadMultipleAttachmentDto uploadMultipleAttachment)
    {
        IEnumerable<UploadAttachmentDto> uploadAttachments = [];

        foreach (var item in uploadMultipleAttachment.Attachments)
        {
            uploadAttachments.Append(new UploadAttachmentDto(item, uploadMultipleAttachment.TaskId));
        }

        return uploadAttachments;
    }

}

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
            FileName = createAttachmentDto.FileName
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
}

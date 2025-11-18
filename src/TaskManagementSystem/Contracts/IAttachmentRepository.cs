using Entities.Models;

namespace Contracts;

public interface IAttachmentRepository
{
    Task CreateAttachment(Attachment newAttachment);
    void DeleteAttachment(Attachment deletedAttachment);
    IQueryable<Attachment> GetAllAttachmentsByTaskId(string taskId, bool trackChanges = true, bool hasQueryFilter = true);
}

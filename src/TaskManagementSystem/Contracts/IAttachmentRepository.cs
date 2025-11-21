using Entities.Models;

namespace Contracts;

public interface IAttachmentRepository
{
    Task CreateAttachment(Attachment newAttachment);
    void DeleteAttachment(Attachment deletedAttachment);
    void UpdateAttachment(Attachment updatedAttachment);
    IQueryable<Attachment> GetAllAttachmentsByTaskId(string taskId, bool trackChanges = true, bool hasQueryFilter = true);
    IQueryable<Attachment> GetByAttachmentId(int Id, bool trackChanges, bool hasQueryFilter = true);
}

using Contracts;
using Entities.Models;

namespace Repository;

public sealed class AttachmentRepository : RepositoryBase<Attachment>, IAttachmentRepository
{
    public AttachmentRepository(RepositoryContext repositoryContext) : base(repositoryContext)
    {
    }

    public async Task CreateAttachment(Attachment newAttachment)
    {
        await Create(newAttachment);
    }

    public async Task CreateMultipleAttachments(IEnumerable<Attachment> newAttachments)
    {
        await CreateMultiple(newAttachments);
    }

    public void DeleteAttachment(Attachment deletedAttachment)
    {
        DeleteEntity(deletedAttachment);
    }

    public IQueryable<Attachment> GetAllAttachmentsByTaskId(string taskId, bool trackChanges = true, bool hasQueryFilter = true)
    {
        return FindByCondition(x => x.TaskId == taskId, trackChanges, hasQueryFilter);
    }

    public IQueryable<Attachment> GetByAttachmentId(int Id, bool trackChanges, bool hasQueryFilter = true)
    {
        return FindByCondition(x => x.Id == Id, trackChanges, hasQueryFilter);
    }

    public void UpdateAttachment(Attachment updatedAttachment)
    {
        UpdateEntity(updatedAttachment);
    }
}

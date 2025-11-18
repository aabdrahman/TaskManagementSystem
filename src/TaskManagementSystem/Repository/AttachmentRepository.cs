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

    public void DeleteAttachment(Attachment deletedAttachment)
    {
        DeleteEntity(deletedAttachment);
    }

    public IQueryable<Attachment> GetAllAttachmentsByTaskId(string taskId, bool trackChanges = true, bool hasQueryFilter = true)
    {
        return FindByCondition(x => x.TaskId == taskId, trackChanges, hasQueryFilter);
    }
}

using Contracts;
using Entities.Models;

namespace Repository;

public sealed class UnitRepository : RepositoryBase<Unit>, IUnitRepository
{
    public UnitRepository(RepositoryContext repositoryContext) : base(repositoryContext)
    {
    }

    public async Task CreateUnit(Unit newUnit)
    {
        await Create(newUnit);
    }

    public void DeleteUnit(Unit entity)
    {
        DeleteEntity(entity);
    }

    public IQueryable<Unit> GetAllUnits(bool trackChanges = true, bool hasQueryFilter = true)
    {
        return FindAll(trackChanges, hasQueryFilter);
    }

    public IQueryable<Unit> GetById(int Id, bool trackChanges = true, bool hasQueryFilter = true)
    {
        return FindByCondition(x => x.Id == Id, trackChanges, hasQueryFilter);
    }

    public void UpdateUnit(Unit updatedEntity)
    {
        UpdateEntity(updatedEntity);
    }
}
 
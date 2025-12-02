using Entities.Models;

namespace Contracts;

public interface IUnitRepository
{
    Task CreateUnit(Unit newUnit);
    void UpdateUnit(Unit updatedEntity);
    void DeleteUnit(Unit entity);
    IQueryable<Unit> GetAllUnits(bool trackChanges = true, bool hasQueryFilter = true);
    IQueryable<Unit> GetById(int Id, bool trackChanges = true, bool hasQueryFilter = true);
}

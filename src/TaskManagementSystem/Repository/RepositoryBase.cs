using Contracts;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
{
    private readonly RepositoryContext _repositoryContext;

    public RepositoryBase(RepositoryContext repositoryContext)
    {
        _repositoryContext = repositoryContext;
    }

    public async Task Create(T entity)
    {
        await _repositoryContext.Set<T>().AddAsync(entity);
    }

    public async Task CreateMultiple(IEnumerable<T> entities)
    {
        await _repositoryContext.AddRangeAsync(entities);
    }

    public async Task<IQueryable<T>> CustomeDatabaseQuery(string command, params object[] parameters)
    {
        return await Task.FromResult(_repositoryContext.Database.SqlQueryRaw<T>(command, parameters));
    }

    public void DeleteEntity(T entity)
    {
        _repositoryContext.Set<T>().Remove(entity);
    }

    public async Task<int> ExecuteCustomQuery(string command, params object[] parameters)
    {
        return await _repositoryContext.Database.ExecuteSqlRawAsync(command, parameters);
    }

    public IQueryable<T> FindAll(bool trackChanges = true, bool hasQueryFilter = true)
    {
        IQueryable<T> entities = _repositoryContext.Set<T>();

        if (!trackChanges)
            entities = entities.AsNoTracking();

        if(!hasQueryFilter)
            entities = entities.IgnoreQueryFilters();

        return entities;
    }

    public IQueryable<T> FindByCondition(System.Linq.Expressions.Expression<Func<T, bool>> condition, bool trackChanges = true, bool hasQueryFilter = true)
    {
        var entities = _repositoryContext.Set<T>()
                                .Where(condition);

        if(!trackChanges)
            entities = entities.AsNoTracking();

        if(!hasQueryFilter)
            entities = entities.IgnoreQueryFilters();  

        return entities;
    }

    public void UpdateEntity(T entity)
    {
       _repositoryContext.Set<T>().Update(entity);
    }
}

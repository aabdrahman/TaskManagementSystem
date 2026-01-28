using System.Linq.Expressions;

namespace Contracts;

public interface IRepositoryBase<T>
{
    Task Create(T entity);
    Task CreateMultiple(IEnumerable<T> entities);
    void UpdateEntity(T entity);
    void DeleteEntity(T entity);
    IQueryable<T> FindAll(bool trackChanges = true, bool hasQueryFilter = true);
    IQueryable<T> FindByCondition(Expression<Func<T, bool>> condition, bool trackChanges = true, bool hasQueryFilter = true);
    Task<int> ExecuteCustomQuery(string command, params object[] parameters);
    Task<IQueryable<T>> CustomeDatabaseQuery(string command, params object[] parameters);
    Task<IQueryable<T>> CustomDatabaseQueryWithListResult(string command, params object[] parameters);
}

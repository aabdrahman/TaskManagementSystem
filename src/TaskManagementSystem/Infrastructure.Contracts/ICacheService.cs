namespace Infrastructure.Contracts;

public interface ICacheService
{
    Task<string> GetFromCache(string cacheKey, CancellationToken cancellationToken = default);
    Task<bool> RemoveFromCache(string cacheKey = "", CancellationToken cancellationToken = default);
    Task<bool> CreateNewCache(string cacheKey, string valueToCache, CancellationToken cancellationToken = default);
    Task<bool> AddNewFusionCache<T>(string key, T value, CancellationToken cancellationToken = default);
    Task<T?> GetFromFusionCache<T>(string key, CancellationToken cancellationToken = default);
    Task<bool> RemoveFromFusionCache(string key, CancellationToken cancellationToken = default);
    Task<bool> RemoveMulipeFromFusionCache(params string[] keys);
}

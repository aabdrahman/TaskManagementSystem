using Contracts.Infrastructure;
using Infrastructure.Contracts;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Caching.Hybrid;
using System.Collections.Concurrent;
using ZiggyCreatures.Caching.Fusion;

namespace Infrastructure;

public sealed class CacheService : ICacheService
{
    private readonly ILoggerManager _loggerManager;
    private readonly HybridCache _hybridCache;
    private readonly IFusionCache _fusionCache;

    private ConcurrentDictionary<string, string> _cacheStore;

    public CacheService(ILoggerManager loggerManager, HybridCache hybridCache, IFusionCache fusionCache)
    {
        _loggerManager = loggerManager;
        _hybridCache = hybridCache;
        _cacheStore = new ConcurrentDictionary<string, string>();
        _fusionCache = fusionCache;
    }

    public async Task<bool> AddNewFusionCache<T>(string key, T value, CancellationToken cancellationToken = default)
    {
        try
        {
            await _loggerManager.LogInfo($"Creating New Cache record. Key: {key}");

            await _fusionCache.SetAsync<T>(key, value, token: cancellationToken);

            await _loggerManager.LogInfo($"Create New cache record Successful. Key: {key}");

            return true;

        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, $"Error setting new cache - Key:{key}");
            return false;
        }
    }

    public async Task<bool> CreateNewCache(string cacheKey, string valueToCache, CancellationToken cancellationToken = default)
    {
        try
        {
            await _loggerManager.LogInfo($"Creating new cache record - Key: {cacheKey}, Value: {valueToCache} - Key:{cacheKey}, Value: {valueToCache}");
            var isSetSuccessful = _cacheStore.TryAdd(cacheKey, valueToCache);

            await _loggerManager.LogInfo($"Create New cache record {(isSetSuccessful ? "Successful" : "Failed")}");

           //await _hybridCache.SetAsync(cacheKey, valueToCache, cancellationToken: cancellationToken);

            return isSetSuccessful;
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, $"Error setting new cache - Key:{cacheKey}, Value: {valueToCache}");
            return false;
        }
    }

    public async Task<string> GetFromCache(string cacheKey, CancellationToken cancellationToken = default)
    {
        try
        {
            await _loggerManager.LogInfo($"Retrieving stored cache: {cacheKey}");


            bool cachedValue = _cacheStore.TryGetValue(cacheKey, out string? fetchedCachedValue);

            await _loggerManager.LogInfo($"Stored cache value key: {cacheKey} returns - {cachedValue}");

            return fetchedCachedValue ?? "";
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, $"Error Fetching from cache. Key: {cacheKey} - {ex.Message}");
            return "";
        }
    }

    public async Task<T?> GetFromFusionCache<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _loggerManager.LogInfo($"Fetching cached result for key: {key}");

            T? result = await _fusionCache.GetOrDefaultAsync<T>(key, token:  cancellationToken);

            return result;
        }
        catch (Exception ex)
        {
            return default;
        }
    }

    public async Task<bool> RemoveFromCache(string cacheKey = "", CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(cacheKey))
            {
                await _loggerManager.LogInfo($"Removing All cached result.....");

                _cacheStore.Clear();

                return true;
            }
            else
            {
                await _loggerManager.LogInfo($"Removing cache with Key: {cacheKey}");
                bool isRemovedSuccessfully = _cacheStore.TryRemove(cacheKey, out string? removedValue);

                await _loggerManager.LogInfo($"Removed cached details: {(isRemovedSuccessfully ? "Successfully" : "Failed")}. Key: {cacheKey}, Value: {removedValue}");

                return isRemovedSuccessfully;
            }
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, $"Error removing cached value: {cacheKey}");
            return false;
        }

    }

    public async Task<bool> RemoveFromFusionCache(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _loggerManager.LogInfo($"Removing cache with Key: {key}");

            await _fusionCache.RemoveAsync(key, token: cancellationToken);

            await _loggerManager.LogInfo($"Cached key: {key} successfully removed.");

            return true;
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, $"Error removing cached value: {key}");
            return false;
        }
    }

    public async Task<bool> RemoveMulipeFromFusionCache(params string[] keys)
    {
        try
        {
            await _loggerManager.LogInfo($"Removing items with key: {string.Join(", ", keys)}");

            foreach (var item in keys)
            {
                await _fusionCache.RemoveAsync(item);
            }

            return true;
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, $"Error removing cached values: {string.Join(", ", keys)}");
            return false;
        }
    }
}

using Microsoft.Extensions.Diagnostics.HealthChecks;
using ZiggyCreatures.Caching.Fusion;

namespace Infrastructure.HealthChecks;

public sealed class CachingServiceHealthCheck : IHealthCheck
{
    private IFusionCache _fusionCache;
    private const string _heathcheck_cache_key = "health_check:fusion";
    private const string _health_check_dummy_value = "01";

    public CachingServiceHealthCheck(IFusionCache fusionCache)
    {
        _fusionCache = fusionCache;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {

            await _fusionCache.SetAsync(_heathcheck_cache_key, _health_check_dummy_value, new FusionCacheEntryOptions() { DistributedCacheDuration = TimeSpan.FromSeconds(5), Duration = TimeSpan.FromSeconds(5) }, token: cancellationToken);

            string? _health_check_cache_value = await _fusionCache.GetOrDefaultAsync<string>(_heathcheck_cache_key, token: cancellationToken);

            return _health_check_cache_value == _health_check_dummy_value ?
                HealthCheckResult.Healthy("Caching Service Available.") :
                HealthCheckResult.Degraded("Caching Service Returns Unexpected Result.");
;        }
        catch (Exception ex)
        {
            return HealthCheckResult.Degraded("Caching Service Unavailable", ex);
        }
    }
}

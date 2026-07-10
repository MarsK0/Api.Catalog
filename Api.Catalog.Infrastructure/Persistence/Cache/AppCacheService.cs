using Api.Catalog.Infrastructure.Contracts;
using Microsoft.Extensions.Caching.Hybrid;

namespace Api.Catalog.Infrastructure.Persistence.Cache;

internal sealed class AppCacheService : ICacheService
{
    private readonly TimeProvider _timeProvider;
    private readonly HybridCache _cache;
    public AppCacheService(
        TimeProvider timeProvider,
        HybridCache cache
    )
    {
        _timeProvider = timeProvider;
        _cache = cache;
    }
    public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, CacheOptions? options = null)
    {
        return await _cache.GetOrCreateAsync(
            key,
            _ => new ValueTask<T>(factory()),
            MapOptions(options) 
        );
    }

    public async Task RemoveAsync(string key) => await _cache.RemoveAsync(key);

    private HybridCacheEntryOptions MapOptions(CacheOptions? options)
    {
        if(
            options is null || 
            (
                options.AbsoluteExpiration is null && 
                options.RelativeExpiration is null
            )
        )
            return new HybridCacheEntryOptions();

        return new HybridCacheEntryOptions
        {
            Expiration = options.AbsoluteExpiration.HasValue
                ? options.AbsoluteExpiration.Value - _timeProvider.GetUtcNow()
                : options.RelativeExpiration
        };
    }
}

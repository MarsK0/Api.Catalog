using Api.Catalog.Infrastructure.Contracts;
using Microsoft.Extensions.Caching.Hybrid;

namespace Api.Catalog.Infrastructure.Persistence.Cache;

internal sealed class AppCacheService(
    TimeProvider timeProvider,
    HybridCache cache
    ) : ICacheService
{
    public async Task<T> GetOrCreateAsync<T>(string key, Func<CancellationToken, Task<T>> factory, CancellationToken ct, CacheOptions? options = null)
    {
        return await cache.GetOrCreateAsync(
            key,
            cacheCt => new ValueTask<T>(factory(cacheCt)),
            MapOptions(options),
            null,
            ct
        );
    }

    public async Task RemoveAsync(string key, CancellationToken ct) => await cache.RemoveAsync(key, ct);

    private HybridCacheEntryOptions MapOptions(CacheOptions? options)
    {
        if (
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
                ? options.AbsoluteExpiration.Value - timeProvider.GetUtcNow()
                : options.RelativeExpiration
        };
    }
}

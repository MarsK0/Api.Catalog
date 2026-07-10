using Api.Catalog.Infrastructure.Contracts;
using Microsoft.Extensions.Caching.Memory;

namespace Api.Catalog.Infrastructure.Persistence.Cache;

internal sealed class AppCacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    public AppCacheService(
        IMemoryCache cache
    )
    {
        _cache = cache;
    }
}

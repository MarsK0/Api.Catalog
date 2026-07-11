namespace Api.Catalog.Infrastructure.Contracts;

public record CacheOptions(
    DateTimeOffset? AbsoluteExpiration = null,
    TimeSpan? RelativeExpiration = null
);
public interface ICacheService
{
    Task<T> GetOrCreateAsync<T>(string key, Func<CancellationToken, Task<T>> factory, CancellationToken ct, CacheOptions? options = null);
    Task RemoveAsync(string key, CancellationToken ct);
}

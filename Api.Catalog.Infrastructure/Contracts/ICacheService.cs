namespace Api.Catalog.Infrastructure.Contracts;

public record CacheOptions(
    DateTimeOffset? AbsoluteExpiration = null,
    TimeSpan? RelativeExpiration = null
);
public interface ICacheService
{
    Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, CacheOptions? options = null);
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, CacheOptions? options = null);
    Task RemoveAsync(string key);
}

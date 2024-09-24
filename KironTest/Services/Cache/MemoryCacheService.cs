using Microsoft.Extensions.Caching.Memory;

namespace KironTest.Services.Cache;
public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;

    public MemoryCacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public async Task<T> GetOrCreateAsync<T>(string cacheKey, Func<Task<T>> factory, TimeSpan? expiration = null)
    {
        // Check if the cache already contains the value
        if (!_cache.TryGetValue(cacheKey, out T cachedValue))
        {
            // If the value is not found, retrieve it using the factory method
            cachedValue = await factory();

            // Set cache options (expiration time)
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(10) // Default expiration to 10 minutes
            };

            // Cache the retrieved value
            _cache.Set(cacheKey, cachedValue, cacheOptions);
        }

        return cachedValue;
    }

    public void Remove(string cacheKey)
    {
        _cache.Remove(cacheKey);
    }
}

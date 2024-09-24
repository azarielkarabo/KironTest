namespace KironTest.Services.Cache
{
    public interface ICacheService
    {
        Task<T> GetOrCreateAsync<T>(string cacheKey, Func<Task<T>> factory, TimeSpan? expiration = null);

        void Remove(string cacheKey);
    }
}

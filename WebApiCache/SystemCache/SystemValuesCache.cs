using System.Runtime.Caching;

namespace WebApiCache.SystemCache;

public static class SystemValuesCache
{
    private static readonly MemoryCache _cache;
    private static DateTime _lastUpdateTime = DateTime.MinValue;
    private static readonly object LockObject = new();
    static SystemValuesCache()
    {
        _cache ??= new MemoryCache("WebApiCache");
    }

    public static CachedData<T> GetCachedData<T>(string cacheKey, Func<Task<List<T>>> fetchDataFunction, double cacheTimeInSeconds)
    {
        List<T> cachedValues = _cache.Get(cacheKey) as List<T> ?? new List<T>();

        if (cachedValues.Count == 0 || DateTime.Now - _lastUpdateTime > TimeSpan.FromSeconds(cacheTimeInSeconds))
        {
            lock (LockObject)
            {
                Task.Run(async () =>
                {
                    var data = await fetchDataFunction();
                    cachedValues.Clear();
                    cachedValues.AddRange(data);
                    var cachePolicy = new CacheItemPolicy
                    {
                        AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(cacheTimeInSeconds)
                    };
                    _cache.Set(cacheKey, cachedValues, cachePolicy);
                    _lastUpdateTime = DateTime.Now;
                }).Wait();
            }
        }
        return new CachedData<T>()
        {
            Data = cachedValues,
            LastUpdateTime = _lastUpdateTime,
            NextUpdateTime = _lastUpdateTime.AddSeconds(cacheTimeInSeconds)
        };
    }

}

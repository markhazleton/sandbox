using System.Runtime.Caching;

namespace WebApiCache.SystemCache;

public static class SystemValuesCache
{
    private static readonly MemoryCache _cache;
    private static readonly object LockObject = new();
    static SystemValuesCache()
    {
        _cache ??= new MemoryCache("WebApiCache");
    }

    public static CachedData<T> GetCachedData<T>(string cacheKey, Func<Task<List<T>>> fetchDataFunction, double cacheTimeInSeconds)
    {
        CachedData<T> cachedValues = _cache.Get(cacheKey) as CachedData<T> ?? new CachedData<T>();
        try
        {
            if (cachedValues.Data.Count > 1)
                return cachedValues;
        }
        finally
        {
            if (cachedValues.Data.Count == 0)
            {
                Task.Run(async () =>
                {
                    await UpdateCache(cacheKey, fetchDataFunction, cacheTimeInSeconds, cachedValues);
                }).Wait();
            }
            else
            {
                if (DateTime.Now - cachedValues.LastUpdateTime > TimeSpan.FromSeconds(cacheTimeInSeconds))
                {
                    Task.Run(async () =>
                    {
                        await UpdateCache(cacheKey, fetchDataFunction, cacheTimeInSeconds, cachedValues);
                    });
                }
            }
        }
        return cachedValues;
    }

    private static async Task UpdateCache<T>(string cacheKey, Func<Task<List<T>>> fetchDataFunction, double cacheTimeInSeconds, CachedData<T> cachedValues)
    {
        var data = await fetchDataFunction();
        cachedValues.Data = data.ToList();
        cachedValues.LastUpdateTime = DateTime.Now;
        cachedValues.NextUpdateTime = cachedValues.LastUpdateTime.AddSeconds(cacheTimeInSeconds);

        var cachePolicy = new CacheItemPolicy
        {
            AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(cacheTimeInSeconds + 30)
        };
        lock (LockObject)
        {
            _cache.Set(cacheKey, cachedValues, cachePolicy);
        }
    }
}

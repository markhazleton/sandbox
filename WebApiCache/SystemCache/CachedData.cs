namespace WebApiCache.SystemCache;

public class CachedData<T>
{
    public DateTime LastUpdateTime { get; set; }
    public DateTime NextUpdateTime { get; set; }
    public List<T> Data { get; set; }
}

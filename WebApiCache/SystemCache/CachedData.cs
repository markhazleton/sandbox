namespace WebApiCache.SystemCache;

public class CachedData<T>
{
    public int Counter { get; set; }
    public DateTime LastUpdateTime { get; set; } = DateTime.MinValue;
    public DateTime NextUpdateTime { get; set; } = DateTime.MinValue;
    public List<T> Data { get; set; } = new List<T>();
}

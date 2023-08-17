using System.Collections.Concurrent;
using System.Runtime.Serialization;
using System.Text;

namespace ThreadSafeDictionary;

public sealed class PropertyBag<TKey, TValue>
    where TKey : notnull
    where TValue : notnull
{
    /// <summary>
    /// The dictionary
    /// </summary>
    private readonly ConcurrentDictionary<TKey, TValue> _Dictionary = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyBag{TKey, TValue}"/> class.
    /// </summary>
    public TValue? this[TKey key]
    {
        get => _Dictionary.TryGetValue(key, out TValue? value) ? value : default;
        set
        {
            if (value is null)
                throw new ArgumentException("Value cannot be null");

            _Dictionary[key] = value;
        }
    }

    /// <summary>
    /// Adds the specified dictionary into the current dictionary
    /// </summary>
    /// <param name="value">The value.</param>
    public void Add(Dictionary<TKey, TValue> value)
    {
        foreach (var item in value.Keys)
        {
            Add(item, value[item]);
        }
    }


    /// <summary>
    /// Adds the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    public void Add(TKey key, TValue? value)
    {
        if (value is null) return;
        _Dictionary[key] = value;
    }

    /// <summary>
    /// Gets the list.
    /// </summary>
    /// <returns>List&lt;System.String&gt;.</returns>
    public List<string> GetList()
    {
        List<string> list = new();
        foreach (var item in _Dictionary)
        {
            list.Add($"{item.Key} - {item.Value}");
        }
        return list;
    }

    /// <summary>
    /// Gets the object data.
    /// </summary>
    /// <param name="info">The information.</param>
    public void GetObjectData(SerializationInfo info)
    {
        foreach (TKey key in _Dictionary.Keys)
        {
            if (key != null)
            {
                info.AddValue(key.ToString()!, _Dictionary[key]);
            }
        }
    }

    public override string ToString()
    {
        string separator = ", ";

        if (_Dictionary is null || _Dictionary.IsEmpty)
            return string.Empty;

        if (_Dictionary.Count == 1)
            return $"{_Dictionary.First().Key}:{_Dictionary.First().Value}";

        StringBuilder sb = new();
        int counter = 0;
        foreach (var item in _Dictionary)
        {
            if (counter == 0)
            {
                sb.Append($"{item.Key}:{item.Value}");
            }
            else
            {
                sb.Append($"{separator}{item.Key}:{item.Value}");
            }
            counter++;
        }
        return sb.ToString();
    }
}
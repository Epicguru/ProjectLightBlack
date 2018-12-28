
using System.Collections.Generic;

public class TileData
{
    private Dictionary<string, object> data = new Dictionary<string, object>();

    public bool Contains(string key)
    {
        return data.ContainsKey(key);
    }

    public void Set(string key, object value)
    {
        if (data.ContainsKey(key))
            data[key] = value;
        else
            data.Add(key, value);
    }

    public T Get<T>(string key)
    {
        return Get(key, default(T));
    }

    public T Get<T>(string key, T defaultValue)
    {
        if (data.ContainsKey(key))
            return (T)data[key];
        else
            return defaultValue;
    }
}

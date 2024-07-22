using System.Text.Json;
using Profiler.Interfaces;

namespace Profiler.Helpers;

public class LocalStorageHelper : ILocalStorageHelper
{
    public T Load<T>(string path) where T : new()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            using FileStream fileStream = new(path, FileMode.Open, FileAccess.Read);
            return JsonSerializer.Deserialize<T>(fileStream)!;
        }
        catch (FileNotFoundException)
        {
            T type = new();
            Save(type, path);
            return type;
        }
    }

    public void Save<T>(T type, string path)
    {
        using FileStream fileStream = new(path, FileMode.Create, FileAccess.Write);
        JsonSerializer.Serialize(fileStream, type);
    }
}
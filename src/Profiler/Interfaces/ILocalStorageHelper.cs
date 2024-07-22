namespace Profiler.Interfaces;

public interface ILocalStorageHelper
{
    public T Load<T>(string path) where T : new();
    
    public void Save<T>(T type, string path);
}
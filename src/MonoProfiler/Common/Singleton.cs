using System;

namespace MonoProfiler.Common;

public class Singleton<T> where T : class, new()
{
    private static readonly Lazy<T> _instance = new(CreateInstance, true);
        
    public static T Instance => _instance.Value;

    protected Singleton() { }

    private static T CreateInstance() => new();
}
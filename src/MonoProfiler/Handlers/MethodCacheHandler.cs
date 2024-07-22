using System;
using System.Collections.Concurrent;
using MonoProfiler.Common;

namespace MonoProfiler.Handlers;

public class MethodCacheHandler : Singleton<MethodCacheHandler>
{
    private readonly ConcurrentDictionary<IntPtr, bool> _cache = new();

    public void Set(IntPtr key, bool value) => _cache[key] = value;
    
    public bool TryGet(IntPtr key, out bool value) => _cache.TryGetValue(key, out value);

    public void Invalidate() => _cache.Clear();
}
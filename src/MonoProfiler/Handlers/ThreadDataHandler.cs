using System;
using System.Collections.Generic;
using System.Threading;
using MonoProfiler.Common;
using MonoProfiler.Models;
using MonoProfiler.Models.Results;

namespace MonoProfiler.Handlers;

public class ThreadDataHandler : Singleton<ThreadDataHandler>
{
    private readonly ThreadLocal<ThreadData> _localThreadData = new(true);
    private readonly ThreadData _mergedThreadData = new();

    public ThreadData GetThreadData()
    {
        if (_localThreadData.IsValueCreated)
        {
            return _localThreadData.Value;
        }

        ThreadData threadData = new();
        SetThreadData(threadData);
        return threadData;
    }
    
    public ThreadData GetMergedThreadData()
    {
        _mergedThreadData.MethodStack.Clear();
        _mergedThreadData.MethodResults.Clear();
        _mergedThreadData.MemoryResults.Clear();
        _mergedThreadData.GcRecord = default;
        
        IList<ThreadData> threadsData = GetThreadsData();
        foreach (ThreadData threadData in threadsData)
        {
            MergeThreadData(threadData, _mergedThreadData);
        }

        return _mergedThreadData;
    }
    
    private IList<ThreadData> GetThreadsData() => _localThreadData.Values;
    
    private void SetThreadData(ThreadData threadData) => _localThreadData.Value = threadData;

    private void MergeThreadData(ThreadData source, ThreadData destination)
    {
        foreach ((IntPtr key, MethodResult value) in source.MethodResults)
        {
            if (!destination.MethodResults.TryGetValue(key, out MethodResult methodResult))
            {
                destination.MethodResults.Add(key, value);
                continue;
            }

            methodResult.Calls += value.Calls;
            methodResult.OwnAllocations += value.OwnAllocations;
            methodResult.TotalAllocations += value.TotalAllocations;
            methodResult.OwnExceptions += value.OwnExceptions;
            methodResult.TotalExceptions += value.TotalExceptions;
            methodResult.OwnTime += value.OwnTime;
            methodResult.TotalTime += value.TotalTime;

            destination.MethodResults[key] = methodResult;
        }
        
        foreach ((IntPtr key, MemoryResult value) in source.MemoryResults)
        {
            if (!destination.MemoryResults.TryGetValue(key, out MemoryResult memoryResult))
            {
                destination.MemoryResults.Add(key, value);
                continue;
            }

            memoryResult.AllocationsCount += value.AllocationsCount;
            memoryResult.TotalAllocations += value.TotalAllocations;
            memoryResult.InstanceSize += value.InstanceSize;

            destination.MemoryResults[key] = memoryResult;
        }
        
        source.MethodStack.Clear();
        source.MethodResults.Clear();
        source.MemoryResults.Clear();
        source.GcRecord = default;
    }
}
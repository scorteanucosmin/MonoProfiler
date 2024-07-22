using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using MonoProfiler.Common;
using MonoProfiler.Handlers;
using MonoProfiler.Helpers;
using MonoProfiler.Models;
using MonoProfiler.Models.Mono;
using MonoProfiler.Models.Records;
using MonoProfiler.Models.Results;

namespace MonoProfiler;

public class Profiler : Singleton<Profiler>
{
    private readonly HashSet<string> _registeredAssemblies = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> _removingAssemblies = new();

    private readonly Dictionary<IntPtr, MethodRecord> _methodRecords = new();
    
    private readonly Dictionary<IntPtr, AssemblyRecord> _assemblyRecords = new();
    
    public ProfilerData Data { get; set; }
    
    public bool Started { get; private set; }

    public unsafe void Toggle(HashSet<string>? args)
    {
        void* profilerHandle = Data.Handle;
        if (&profilerHandle == null)
        {
            return;
        }
        
        switch (Started)
        {
            case true:
            {
                Started = false;
                CallbackHandler.Instance.RemoveCallbacks(profilerHandle);
                ProfilingSample profilingSample = GetProfilingSample(ThreadDataHandler.Instance.GetMergedThreadData());
                WebSocketServerHandler.Instance.SendDataAsync(profilingSample);
                break;
            }
            case false:
            {
                Started = true;
                if (args == null)
                {
                    CallbackHandler.Instance.RegisterCallbacks(profilerHandle);
                    return;
                }

                _removingAssemblies.Clear();
                
                bool invalidateCache = false;
                foreach (string registeredAssembly in _registeredAssemblies)
                {
                    if (args.Contains(registeredAssembly))
                    {
                        continue;
                    }

                    _removingAssemblies.Add(registeredAssembly);
                    invalidateCache = true;
                }
                
                _registeredAssemblies.ExceptWith(_removingAssemblies);

                if (invalidateCache)
                {
                    MethodCacheHandler.Instance.Invalidate();
                }
                
                foreach (string arg in args)
                {
                    RegisterAssembly(arg);
                }

                CallbackHandler.Instance.RegisterCallbacks(profilerHandle);
                break;
            }
        }
    }

    public void RegisterAssembly(string assembly) => _registeredAssemblies.Add(assembly);

    public void RemoveAssembly(string assembly) => _registeredAssemblies.Remove(assembly);

    public bool IsAssemblyRegistered(string assembly) => _registeredAssemblies.Contains(assembly);

    public void ClearRegisteredAssemblies() => _registeredAssemblies.Clear();
    
    private unsafe ProfilingSample GetProfilingSample(ThreadData threadData)
    {
        ProfilingSample profilingSample = new()
        {
            //GcRecord = threadData.GcRecord
        };
        
        _assemblyRecords.Clear();
        _methodRecords.Clear();
        
        foreach ((IntPtr monoClassPtr, MemoryResult memoryResult) in threadData.MemoryResults)
        {
            MonoClass* monoClass = (MonoClass*)monoClassPtr;
            string assemblyName = Marshal.PtrToStringAnsi((IntPtr)monoClass->image->assembly_name)!;
      
            profilingSample.MemoryRecords.Add(new MemoryRecord
            {
                AssemblyName = assemblyName,
                ClassName = MonoHelper.Instance.GetClassName(monoClass),
                AllocationsCount = memoryResult.AllocationsCount,
                TotalAllocations = memoryResult.TotalAllocations,
            });
        }
        
        foreach ((IntPtr monoMethodPtr, MethodResult methodResult) in threadData.MethodResults)
        {
            MonoMethod* monoMethod = (MonoMethod*)monoMethodPtr;
            string assemblyName = Marshal.PtrToStringAnsi((IntPtr)monoMethod->klass->image->assembly_name)!;
            IntPtr monoImagePtr = (IntPtr)monoMethod->klass->image;

            if (!_assemblyRecords.TryGetValue(monoImagePtr, out AssemblyRecord assemblyRecord))
            {
                assemblyRecord = new AssemblyRecord
                {
                    AssemblyName = assemblyName
                };
            }

            assemblyRecord.TotalCalls += methodResult.Calls;
            assemblyRecord.TotalTime += methodResult.OwnTime;
            assemblyRecord.TotalAllocations += methodResult.OwnAllocations;
            assemblyRecord.TotalExceptions += methodResult.OwnExceptions;
            _assemblyRecords[monoImagePtr] = assemblyRecord;

            if (!_methodRecords.TryGetValue(monoMethodPtr, out MethodRecord methodRecord))
            {
                methodRecord = new MethodRecord
                {
                    MethodName = MonoHelper.Instance.GetMethodFullName(monoMethod)
                };
            }

            methodRecord.Calls += methodResult.Calls;
            methodRecord.OwnTime += methodResult.OwnTime;
            methodRecord.TotalTime += methodResult.TotalTime;
            methodRecord.OwnAllocations += methodResult.OwnAllocations;
            methodRecord.TotalAllocations += methodResult.TotalAllocations;
            methodRecord.OwnExceptions += methodResult.OwnExceptions;
            methodRecord.TotalExceptions += methodResult.TotalExceptions;
            _methodRecords[monoMethodPtr] = methodRecord;
        }

        profilingSample.AssemblyRecords.AddRange(_assemblyRecords.Values);
        profilingSample.MethodRecords.AddRange(_methodRecords.Values);
        return profilingSample;
    }
}
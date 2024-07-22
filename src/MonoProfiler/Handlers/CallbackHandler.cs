using System;
using System.Collections.Generic;
using System.Diagnostics;
using MonoProfiler.Common;
using MonoProfiler.Enums;
using MonoProfiler.Helpers;
using MonoProfiler.Models;
using MonoProfiler.Models.Mono;
using MonoProfiler.Models.Results;

namespace MonoProfiler.Handlers;

public class CallbackHandler : Singleton<CallbackHandler>
{
    public unsafe void RegisterMethodFilterCallback(void* profilerHandle) => 
        Interop.SetMethodFilterCallback(profilerHandle, MethodFilterCallback);

    public unsafe void RegisterCallbacks(void* profilerHandle)
    {
        Interop.SetMethodEnterCallback(profilerHandle, MethodEnterCallback);
        Interop.SetMethodLeaveCallback(profilerHandle, MethodLeaveCallback);
        Interop.SetMethodExceptionLeaveCallback(profilerHandle, MethodExceptionLeaveCallback);
        Interop.SetExceptionThrowCallback(profilerHandle, ExceptionThrowCallback);
        Interop.SetGcAllocationCallback(profilerHandle, GcAllocationCallback);
        //Interop.SetGcEventCallback(profilerHandle, GcCollectCallback);
    }
    
    public unsafe void RemoveCallbacks(void* profilerHandle)
    {
        Interop.SetMethodEnterCallback(profilerHandle, null);
        Interop.SetMethodLeaveCallback(profilerHandle, null);
        Interop.SetMethodExceptionLeaveCallback(profilerHandle, null);
        Interop.SetExceptionThrowCallback(profilerHandle, null);
        Interop.SetGcAllocationCallback(profilerHandle, null);
        //Interop.SetGcEventCallback(profilerHandle, null);
    }
    
    private static unsafe CallInstrumentationFlags MethodFilterCallback(ProfilerData* profilerData, MonoMethod* monoMethod)
    {
        if (monoMethod->klass == null || monoMethod->klass->image == null)
        {
            return CallInstrumentationFlags.None;
        }
        
        return Constants.CallInstrumentationTypes;
    }
    
    private static unsafe void MethodEnterCallback(ProfilerData* profilerData, MonoMethod* monoMethod, MonoProfilerCallContext* context)
    {
        if (!Profiler.Instance.Started || !MonoHelper.Instance.CanProfileMonoMethod(monoMethod))
        {
            return;
        }
        
        ThreadDataHandler.Instance.GetThreadData().MethodStack.Add(new MethodData(monoMethod));
    }
    
    private static unsafe void MethodLeaveCallback(ProfilerData* profilerData, MonoMethod* monoMethod) => 
        HandleMethodLeaveCallback(monoMethod, false);

    private static unsafe void MethodExceptionLeaveCallback(ProfilerData* profilerData, MonoMethod* monoMethod, MonoObject* monoObject) =>
        HandleMethodLeaveCallback(monoMethod, true);

    private static unsafe void ExceptionThrowCallback(ProfilerData* profilerData, MonoMethod* monoMethod, MonoObject* monoObject)
    {
        if (!Profiler.Instance.Started)
        {
            return;
        }
        
        ThreadData threadData = ThreadDataHandler.Instance.GetThreadData();
        if (threadData.MethodStack.Count <= 0)
        {
            return;
        }
        
        MethodData lastMethodData = threadData.MethodStack[^1];
        IntPtr monoMethodPtr = (IntPtr)lastMethodData.Method;
        
        MethodResult methodResult = threadData.MethodResults.GetValueOrDefault(monoMethodPtr);
        methodResult.OwnExceptions += 1;
        
        threadData.MethodResults[monoMethodPtr] = methodResult;
    }

    private static unsafe void GcAllocationCallback(ProfilerData* profilerData, MonoObject* monoObject)
    {
        if (!Profiler.Instance.Started)
        {
            return;
        }

        ThreadData threadData = ThreadDataHandler.Instance.GetThreadData();
        int objectAllocation = MonoHelper.Instance.GetSizeOfMonoObject(monoObject);
        if (threadData.MethodStack.Count > 0)
        {
            Index lastIndex = ^1;
            MethodData lastMethodData = threadData.MethodStack[lastIndex];
            lastMethodData.OwnAllocations += objectAllocation;
            lastMethodData.TotalAllocations += objectAllocation;
            threadData.MethodStack[lastIndex] = lastMethodData;
        }

        MonoClass* monoClass = monoObject->vtable->klass;
        if (monoClass == null)
        {
            return;
        }

        IntPtr monoClassPtr = (IntPtr)monoClass;
        if (threadData.MemoryResults.TryGetValue(monoClassPtr, out MemoryResult memoryResult))
        {
            memoryResult.AllocationsCount += 1;
            memoryResult.TotalAllocations += objectAllocation;
            memoryResult.InstanceSize = monoClass->instance_size;
            
            threadData.MemoryResults[monoClassPtr] = memoryResult;
            return;
        }

        memoryResult = new MemoryResult
        {
            AllocationsCount = 1,
            TotalAllocations = objectAllocation,
            InstanceSize = monoClass->instance_size
        };

        threadData.MemoryResults[monoClassPtr] = memoryResult;
    }
    
    //TODO: Fix occasional NRE
    /*private static unsafe void GcCollectCallback(ProfilerData* profilerData, MonoProfilerGCEvent @event, uint _generation, bool _serial)
    {
        if (!Profiler.Instance.Started)
        {
            return;
        }

        ThreadData threadData = ThreadDataHandler.Instance.GetThreadData();
        if (!threadData.MainThread)
        {
            return;
        }

        switch (@event)
        {
            case MonoProfilerGCEvent.PreStopWorld:
            {
                GCRecord gcRecord = threadData.GcRecord;
                gcRecord.Timer = new Stopwatch();
                gcRecord.Timer.Start();
                threadData.GcRecord = gcRecord;
                break;
            }
            case MonoProfilerGCEvent.PostStartWorldUnlocked:
            {
                GCRecord gcRecord = threadData.GcRecord;
                Stopwatch timer = gcRecord.Timer;
                timer.Stop();
                gcRecord.TotalTime =+ timer.ElapsedTicks * Constants.ElapsedTicksMultiplier;
                gcRecord.Calls += 1;
                threadData.GcRecord = gcRecord;
                break;
            }
        }
    }*/
    
    private static unsafe void HandleMethodLeaveCallback(MonoMethod* monoMethod, bool isException)
    {
        if (!Profiler.Instance.Started || !MonoHelper.Instance.CanProfileMonoMethod(monoMethod))
        {
            return;
        }

        ThreadData threadData = ThreadDataHandler.Instance.GetThreadData();
        
        int index = threadData.MethodStack.FindLastIndex(methodData => methodData.Method == monoMethod);
        if (index == -1)
        {
            return;
        }
        
        MethodData methodData = threadData.MethodStack[index];
        Stopwatch timer = methodData.StackTime.Timer;
        timer.Stop();
        
        IntPtr monoMethodPtr = (IntPtr)monoMethod;
        MethodResult methodResult = threadData.MethodResults.GetValueOrDefault(monoMethodPtr);

        methodResult.Calls++;
        methodResult.OwnAllocations += methodData.OwnAllocations;
        int totalAlloc = methodData.TotalAllocations;
        methodResult.TotalAllocations += totalAlloc;
        
        if (isException)
        {
            methodResult.TotalExceptions++;
        }
        
        long methodDuration = timer.ElapsedTicks * Constants.ElapsedTicksMultiplier;
        
        methodResult.TotalTime += methodDuration;
        methodResult.OwnTime += methodDuration - methodData.StackTime.OtherMethodsDuration;

        threadData.MethodResults[monoMethodPtr] = methodResult;

        if (index > 0)
        {
            int previousIndex = index - 1;
            MethodData previousMethodData = threadData.MethodStack[previousIndex];
            previousMethodData.TotalAllocations += totalAlloc;
            StackTime stackTime = previousMethodData.StackTime;
            stackTime.OtherMethodsDuration += methodDuration;
            previousMethodData.StackTime = stackTime;
            
            threadData.MethodStack[previousIndex] = previousMethodData;
        }
        
        threadData.MethodStack.RemoveRange(index, threadData.MethodStack.Count - index);
    }
}
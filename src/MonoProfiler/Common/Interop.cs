using System;
using System.Runtime.InteropServices;
using MonoProfiler.Enums;
using MonoProfiler.Handlers;
using MonoProfiler.Models;
using MonoProfiler.Models.Mono;

namespace MonoProfiler.Common;

public static partial class Interop
{
    [LibraryImport(Constants.MonoRuntimePath, EntryPoint = "mono_profiler_create")]
    private static unsafe partial void* Create(ProfilerData* monoProfiler);
    
    [LibraryImport(Constants.MonoRuntimePath, EntryPoint = "mono_profiler_enable_allocations")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool EnableAllocations();

    [LibraryImport(Constants.MonoRuntimePath, EntryPoint = "mono_profiler_set_call_instrumentation_filter_callback")]
    public static unsafe partial void SetMethodFilterCallback(void* handle, MethodFilterDelegate callback);
    public unsafe delegate CallInstrumentationFlags MethodFilterDelegate(ProfilerData* profiler, MonoMethod* method);
    
    [LibraryImport(Constants.MonoRuntimePath, EntryPoint = "mono_profiler_set_method_enter_callback")]
    public static unsafe partial void SetMethodEnterCallback(void* handle, MethodEnterDelegate callback);
    public unsafe delegate void MethodEnterDelegate(ProfilerData* profiler, MonoMethod* method, MonoProfilerCallContext* context);
    
    [LibraryImport(Constants.MonoRuntimePath, EntryPoint = "mono_profiler_set_method_leave_callback")]
    public static unsafe partial void SetMethodLeaveCallback(void* handle, MethodLeaveDelegate callback);
    public unsafe delegate void MethodLeaveDelegate(ProfilerData* profiler, MonoMethod* method);
    
    [LibraryImport(Constants.MonoRuntimePath, EntryPoint = "mono_profiler_set_method_exception_leave_callback")]
    public static unsafe partial void SetMethodExceptionLeaveCallback(void* handle, MethodExceptionLeaveCallback callback);
    public unsafe delegate void MethodExceptionLeaveCallback(ProfilerData* profiler, MonoMethod* method, MonoObject* monoObject);
    
    [LibraryImport(Constants.MonoRuntimePath, EntryPoint = "mono_profiler_set_exception_throw_callback")]
    public static unsafe partial void SetExceptionThrowCallback(void* handle, ExceptionThrowCallback callback);
    public unsafe delegate void ExceptionThrowCallback(ProfilerData* profiler, MonoMethod* method, MonoObject* monoObject);
    
    [LibraryImport(Constants.MonoRuntimePath, EntryPoint = "mono_profiler_set_gc_allocation_callback")]
    public static unsafe partial void SetGcAllocationCallback(void* handle, GcAllocationCallback callback);
    public unsafe delegate void GcAllocationCallback(ProfilerData* profiler, MonoObject* monoObject);
    
    [LibraryImport(Constants.MonoRuntimePath, EntryPoint = "mono_profiler_set_gc_event_callback")]
    public static unsafe partial void SetGcEventCallback(void* handle, GcEventCallback callback);
    public unsafe delegate void GcEventCallback(ProfilerData* profiler, MonoProfilerGCEvent gcEvent, uint generation, bool isStart);
    
    [LibraryImport(Constants.MonoRuntimePath, EntryPoint = "mono_type_get_name_full")]
    public static unsafe partial char* GetFullName(MonoType* monoType, MonoTypeNameFormat format);
    
    [LibraryImport(Constants.MonoRuntimePath, EntryPoint = "mono_free")]
    public static unsafe partial void MonoFree(void* pointer);
    
    [LibraryImport(Constants.MonoRuntimePath, EntryPoint = "mono_domain_get")]
    public static unsafe partial MonoDomain* GetDomain();
    
    [LibraryImport(Constants.MonoRuntimePath, EntryPoint = "mono_get_string_class")]
    public static unsafe partial MonoClass* GetMonoStringClass();
    
    [UnmanagedCallersOnly]
    public static unsafe void Initialize()
    {
        Profiler profiler = Profiler.Instance;
        ProfilerData profilerData = new();

        ThreadData threadData = ThreadDataHandler.Instance.GetThreadData();
        threadData.MainThread = true;
        
        profilerData.Handle = Create(&profilerData);
        void* profilerHandle = profilerData.Handle;
        if (&profilerHandle == null)
        {
            Console.WriteLine("Failed to create profiler handle");
            return;
        }

        profiler.Data = profilerData;
        EnableAllocations();
        CallbackHandler.Instance.RegisterMethodFilterCallback(profilerData.Handle);
        WebSocketServerHandler.Instance.InitializeAsync();
    }
}
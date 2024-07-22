using System.Diagnostics;
using System.Text.Json;
using MonoProfiler.Enums;
using MonoProfiler.Models.Mono;

namespace MonoProfiler.Common;

public static unsafe class Constants
{
    public const string MonoRuntimePath = @"MonoBleedingEdge\EmbedRuntime\mono-2.0-bdwgc.dll";
    public const string OxidePluginNamespace = "Oxide.Plugins.";
    
    public const CallInstrumentationFlags CallInstrumentationTypes = CallInstrumentationFlags.Enter | 
                                                                    CallInstrumentationFlags.Leave | 
                                                                    CallInstrumentationFlags.ExceptionLeave;
    
    public static readonly MonoClass* MonoStringClass = Interop.GetMonoStringClass();

    public static readonly long ElapsedTicksMultiplier = 1000000000 / Stopwatch.Frequency;
    public const float NanosecondsMultiplier = 0.000001f;
    
    public static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        WriteIndented = true
    };
}
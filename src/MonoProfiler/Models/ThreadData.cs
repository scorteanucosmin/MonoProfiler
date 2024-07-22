using System;
using System.Collections.Generic;
using MonoProfiler.Models.Records;
using MonoProfiler.Models.Results;

namespace MonoProfiler.Models;

public class ThreadData
{
    public bool MainThread { get; set; }

    public List<MethodData> MethodStack { get; set; } = new();
    
    public Dictionary<IntPtr, MethodResult> MethodResults { get; set; } = new();
    
    public Dictionary<IntPtr, MemoryResult> MemoryResults { get; set; } = new();

    public GCRecord GcRecord { get; set; }
}
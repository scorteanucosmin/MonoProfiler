using System.Collections.Generic;
using MonoProfiler.Models.Records;

namespace MonoProfiler.Models;

public struct ProfilingSample()
{
    public List<MemoryRecord> MemoryRecords { get; init; } = new();
    
    public List<MethodRecord> MethodRecords { get; init; } = new();
    
    public List<AssemblyRecord> AssemblyRecords { get; init; } = new();
    
    //public GCRecord GcRecord { get; init; }
}
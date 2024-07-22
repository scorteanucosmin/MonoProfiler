using MonoProfiler.Common;

namespace MonoProfiler.Models.Records;

public struct AssemblyRecord
{
    public string AssemblyName { get; set; }
    
    public int TotalCalls { get; set; }
    
    public long TotalTime { get; set; }
    
    public long TotalTimeMs => (long)(TotalTime * Constants.NanosecondsMultiplier);
    
    public int TotalAllocations { get; set; }
    
    public int TotalExceptions { get; set; }
}
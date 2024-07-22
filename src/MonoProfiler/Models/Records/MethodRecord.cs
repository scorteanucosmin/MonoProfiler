using MonoProfiler.Common;

namespace MonoProfiler.Models.Records;

public struct MethodRecord
{
    public string MethodName { get; set; }
    
    public int Calls { get; set; }
    
    public long OwnTime { get; set; }
    
    public long OwnTimeMs => (long)(OwnTime * Constants.NanosecondsMultiplier);
    
    public long TotalTime { get; set; }
    
    public long TotalTimeMs => (long)(TotalTime * Constants.NanosecondsMultiplier);
    
    public int OwnAllocations { get; set; }
    
    public int TotalAllocations { get; set; }
    
    public int OwnExceptions { get; set; }
    
    public int TotalExceptions { get; set; }
}
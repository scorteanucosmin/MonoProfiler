namespace MonoProfiler.Models.Results;

public struct MethodResult
{
    public int Calls { get; set; }
    
    public long OwnTime { get; set; }
    
    public long TotalTime { get; set; }
    
    public int OwnAllocations { get; set; }
    
    public int TotalAllocations { get; set; }
    
    public int OwnExceptions { get; set; }
    
    public int TotalExceptions { get; set; }
}
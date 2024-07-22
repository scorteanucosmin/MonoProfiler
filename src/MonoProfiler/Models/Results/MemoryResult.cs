namespace MonoProfiler.Models.Results;

public struct MemoryResult
{
    public int AllocationsCount { get; set; }
    
    public int TotalAllocations { get; set; }
    
    public int InstanceSize { get; set; }
}
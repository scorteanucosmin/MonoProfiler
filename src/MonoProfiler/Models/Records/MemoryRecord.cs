
namespace MonoProfiler.Models.Records;

public struct MemoryRecord
{
    public string AssemblyName { get; set; } //todo: if is a plugin exctract the name from MonoHelper.Instance.GetClassName() -> should output plugin's name
    
    public string ClassName { get; set; }
    
    public int AllocationsCount { get; set; }
    
    public int TotalAllocations { get; set; }
    
}
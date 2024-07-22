
using MonoProfiler.Models.Mono;

namespace MonoProfiler.Models.Results;

public unsafe struct MethodData(MonoMethod* method)
{
    public MonoMethod* Method { get; set; } = method;

    public int OwnAllocations { get; set; }
    
    public int TotalAllocations { get; set; }

    public StackTime StackTime { get; set; } = new();
}
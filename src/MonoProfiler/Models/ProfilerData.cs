using MonoProfiler.Common;
using MonoProfiler.Models.Mono;

namespace MonoProfiler.Models;

public unsafe struct ProfilerData()
{
    public MonoDomain* MonoDomain { get; set; } = Interop.GetDomain();

    public void* Handle { get; set; }
}
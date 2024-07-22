using System.Runtime.InteropServices;

namespace MonoProfiler.Models.Mono;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct MonoObject
{
    public MonoVTable* vtable;
    
    public MonoThreadsSync* sync;
}
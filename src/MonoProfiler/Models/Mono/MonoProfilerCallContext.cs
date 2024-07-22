using System.Runtime.InteropServices;

namespace MonoProfiler.Models.Mono;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct MonoProfilerCallContext
{
    public MonoContext context;
    
    public void* interp_frame;
    
    public MonoMethod* method;
    
    public void* return_value;
    
    public void** args;
}